using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float bulletSpeed = 1;
    public Rigidbody2D rb;
    private int bounces;
    public int maxBounces = 3;
    public bool isActive = false;
    public AudioSource audio;
    
    public GameObject particleEffectPrefab;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "sciany" || collider.gameObject.name == "lustra")
        {   
            audio.Play();
            
            GameObject particleEffect = Instantiate(particleEffectPrefab, this.transform.position, Quaternion.identity);
            
            Destroy(particleEffect, 0.7f);
            
            if (bounces == maxBounces)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Vector2 directionToWall = collider.transform.position - transform.position;
            Vector2 normal = directionToWall.normalized;
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);
            if (Mathf.Abs(normal.x) <= Mathf.Abs(normal.y))
            {
                rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            }
            bounces++;
        }
    }
    
    void Start()
    {
    }

    // Update is called once per frame
    
    void FixedUpdate()
    {   
        
    }
}
