using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{ /*

    // Dictionary to store player names and their scores
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    // You can reference UI text here if you want to display the score
    public Text scoreDisplay;

    public GameManager gm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       foreach (var player in gm.players)
        {
            player.score = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementScore(string playerName)
    {
        Player player = players.Find(p => p.name == playerName);

        if (player != null)
        {
            player.score++;
            Debug.Log($"{playerName} has been tagged! New Score: {player.score}");
            UpdateScoreDisplay();
        }
        else
        {
            Debug.Log("Player not found!");
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = "Scores:\n";
            foreach (var player in gm.players)
            {
                scoreDisplay.text += $"{player.name}: {player.score}\n";
            }
        }
    }
    */
}
