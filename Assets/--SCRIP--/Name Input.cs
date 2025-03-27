using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Button submitButton;
    public TextMeshProUGUI playerNameLabel;

    private void Start()
    {
        // Add listener for when user finishes editing input field
        nameInputField.onEndEdit.AddListener(OnNameSubmit);
    }

    private void OnNameSubmit(string text = "")
    {
        string playerName = text;
        // Here you would call a method to set the player name
        Player.Instance.SetPlayerName(playerName);
        playerNameLabel.text = playerName;
        Player.Instance.SavePlayer();
    }

    private void Update()
    {
        if (playerNameLabel != null && playerNameLabel.text != Player.Instance.playerName)
        {
            playerNameLabel.text = Player.Instance.playerName;
        }
    }

}

