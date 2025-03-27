using Unity.Netcode;
using TMPro;
using UnityEngine;

public class DisplayLevel : MonoBehaviour
{
    public TextMeshProUGUI showLevel; 

    private void Start()
    {
        if (Player.Instance.level == 0)
        {
            Debug.LogError("level 0");
        }
        if (showLevel == null)
        {
            Debug.LogError("TextMeshProUGUI reference is missing!");
        }
    }

    
    private void Update()
    {
        // Update the level display
        if (Player.Instance.level != 0 && showLevel != null)
        {
            showLevel.text = "Level: " + Player.Instance.level.ToString();
        }
    }
}
