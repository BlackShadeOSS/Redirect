using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius in which the shard will detect players
    public float acceleration = 1.01f; // Acceleration towards the player

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    float speed = 0.001f;
    void FixedUpdate()
    {
        // Get all colliders within the detection radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Find the closest player
        GameObject closestPlayer = null;
        float minDistance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    closestPlayer = collider.gameObject;
                    minDistance = distance;
                }
            }
        }

        // If a player is found, accelerate towards it
        if (closestPlayer != null)
        {
            Vector2 direction = (closestPlayer.transform.position - transform.position).normalized;
            rb.AddForce(direction * speed);
            speed = acceleration * speed;
        }
    }
}