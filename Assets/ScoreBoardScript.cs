using System.Linq;
using Events;
using UnityEngine;
using TMPro;
using System;
using System.Collections;


public class ScoreBoardScript : MonoBehaviour, healthEvent
{
    // Array to store the player scores (index 0 for player 1, index 1 for player 2)
    public int[] playerScores = new int[2];

    // Text components to display the players' scores
    public TMP_Text Player1Score;
    public TMP_Text Player2Score;

    // GameObjects for players (if needed for other logic)
    public GameObject P1;
    public GameObject P2;

    // Player starting positions
    private Vector3 P1StartingPosition;
    private Vector3 P2StartingPosition;

    // End game UI Panel
    public GameObject endGamePanel;
    public TMP_Text endGameText;

    // Define the points required to win the game
    public int pointsToWin = 3;

    // Bullet cleanup settings (assuming bullets are tagged as "Bullet")
    public string bulletTag = "Bullet";

    // Respawn delay in seconds
    public float respawnDelay = 2f;
    
    public GameObject player;
    public GameObject player2;

    void Start()
    {
        // Initialize starting positions
        P1StartingPosition = new Vector3(0, -20, 0);
        P2StartingPosition = new Vector3(0, 20, 0);

        eventRegistry.addHealthEvent(this);
        UpdateScoreDisplay();  // Update the UI on start
        endGamePanel.SetActive(false);  // Ensure the end game panel is hidden at the start
    }

    // Method that is triggered when a player dies
    public void onDeath(GameObject player)
    {
        // Extract the player index from the player's name (assuming name format is "Player1" or "Player2")
        int playerIndex = player.GetComponent<Player>().index;  // Converts "Player1" -> 0, "Player2" -> 1

        // Increase the score of the other player
        playerScores[(playerIndex + 2) % 2]++; // If player 1 dies (index 0), increase player 2's score (index 1), and vice versa

        UpdateScoreDisplay();  // Update the score display UI after a score change

        // Clean up all bullets
        CleanUpBullets();

        // Disable the player who died
        player.SetActive(false);

        // After a delay, respawn the player and return both to starting positions
        StartCoroutine(RespawnPlayer(player, playerIndex));

        // Check if either player has reached the required points to win
        if (playerScores[0] >= pointsToWin || playerScores[1] >= pointsToWin)
        {
            EndGame();  // End the game if a player wins
        }
    }

    // This method can be used if you want to handle player hits, you can update as needed
    public void onHit(GameObject player, float x)
    {
        // Implement your logic for handling player hits (if necessary)
    }

    // Update is called once per frame
    void Update()
    {
        // If you need to do something every frame (e.g., updating the score text continuously), do it here
    }

    // Method to update the score display in the UI
    private void UpdateScoreDisplay()
    {
        Player1Score.text = playerScores[0].ToString();  // Update Player 1's score display
        Player2Score.text = playerScores[1].ToString();  // Update Player 2's score display
    }

    // Method to end the game and display the winner
    public void EndGame()
    {
        // Determine the winner
        string winner = playerScores[0] >= pointsToWin ? "Player 1 Wins!" : "Player 2 Wins!";

        // Show the end game panel
        endGamePanel.SetActive(true);
        endGameText.text = winner;  // Display the winner's message
        
        player.GetComponent<Player>().paused = true;
        player.GetComponent<Movement>().paused = true;
        player2.GetComponent<Player>().paused = true;
        player2.GetComponent<Movement>().paused = true;
        StartCoroutine(exitOO());
    }

    public IEnumerator exitOO()
    {
        yield return new WaitForSeconds(5); 
        Application.Quit();
    }

    // Method to clean up all bullets in the scene
    private void CleanUpBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag(bulletTag);  // Get all objects with the bullet tag
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);  // Destroy each bullet
        }
    }

    // Coroutine to respawn the player after a delay
    public IEnumerator RespawnPlayer(GameObject player, int playerIndex)
    {
        yield return new WaitForSeconds(respawnDelay);  // Wait for the respawn delay

        // Respawn the player at the starting position
        if (playerIndex == 1)  // Player 1
        {
            player.transform.position = P1StartingPosition;
        }
        else  // Player 2
        {
            player.transform.position = P2StartingPosition;
        }
        
        player.GetComponent<Zycie>().resetHealth();
        player.GetComponent<Movement>().unhit();
        player.GetComponent<Player>().unPause();
        player.SetActive(true);  // Re-enable the player object
    }
}
