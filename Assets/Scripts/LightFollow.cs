using UnityEngine;
using UnityEngine.InputSystem;
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
    
    private Vector2 look;
    private Vector2 _movementInput;
    private List<LineRenderer> rayLines = new List<LineRenderer>();
    private Light flashlight;
    
    void Start()
    {
        // Get flashlight component
        flashlight = LightToRotate.GetComponentInChildren<Light>();
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
        // If flashlight is off, disable all rays
        if (flashlight != null && !flashlight.enabled)
        {
            foreach (var line in rayLines)
            {
                line.enabled = false;
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
                    
                    // Optional: Add visual effect to hit object
                    SpriteRenderer hitRenderer = hit.collider.GetComponent<SpriteRenderer>();
                    if (hitRenderer != null)
                    {
                        // Set emission briefly
                        hitRenderer.material.SetColor("_EmissionColor", rayColor * 0.3f);
                    }
                    
                    // Calculate reflection
                    Vector3 reflectionDirection = Vector3.Reflect(currentDirection, hit.normal);
                    
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
    
    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>().normalized;
    }
    
    private void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
}