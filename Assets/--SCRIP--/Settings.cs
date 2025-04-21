using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI value;
    public string sliderName;
    public int defaultValue;
    private float sliderValue;

    void Start()
    {
        // Get the components from the current GameObject and its children
        slider = GetComponent<Slider>();
        value = GetComponentInChildren<TextMeshProUGUI>();

        if (slider != null && value != null)
        {
            // Set the initial text value based on the current slider value
            UpdateText(slider.value);

            // Add listener to update the text when the slider value changes
            slider.onValueChanged.AddListener(UpdateText);
        }

        // Load saved value for the slider
        sliderValue = PlayerPrefs.GetFloat(sliderName, defaultValue);
        slider.value = sliderValue;
    }

    // Method to update the text label when the slider changes
    void UpdateText(float value)
    {
        // Update the displayText string based on the slider value
        this.value.text = value.ToString("F0");

        if (sliderName == "Deadzone Right")
        {
            this.value.text = value.ToString("F1");
        }
    }

    // Method to save the Deadzone Right value when the slider value changes
    public void OnDeadzoneSliderChanged(float value)
    {
        // Save the updated Deadzone Right value to PlayerPrefs
        PlayerPrefs.SetFloat("Deadzone Right", value);
    }

    // Method to save the FOV value when the slider value changes
    public void OnFOVSliderChanged(float value)
    {
        // Save the updated FOV value to PlayerPrefs
        PlayerPrefs.SetFloat("FOV", value);
    }

    // Method to save the Mouse Sensitivity X value when the slider value changes
    public void OnMouseSensitivityXSliderChanged(float value)
    {
        // Save the updated Mouse Sensitivity X value to PlayerPrefs
        PlayerPrefs.SetFloat("Mouse Sensitivity X", value);
    }

    // Method to save the Mouse Sensitivity Y value when the slider value changes
    public void OnMouseSensitivityYSliderChanged(float value)
    {
        // Save the updated Mouse Sensitivity Y value to PlayerPrefs
        PlayerPrefs.SetFloat("Mouse Sensitivity Y", value);
    }

    // Method to save the Controller Sensitivity X value when the slider value changes
    public void OnControllerSensitivityXSliderChanged(float value)
    {
        // Save the updated Controller Sensitivity X value to PlayerPrefs
        PlayerPrefs.SetFloat("Controller Sensitivity X", value);
    }

    // Method to save the Controller Sensitivity Y value when the slider value changes
    public void OnControllerSensitivityYSliderChanged(float value)
    {
        // Save the updated Controller Sensitivity Y value to PlayerPrefs
        PlayerPrefs.SetFloat("Controller Sensitivity Y", value);
    }
}
