using UnityEngine;
using UnityEngine.UI;

public class BackButtons : MonoBehaviour
{
    private Button theButton;

    private UIinputManager inputManager;

    public UIStuff uiStuff;

    public float delay;
    public bool clickReset;
    public enum UIStuff
    {
        back,
        next,
        previous
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        theButton = GetComponent<Button>();
        inputManager = GameObject.Find("InputManager").GetComponent<UIinputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            clickReset = false;
        }

        if (delay <= 0)
        {
            clickReset = true;
        }

        if (inputManager.isBacken && uiStuff == UIStuff.back && clickReset)
        {
            // Simulate the button click
            SimulateButtonClick();
        }

        if (inputManager.isNexten && uiStuff == UIStuff.next && clickReset)
        {
            // Simulate the button click
            SimulateButtonClick();
        }

        if (inputManager.isPreviousen && uiStuff == UIStuff.previous && clickReset)
        {
            // Simulate the button click
            SimulateButtonClick();
        }
    }

    private void SimulateButtonClick()
    {
        if (theButton != null)
        {
            // Trigger the button's onClick event
            theButton.onClick.Invoke();
        }

        delay = 0.25f;
    }
}
