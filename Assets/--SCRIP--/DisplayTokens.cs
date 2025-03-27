using Unity.Netcode;
using TMPro;
using UnityEngine;

public class DisplayTokens : MonoBehaviour
{
    public TextMeshProUGUI showTokens;

    private void Start()
    {
        if (Player.Instance.tokens == 0)
        {
            Debug.LogError("No tokens");
        }
        if (showTokens == null)
        {
            Debug.LogError("TextMeshProUGUI reference is missing!");
        }
    }


    private void Update()
    {
        // Update the level display
        if (Player.Instance.tokens != 0 && showTokens != null)
        {
            showTokens.text = Player.Instance.tokens.ToString();
        }
    }
}
