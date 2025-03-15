using UnityEngine;
using System.Collections;

public class rESTARTER : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        player.SetActive(false);
        StartCoroutine(RespawnPlayer());
    }
    
    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(0.2f);  // Wait for the respawn delay
        player.SetActive(true);  // Re-enable the player object
    }
}
