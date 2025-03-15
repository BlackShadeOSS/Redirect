using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
    [HideInInspector]
    public float rayWidth = 0.05f;
    
    // Fix max bounces at 2
    [SerializeField]
    private int maxBounces = 2;
    
    // Light color and intensity
    public Color mainLightColor = Color.white;
    [Range(0.5f, 2.0f)]
    public float baseIntensity = 1.2f;    // Stronger initial intensity
    [Range(0.2f, 0.7f)]
    public float intensityDecay = 0.3f;   // More aggressive decay per bounce
    
    // Light shape settings
    [Range(20f, 90f)]
    public float spotAngle = 45f;         // Cone angle for spotlight effect
    public float falloffStrength = 0.3f;   // Requested falloff strength
    
    // Prevent additive behavior options
    [SerializeField]
    private bool nonAdditiveLighting = true;
    
    // Property for controlling volumetric intensity
    [Range(0f, 1f)]
    public float volumetricIntensity = 0.0f;
    
    [Range(0f, 1f)]
    public float shadowStrength = 1.0f;
    
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
        else
        {
            // Set the main flashlight's shadow strength
            flashlight.shadowIntensity = 1.0f;
            flashlight.shadowVolumeIntensity = volumetricIntensity;
            
            // Attempt to set composite mode if available
            SetNonAdditiveBehavior(flashlight);
        }
        
        _lookingDirection = GetComponent<LookingDirection>();
        
        // Initialize reflection lights
        CreateReflectionLights();
        
        // Initialize position tracking arrays
        InitializePositionTracking();
    }

    private void SetNonAdditiveBehavior(Light2D light)
    {
        if (!nonAdditiveLighting) return;
        
        // Try all possible ways to set non-additive behavior via reflection
        try {
            PropertyInfo compositeProperty = typeof(Light2D).GetProperty("compositeOperation");
            if (compositeProperty != null)
            {
                // Values typically are: 0 = Additive, 1 = Alpha, 2 = Custom
                // Alpha blend mode should prevent additive stacking
                compositeProperty.SetValue(light, 1); // Alpha blend mode
            }
        }
        catch (System.Exception) {
            // Silently fail if property isn't available
        }
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
            
            // Try to set non-additive behavior
            SetNonAdditiveBehavior(light2D);
            
            // set spot angle
            light2D.
            
            // Configure light properties
            light2D.color = mainLightColor;  // Start with white
            light2D.intensity = 0;  // Start with zero intensity
            
            // Set appropriate radius and falloff
            float baseRadius = maxRayDistance * 0.4f;  
            light2D.pointLightOuterRadius = baseRadius;
            light2D.pointLightInnerRadius = baseRadius * (1.0f - falloffStrength);  
            
            // Set shadows for more realism - increase to 1.0 for full strength
            light2D.shadowIntensity = 1.0f;
            light2D.shadowVolumeIntensity = volumetricIntensity;
            
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
                            
                            // Calculate bounce intensity - more aggressive decay
                            float bounceIntensity = baseIntensity;
                            for (int b = 0; b <= bounce; b++)
                            {
                                bounceIntensity *= intensityDecay;
                            }
                            
                            // For non-additive lighting, further reduce intensity to prevent overlap issues
                            if (nonAdditiveLighting && bounce > 0)
                            {
                                bounceIntensity *= 0.7f;  // Reduce intensity of bounce lights
                            }
                            
                            // Calculate bounce color - more subtle shift toward warmer tones
                            Color bounceColor = mainLightColor;
                            if (bounce > 0)
                            {
                                // For bounce 1, shift slightly toward yellow/orange
                                float shift = 0.1f * bounce;
                                bounceColor = new Color(
                                    Mathf.Min(bounceColor.r, 1f),
                                    Mathf.Min(bounceColor.g, 1f) - shift * 0.2f,
                                    Mathf.Max(bounceColor.b - shift * 0.4f, 0f)
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
        
        // Set light properties
        light2D.color = lightColor;
        light2D.intensity = intensity;

        light2D.shadowIntensity = shadowStrength;
        
        // Scale radius slightly based on distance
        float baseRadius = maxRayDistance * 0.4f;
        float radiusScale = 1.0f + (hitDistance / maxRayDistance) * 0.2f;
        
        // Apply fall-off settings
        light2D.pointLightOuterRadius = baseRadius * radiusScale;
        light2D.pointLightInnerRadius = light2D.pointLightOuterRadius * (1.0f - falloffStrength);
        
        // Enable the light
        light2D.enabled = true;
        
        // For spotlight types, make sure it's properly oriented
        if (light2D.lightType.ToString() == "Spot")
        {
            // Spot lights might need additional orientation help
            // This works because the light is already oriented with transform.rotation
            
            // Try to set falloff for spotlight if possible
            var falloffProperty = light2D.GetType().GetProperty("falloffIntensity");
            if (falloffProperty != null)
            {
                falloffProperty.SetValue(light2D, falloffStrength);
            }
        }
        else
        {
            // For point lights, we use a directional cookie to fake a spotlight effect
            // This is a backup if the Spot type isn't available
            // The directionality is achieved by changing the light's transform orientation
            
            // We can narrow the effective radius in the direction we don't want light
            // by making the light position slightly offset in the direction of reflection
            float offset = 0.1f;
            lightObj.transform.position = position + direction.normalized * offset;
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