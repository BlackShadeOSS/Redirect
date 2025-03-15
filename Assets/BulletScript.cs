using Unity.Mathematics.Geometry;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float bulletSpeed = 1;
    public Rigidbody2D rb;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {   
        Vector2 directionToWall = collider.transform.position - transform.position;
        Vector2 normal = directionToWall.normalized;
        rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);
    }
    
    void Start()
    {
    }

    // Update is called once per frame
    
    void FixedUpdate()
    {   
        
    }
}
