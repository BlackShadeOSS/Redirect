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
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, normal);
            bounces++;
        } else if (collider.gameObject.name.Split(" ").Contains("Player"))
        {
            //int playerIndex = int.Parse(collider.gameObject.name.Split(" ").Last());
            //scoreBoard.GetComponent<ScoreBoardScript>().playerScores[playerIndex - 1]++;
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
