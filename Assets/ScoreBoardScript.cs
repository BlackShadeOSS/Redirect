using System.Linq;
using Events;
using UnityEngine;

public class ScoreBoardScript : MonoBehaviour, healthEvent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int[] playerScores = new int[2];
    void Start()
    {
        eventRegistry.addHealthEvent(this);
    }

    public void onDeath(GameObject player)
    {
        int playerIndex = int.Parse(player.name.Split(" ").Last());
        playerScores[(playerIndex + 1) % 2]++;
    }
    
    public void onHit(GameObject player, float x)
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
