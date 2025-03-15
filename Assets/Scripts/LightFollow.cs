using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal; // Dodane dla świateł 2D
using System.Collections;
using System.Collections.Generic;

public class LightFollow : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject LightToRotate;
    public float angleOffset = 90.0f;
    public float rotationSmoothTime = 20.0f;
    private LookingDirection _lookingDirection;
    
    // Reflection settings
    public LayerMask reflectiveSurfaces;
    public int rayCount = 5;               // Number of rays to cast from flashlight
    public float raySpread = 15f;          // Spread angle of rays in degrees
    public float maxRayDistance = 10f;     // Maximum distance of each ray
    public int maxBounces = 3;             // Maximum number of bounces per ray
    public Color rayColor = Color.yellow;  // Color of the reflection lines
    public float rayWidth = 0.05f;         // Width of the reflection lines
    
    // Nowe parametry dla symulacji odbicia światła
    public float reflectionIntensity = 0.5f;    // Intensywność odbicia
    public float reflectionSpread = 30f;        // Rozpraszanie odbicia
    public int reflectionRayCount = 8;          // Liczba promieni odbicia
    public float reflectionDistance = 15f;      // Maksymalna odległość odbicia
    
    private Vector2 look;
    private Vector2 _movementInput;
    private List<LineRenderer> rayLines = new List<LineRenderer>();
    private Light2D flashlight;  // Zmienione na Light2D
    private List<List<Vector3>> reflectionPoints = new List<List<Vector3>>();
    private List<GameObject> reflectionLights = new List<GameObject>();

    void Start()
    {
        // Inicjalizacja tablicy punktów odbicia
        for (int i = 0; i < rayCount; i++)
        {
            reflectionPoints.Add(new List<Vector3>());
        }
        
        // Get flashlight component (teraz jako Light2D)
        flashlight = LightToRotate.GetComponentInChildren<Light2D>();
        if (flashlight == null)
        {
            Debug.LogError("Nie znaleziono komponentu Light2D w LightToRotate!");
        }
        
        _lookingDirection = GetComponent<LookingDirection>();
        
        // Create line renderers for each ray
        for (int i = 0; i < rayCount; i++)
        {
            GameObject rayObj = new GameObject($"FlashlightRay_{i}");
            rayObj.transform.SetParent(transform);
            
            LineRenderer line = rayObj.AddComponent<LineRenderer>();
            line.positionCount = maxBounces + 2; // Start point + max bounces + end point
            line.startWidth = rayWidth;
            line.endWidth = rayWidth * 0.5f;
            
            // Set material and color
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = rayColor;
            line.endColor = new Color(rayColor.r, rayColor.g, rayColor.b, 0.2f);
            
            line.enabled = false;
            rayLines.Add(line);
        }
        
        // Inicjalizacja świateł odbicia
        InitializeReflectionLights();
    }

    private void InitializeReflectionLights()
    {
        // Usuń istniejące światła odbicia
        foreach (GameObject light in reflectionLights)
        {
            Destroy(light);
        }
        reflectionLights.Clear();

        // Utwórz nowe światła odbicia jako Light2D
        for (int i = 0; i < rayCount; i++)
        {
            GameObject reflectionLight = new GameObject($"ReflectionLight_{i}");
            reflectionLight.transform.SetParent(transform);
            
            // Dodaj komponent Light2D zamiast standardowego Light
            Light2D light2D = reflectionLight.AddComponent<Light2D>();
            light2D.lightType = Light2D.LightType.Point;  // Światło punktowe
            light2D.color = rayColor;
            light2D.intensity = reflectionIntensity;
            light2D.pointLightOuterRadius = reflectionDistance;
            light2D.pointLightInnerRadius = reflectionDistance * 0.5f;
            light2D.shadowIntensity = 0.7f;
            light2D.enabled = false;
            
            reflectionLights.Add(reflectionLight);
        }
    }

    void Update()
    {
        RotateFlashlight();
        CastRays();
    }

    void RotateFlashlight()
    {
        Quaternion playerRotation = _lookingDirection.GetPlayerRotation();
        // Apply the angle offset by rotating around the Z axis
        LightToRotate.transform.rotation = playerRotation * Quaternion.Euler(0, 0, angleOffset);
    }

    void CastRays()
    {
        // If flashlight is off, disable all rays and reflection lights
        if (flashlight != null && !flashlight.enabled)
        {
            foreach (var line in rayLines)
            {
                line.enabled = false;
            }
            
            // Wyłącz wszystkie światła odbicia
            foreach (var lightObj in reflectionLights)
            {
                Light2D light = lightObj.GetComponent<Light2D>();
                if (light != null)
                {
                    light.enabled = false;
                }
            }
            return;
        }
        
        // Get the forward direction of the light based on its current rotation
        // Use forward (up in 2D) to match the actual light direction with the angle offset applied
        Vector3 baseDirection = LightToRotate.transform.up;
        
        // Origin is the light's position
        Vector3 rayOrigin = LightToRotate.transform.position;
        
        // Calculate the angle between rays
        float angleStep = rayCount > 1 ? raySpread / (rayCount - 1) : 0;
        float startAngle = -raySpread / 2;
        
        // Cast each ray
        for (int i = 0; i < rayCount; i++)
        {
            // Upewnij się, że mamy dostęp do światła odbicia dla tego promienia
            if (i >= reflectionLights.Count)
            {
                continue;
            }
            
            // Domyślnie wyłącz światło odbicia dla tego promienia
            Light2D reflectionLight = reflectionLights[i].GetComponent<Light2D>();
            reflectionLight.enabled = false;
            
            // Clear previous reflection points for this ray
            reflectionPoints[i].Clear();
            
            // Calculate ray direction with spread
            float angle = startAngle + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * baseDirection;
            
            // Track ray positions for line renderer
            List<Vector3> rayPositions = new List<Vector3>();
            rayPositions.Add(rayOrigin);
            
            Vector3 currentOrigin = rayOrigin;
            Vector3 currentDirection = rayDirection;
            
            // Calculate bounces
            bool hitSomething = false;
            for (int bounce = 0; bounce < maxBounces; bounce++)
            {
                RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxRayDistance, reflectiveSurfaces);
                
                if (hit.collider != null)
                {
                    hitSomething = true;
                    
                    // Record hit point
                    Vector3 hitPoint = hit.point;
                    rayPositions.Add(hitPoint);
                    reflectionPoints[i].Add(hitPoint);  // Add to reflection points array
                    
                    // Optional: Add visual effect to hit object
                    SpriteRenderer hitRenderer = hit.collider.GetComponent<SpriteRenderer>();
                    if (hitRenderer != null)
                    {
                        // Sprawdź, czy materiał wspiera emisję
                        if (hitRenderer.material.HasProperty("_EmissionColor"))
                        {
                            // Set emission briefly
                            hitRenderer.material.SetColor("_EmissionColor", rayColor * 0.3f);
                            hitRenderer.material.EnableKeyword("_EMISSION");
                        }
                    }
                    
                    // Calculate reflection
                    Vector3 reflectionDirection = Vector3.Reflect(currentDirection, hit.normal);
                    
                    // Symuluj odbicie światła dla wszystkich odbić
                    SimulateLightReflection(i, hitPoint, hit.normal, currentDirection, reflectionDirection);
                    
                    // Update for next bounce
                    currentOrigin = hitPoint + reflectionDirection * 0.01f; // Small offset
                    currentDirection = reflectionDirection;
                }
                else
                {
                    // Ray didn't hit - extend to max distance
                    Vector3 endPoint = currentOrigin + currentDirection * maxRayDistance;
                    rayPositions.Add(endPoint);
                    break;
                }
            }
            
            // Update line renderer
            LineRenderer line = rayLines[i];
            
            // Set positions even if we didn't hit anything (to show the rays)
            line.enabled = true;
            line.positionCount = rayPositions.Count;
            
            for (int p = 0; p < rayPositions.Count; p++)
            {
                line.SetPosition(p, rayPositions[p]);
            }
            
            // Set gradient based on distance
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(rayColor, 0);
            colorKeys[1] = new GradientColorKey(rayColor, 1);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(0.8f, 0);
            alphaKeys[1] = new GradientAlphaKey(0.2f, 1);
            
            gradient.SetKeys(colorKeys, alphaKeys);
            line.colorGradient = gradient;
        }
    }

    private void SimulateLightReflection(int rayIndex, Vector3 hitPoint, Vector3 hitNormal, Vector3 incomingDirection, Vector3 reflectionDirection)
    {
        if (rayIndex >= reflectionLights.Count)
            return;
            
        GameObject reflectionLightObj = reflectionLights[rayIndex];
        Light2D light2D = reflectionLightObj.GetComponent<Light2D>();
        
        if (light2D == null)
            return;
            
        // Ustaw pozycję światła w miejscu odbicia
        reflectionLightObj.transform.position = hitPoint;
        
        // Włącz światło i ustaw jego parametry
        light2D.enabled = true;
        light2D.color = rayColor;
        light2D.intensity = reflectionIntensity;
        
        // Dodaj efekt pulsowania światła
        StartCoroutine(PulseReflectionLight(light2D, rayIndex));
    }

    private IEnumerator PulseReflectionLight(Light2D light2D, int rayIndex)
    {
        this.StopCoroutine($"PulseLight_{rayIndex}");
        
        float initialIntensity = light2D.intensity;
        float pulseDuration = 0.8f;
        float elapsedTime = 0f;
        
        // Upewnij się, że światło jest włączone na początku
        light2D.enabled = true;

        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Modyfikacja intensywności światła przez czas
            float normalizedTime = elapsedTime / pulseDuration;
            
            // Użyj krzywej dla naturalizacji efektu pulsowania
            float pulseIntensity = initialIntensity * (1f - Mathf.Pow(normalizedTime, 1.5f));
            light2D.intensity = pulseIntensity;
            
            // Modyfikuj również promień światła dla lepszego efektu
            float radiusMultiplier = 1f + normalizedTime * 0.5f;
            light2D.pointLightOuterRadius = reflectionDistance * radiusMultiplier;
            
            yield return null;
        }

        // Wyłącz światło po zakończeniu animacji
        light2D.enabled = false;
    }

    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>().normalized;
    }

    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
}