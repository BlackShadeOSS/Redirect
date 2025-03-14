using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
     public float bulletTTL = 2.0f;
     public float bulletVelo = 8.0f;
     public float bulletKnockbackFactor = 1.0f;
     public float damage = 10.0f;
     private float timeFactor = 1.0f;
    void enemyHit(GameObject hitObject)
    {
        // Get the enemy script
        Enemy enemy = hitObject.GetComponent<Enemy>();
        // Call the enemyHit function
        enemy.enemyHit(this.gameObject);
        // Destroy the bullet
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    // Check if the bullet has hit an enemy
    if (collision.gameObject.CompareTag("Enemy"))
    {
        if(collision.gameObject.GetComponent<Player>() != null) return;
        // Call the enemyHit function
        enemyHit(collision.gameObject);

        // Get the Rigidbody2D component of the enemy
        Rigidbody2D enemyRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        // Calculate the force to be applied
        Vector2 force = transform.right * bulletVelo * bulletKnockbackFactor / 100;

        // Apply the force to the enemy
        enemyRigidbody.AddForce(force);
    
        // destroy itself
        Destroy(gameObject);
    }
    if (collision.gameObject.CompareTag("Skill"))
    {
        Debug.Log("szkyl");
        SlowTimeCircle slowTimeCircle = collision.gameObject.GetComponent<SlowTimeCircle>();
        this.timeFactor = slowTimeCircle.getTimeFactor();
    }
    else
    {
        this.timeFactor = 1.0f;
    }
}

    public float getDamage()
    {
        return this.damage;
    }
}
