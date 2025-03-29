using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerID;  // Unique ID for the player (could be based on network or index)
    public int mostTags = 0; // Tracks the number of players this player has tagged
    public float timeSurvived = 0f; // Tracks how long the player has survived (untagged time)
    public float timeAsTagger = 0f; // Tracks how long the player has been the tagger

    private bool isTagger = false;
    private bool isUntagged = true;

    private float taggedTime = 0f;  // Time the player has been the tagger

    public GameManager gameManager;  // Reference to GameManager (assumed to be a singleton)

    void Update()
    {
        // If the player is the tagger, track the time they have been the tagger
        if (isTagger)
        {
            taggedTime += Time.deltaTime;
            timeAsTagger = taggedTime; // Update the timeAsTagger with the current tagged time
        }

        // If the player is not the tagger (i.e., they are untagged), track the time survived
        if (!isTagger && isUntagged)
        {
            timeSurvived += Time.deltaTime;
        }
    }

    // Call this function when the player becomes the tagger
    public void BecomeTagger()
    {
        isTagger = true;
        isUntagged = false; // Mark the player as tagged
    }

    // Call this function when the player stops being the tagger
    public void StopBeingTagger()
    {
        isTagger = false;
        isUntagged = true; // Mark the player as untagged
    }

    // Call this function when the player tags another player
    public void TagAnotherPlayer()
    {
        mostTags += 1; // Increment the player's tag count
    }
}