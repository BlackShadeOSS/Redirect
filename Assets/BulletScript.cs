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
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "sciany")
        {
            if (bounces == maxBounces)
            {
                Destroy(this.gameObject);
                return;
            }

            Vector2 directionToWall = collider.transform.position - transform.position;
            Vector2 normal = directionToWall.normalized;

            // Ensure the normal vector reflects properly in both horizontal and vertical cases
            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            {
                // Horizontal wall (left/right)
                rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            }
            else
            {
                // Vertical wall (top/bottom)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
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
