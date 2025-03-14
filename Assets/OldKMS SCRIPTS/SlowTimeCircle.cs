using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimeCircle : MonoBehaviour
{   
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Sprite slowTimeCircleSprite; // The sprite for the slow time circle
    [SerializeField] private Sprite slowTimeSandSprite; // The sprite for the slow time sand
    [SerializeField] private float sizeSmoothness = 2f; // The sprite for the slow time sand
    [SerializeField] private float timeFactor = 0.1f; // The time factor for the slow time effect
    
    private Collider2D slowTimeCircleCollider; // The collider for the slow time circle
    
    private List<Enemy> affectedEnemies = new List<Enemy>(); // The list of enemies affected by the slow time effect
    
    public float slowTimeEffectDuration = 15f; // Duration of the slow time effect in seconds
    public float activationDelay = 3f; // Delay before the slow time effect is applied
    private float desiredScale = 1; // The desired scale of the slow time circle
    private void Start()
    {   
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = slowTimeSandSprite;
        StartCoroutine(ActivateAfterDelay());
    }
    private void Update()
    {
        // Smoothly scale the slow time circle to the desired scale
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * desiredScale, Time.deltaTime * sizeSmoothness);
    }

    private IEnumerator ActivateAfterDelay()
    {
        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);
        
        // Change the sprite to the slow time circle
        spriteRenderer.sprite = slowTimeCircleSprite;
        
        // Scale the circle 10 times its original size
        this.desiredScale = 10;
        
        // Wait for the durme effect
        yield return new WaitForSeconds(slowTimeEffectDuration);
        
        

        // Remove the slow time circle
        Destroy(gameObject);
    }

    public void restore()
    {
        foreach (Enemy en in this.affectedEnemies)  
        {
            en.restore();            
        }
    }

    public float getTimeFactor()
    {
        return this.timeFactor;
    }
    public void addEnemy(Enemy enemy)
    {
       // if the enemy is not already affected by the slow time effect, add it to the list
        if (!affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
        }
    }
}