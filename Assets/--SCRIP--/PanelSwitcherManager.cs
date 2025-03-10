using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelSwitcherManager : MonoBehaviour
{
    // Array to hold the panels
    public GameObject[] panels;

    // Track the current panel index
    private int currentPanelIndex = 0;

    private Selectable[] firstButtons;

    // Start is called before the first frame update
    void Start()
    {
        firstButtons = new Selectable[panels.Length];

        for (int i = 0; i < panels.Length; i++)
        {
            Selectable[] buttons = panels[i].GetComponentsInChildren<Selectable>();
            if (buttons.Length > 0)
            {
                firstButtons[i] = buttons[0]; // Get the first button in the panel
            }
        }

        // Make sure the panels array is not empty
        if (panels.Length == 0)
        {
            Debug.LogError("No panels assigned to the PanelSwitcher!");
            return;
        }

        // Initially, show the first panel
        ShowPanel(currentPanelIndex);
    }

    // Switch to the next panel
    public void NextPanel()
    {
        // Increment the index
        currentPanelIndex++;

        // Loop back to the first panel if we've reached the end of the array
        if (currentPanelIndex >= panels.Length)
        {
            currentPanelIndex = 0;
        }

        if (firstButtons[currentPanelIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButtons[currentPanelIndex].gameObject);
        }

        ShowPanel(currentPanelIndex);
    }

    // Switch to the previous panel
    public void PreviousPanel()
    {
        // Decrement the index
        currentPanelIndex--;

        // Loop back to the last panel if we're at the beginning of the array
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = panels.Length - 1;
        }

        if (firstButtons[currentPanelIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButtons[currentPanelIndex].gameObject);
        }

        ShowPanel(currentPanelIndex);
    }

    // Show the panel at the given index and hide all others
    private void ShowPanel(int index)
    {
        // Hide all panels first
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // Show the selected panel
        panels[index].SetActive(true);
    }
}
