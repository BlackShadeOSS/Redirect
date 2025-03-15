using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class LightFollow : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject LightToRotate;
    public float angleOffset = 90.0f;
    private LookingDirection _lookingDirection;
    
    // Reflection settings
    public LayerMask reflectiveSurfaces;
    public int rayCount = 3;
    public float raySpread = 15f;
    public float maxRayDistance = 10f;
    [HideInInspector] // Ray lasers are now hidden
    public float rayWidth = 0.05f;
    
    // Fix max bounces at 2
    [SerializeField]
    private int maxBounces = 2;
    
    public Color rayColor = Color.yellow;
    
    // Realistic light parameters
    [Range(0.1f, 1.0f)]
    public float reflectionIntensity = 0.6f;
    [Range(0.5f, 0.9f)]
    public float intensityDecay = 0.7f;
    public bool useColorShift = true;
    public float colorShiftAmount = 0.1f;
    
    private Vector2 look;
    private Vector2 _movementInput;
    private Light2D flashlight;
    private List<GameObject> reflectionLights = new List<GameObject>();
    
    // Tracking last positions to prevent jitter
    private Vector3[][] lastHitPositions;
    private bool[][] hitActiveStatus;
    private float stabilityThreshold = 0.05f; // Distance threshold for position updates

    void Start()
    {
        // Enforce max bounces of 2
        maxBounces = Mathf.Min(maxBounces, 2);
        
        // Get flashlight component
        flashlight = LightToRotate.GetComponentInChildren<Light2D>();
        if (flashlight == null)
        {
            Debug.LogError("No Light2D component found in LightToRotate!");
        }
        
        _lookingDirection = GetComponent<LookingDirection>();
        
        // Initialize reflection lights
        CreateReflectionLights();
        
        // Initialize position tracking arrays
        InitializePositionTracking();
    }

    private void InitializePositionTracking()
    {
        // Create arrays to track last hit positions and active status
        lastHitPositions = new Vector3[rayCount][];
        hitActiveStatus = new bool[rayCount][];
        
        for (int i = 0; i < rayCount; i++)
        {
            lastHitPositions[i] = new Vector3[maxBounces];
            hitActiveStatus[i] = new bool[maxBounces];
            
            // Initialize with far-away positions
            for (int j = 0; j < maxBounces; j++)
            {
                lastHitPositions[i][j] = new Vector3(-9999, -9999, -9999);
                hitActiveStatus[i][j] = false;
            }
        }
    }

    private void CreateReflectionLights()
    {
        // Remove existing reflection lights
        foreach (GameObject light in reflectionLights)
        {
            Destroy(light);
        }
        reflectionLights.Clear();

        // Create one light for each possible reflection point
        int totalLights = rayCount * maxBounces;
        
        for (int i = 0; i < totalLights; i++)
        {
            GameObject lightObj = new GameObject($"ReflectionLight_{i}");
            lightObj.transform.SetParent(transform);
            
            // Add Light2D component
            Light2D light2D = lightObj.AddComponent<Light2D>();
            
            // Configure the light
            light2D.lightType = Light2D.LightType.Point;
            light2D.color = rayColor;
            light2D.intensity = 0; // Start with zero intensity
            light2D.pointLightOuterRadius = maxRayDistance * 0.3f;
            light2D.pointLightInnerRadius = light2D.pointLightOuterRadius * 0.5f;
            
            // Set shadows for more realism
            light2D.shadowIntensity = 0.7f;
            light2D.shadowVolumeIntensity = 0.2f;
            
            // Disable initially
            light2D.enabled = false;
            
            reflectionLights.Add(lightObj);
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
        // If flashlight is off, disable all reflection lights
        if (flashlight != null && !flashlight.enabled)
        {
            // Disable all reflection lights
            foreach (var lightObj in reflectionLights)
            {
                Light2D light = lightObj.GetComponent<Light2D>();
                if (light != null)
                {
                    light.enabled = false;
                }
            }
            
            // Reset hit tracking
            for (int i = 0; i < rayCount; i++)
            {
                for (int j = 0; j < maxBounces; j++)
                {
                    hitActiveStatus[i][j] = false;
                }
            }
            
            return;
        }
        
        // Get the forward direction of the light based on its current rotation
        Vector3 baseDirection = LightToRotate.transform.up;
        
        // Origin is the light's position
        Vector3 rayOrigin = LightToRotate.transform.position;
        
        // Calculate the angle between rays
        float angleStep = rayCount > 1 ? raySpread / (rayCount - 1) : 0;
        float startAngle = -raySpread / 2;
        
        // Track which lights are active this frame
        bool[,] currentHitActive = new bool[rayCount, maxBounces];
        
        // Cast each ray
        for (int i = 0; i < rayCount; i++)
        {
            // Calculate ray direction with spread
            float angle = startAngle + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * baseDirection;
            
            Vector3 currentOrigin = rayOrigin;
            Vector3 currentDirection = rayDirection;
            Color currentColor = rayColor;
            float currentIntensity = reflectionIntensity;
            
            // Calculate bounces - max of 2
            for (int bounce = 0; bounce < maxBounces; bounce++)
            {
                RaycastHit2D hit = Physics2D.Raycast(currentOrigin, currentDirection, maxRayDistance, reflectiveSurfaces);
                
                if (hit.collider != null)
                {
                    // We hit something
                    Vector3 hitPoint = hit.point;
                    
                    // Mark this hit as active
                    currentHitActive[i, bounce] = true;
                    
                    // Check if this hit position has moved significantly
                    bool significantChange = Vector3.Distance(hitPoint, lastHitPositions[i][bounce]) > stabilityThreshold;
                    bool statusChanged = hitActiveStatus[i][bounce] != currentHitActive[i, bounce];
                    
                    // Only update light position if there's a significant change
                    if (significantChange || statusChanged)
                    {
                        // Calculate light index
                        int lightIndex = i * maxBounces + bounce;
                        
                        // Update light position and properties
                        if (lightIndex < reflectionLights.Count)
                        {
                            // Calculate reflection (no randomness)
                            Vector3 reflectionDirection = Vector3.Reflect(currentDirection, hit.normal);
                            
                            // Decrease intensity with each bounce
                            float bounceIntensity = reflectionIntensity;
                            for (int b = 0; b <= bounce; b++)
                            {
                                bounceIntensity *= intensityDecay;
                            }
                            
                            // Shift color if enabled
                            Color bounceColor = rayColor;
                            if (useColorShift)
                            {
                                float shift = colorShiftAmount * bounce;
                                bounceColor = new Color(
                                    Mathf.Min(bounceColor.r + shift * 0.2f, 1f),
                                    Mathf.Min(bounceColor.g + shift * 0.1f, 1f),
                                    Mathf.Max(bounceColor.b - shift, 0f)
                                );
                            }
                            
                            // Configure the light
                            ConfigureStableLight(
                                reflectionLights[lightIndex],
                                hitPoint,
                                reflectionDirection,
                                bounceColor,
                                bounceIntensity,
                                hit.distance
                            );
                            
                            // Update the stored position
                            lastHitPositions[i][bounce] = hitPoint;
                        }
                    }
                    
                    // Calculate reflection for next bounce
                    Vector3 nextReflectionDirection = Vector3.Reflect(currentDirection, hit.normal);
                    currentOrigin = hitPoint + nextReflectionDirection * 0.01f;
                    currentDirection = nextReflectionDirection;
                    
                    // Decay intensity for next bounce
                    currentIntensity *= intensityDecay;
                    
                    // Shift color for next bounce
                    if (useColorShift)
                    {
                        currentColor = new Color(
                            Mathf.Min(currentColor.r + colorShiftAmount * 0.2f, 1f),
                            Mathf.Min(currentColor.g + colorShiftAmount * 0.1f, 1f),
                            Mathf.Max(currentColor.b - colorShiftAmount, 0f)
                        );
                    }
                }
                else
                {
                    // Ray didn't hit - mark this bounce as inactive
                    currentHitActive[i, bounce] = false;
                    
                    // Disable the corresponding light if it was previously active
                    if (hitActiveStatus[i][bounce])
                    {
                        int lightIndex = i * maxBounces + bounce;
                        if (lightIndex < reflectionLights.Count)
                        {
                            Light2D light = reflectionLights[lightIndex].GetComponent<Light2D>();
                            if (light != null)
                            {
                                light.enabled = false;
                            }
                        }
                    }
                    
                    break;
                }
            }
        }
        
        // Update active status for the next frame
        for (int i = 0; i < rayCount; i++)
        {
            for (int j = 0; j < maxBounces; j++)
            {
                hitActiveStatus[i][j] = currentHitActive[i, j];
                
                // If this hit is no longer active, disable its light
                if (!currentHitActive[i, j])
                {
                    int lightIndex = i * maxBounces + j;
                    if (lightIndex < reflectionLights.Count)
                    {
                        Light2D light = reflectionLights[lightIndex].GetComponent<Light2D>();
                        if (light != null)
                        {
                            light.enabled = false;
                        }
                    }
                }
            }
        }
    }

    private void ConfigureStableLight(GameObject lightObj, Vector3 position, Vector3 direction, 
                                   Color lightColor, float intensity, float hitDistance)
    {
        Light2D light2D = lightObj.GetComponent<Light2D>();
        if (light2D == null) return;
        
        // Position the light at the hit point
        lightObj.transform.position = position;
        
        // Orient the light in the reflection direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lightObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
        // Set stable light properties - no pulsing or animation
        light2D.color = lightColor;
        
        // Scale intensity based on distance but keep it stable
        float distanceScaledIntensity = intensity * Mathf.Clamp01(1.0f - (hitDistance / maxRayDistance) * 0.5f);
        light2D.intensity = distanceScaledIntensity;
        
        // Set light radius based on distance but keep it consistent
        float radiusScale = 1.0f + (hitDistance / maxRayDistance) * 0.3f;
        light2D.pointLightOuterRadius = maxRayDistance * 0.3f * radiusScale;
        light2D.pointLightInnerRadius = light2D.pointLightOuterRadius * 0.5f;
        
        // Enable the light
        light2D.enabled = true;
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
