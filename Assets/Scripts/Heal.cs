using System;
using UnityEngine;

public class Heal : MonoBehaviour
{
    private Zycie zycie;
    public float healAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the trigger zone
        if (other.CompareTag("Player"))  // Ensure the tag of the player is "Player"
        {
            heal(other.gameObject);
            Debug.Log("Leczonko");
        }
    }

    public void heal(GameObject player)
    {
        player.GetComponent<Zycie>().addHealth(healAmount);
        Destroy(gameObject); // Destroy the heal item
    }
}