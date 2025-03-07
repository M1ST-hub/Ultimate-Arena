using UnityEngine;
using UnityEngine.UI;

public class BackButtons : MonoBehaviour
{
    private Button backButton;

    private UIinputManager inputManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backButton = GetComponent<Button>();
        inputManager = GameObject.Find("InputManager").GetComponent<UIinputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.isBacken)
        {
            // Simulate the button click
            SimulateButtonClick();
        }
    }

    private void SimulateButtonClick()
    {
        if (backButton != null)
        {
            // Trigger the button's onClick event
            backButton.onClick.Invoke();
        }
    }
}
