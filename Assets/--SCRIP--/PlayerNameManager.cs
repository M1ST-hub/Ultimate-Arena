using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNameManager : NetworkBehaviour
{
    public static PlayerNameManager Instance;
    private NetworkVariable<string> playerName = new NetworkVariable<string>(string.Empty);

    public TextMeshProUGUI playerNameLabel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Call this method to set the player's name
    public void SetPlayerName(string newName)
    {
        if (IsOwner)
        {
            playerName.Value = newName;
            // Update name across the network
            SetPlayerNameOnServerRpc(newName);
        }
    }

    public string GetPlayerName()
    {
        return playerName.Value;
    }

    // Sync name on the server
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameOnServerRpc(string newName)
    {
        playerName.Value = newName;
    }

    // Sync the name across clients
    private void Update()
    {
        if (IsOwner && !string.IsNullOrEmpty(playerName.Value))
        {
            // You can also display the name or update the player’s name UI
            Debug.Log($"Player's name: {playerName.Value}");

            playerNameLabel.text = playerName.Value;
        }
    }
}
