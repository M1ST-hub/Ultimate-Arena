using Unity.Netcode;
using TMPro;
using UnityEngine; // Import TextMeshPro namespace

public class DisplayPlayerName : MonoBehaviour
{
    public TextMeshProUGUI playerNameLabel; // Reference to TextMeshProUGUI component

    private void Start()
    {
        if (playerNameLabel != null && playerNameLabel.text != Player.Instance.playerName)
        {
            playerNameLabel.text = Player.Instance.playerName;
        }
    }

    // Optionally, you could update this dynamically during gameplay
    private void Update()
    {
        if (playerNameLabel != null && playerNameLabel.text != Player.Instance.playerName)
        {
            playerNameLabel.text = Player.Instance.playerName;
        }
    }
}
