using Events;
using UnityEngine;

public class ScoreBoardScript : MonoBehaviour, healthEvent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int[] playerScores = new int[2];
     public eventRegistry _eventRegistry;
    
    void Start()
    {
        _eventRegistry.addHealthEvent(this);
    }

    public void onDeath(GameObject player)
    {
        Debug.Log(player.name);
    }
    
    public void onHit(GameObject player, float x)
    {
        Debug.Log("hit " + player.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
