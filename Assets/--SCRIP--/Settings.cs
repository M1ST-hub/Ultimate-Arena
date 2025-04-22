using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Settings : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI value;
    public string sliderName;
    public int defaultValue;
    private float sliderValue;
    private Toggle toggle;

    void Start()
    {
        slider = GetComponent<Slider>();
        value = GetComponentInChildren<TextMeshProUGUI>();

        if (slider != null && value != null)
        {
            // Load value from PlayerPrefs or use default
            sliderValue = PlayerPrefs.GetFloat(sliderName, defaultValue);

            slider.value = sliderValue;

            // Update display text
            UpdateText(sliderValue);

            // Add listener
            slider.onValueChanged.AddListener(UpdateText);
        }

        if (toggle != null && value != null)
        {
            // Load value from PlayerPrefs or use default
            sliderValue = PlayerPrefs.GetInt(sliderName, defaultValue);

            toggle.isOn = Convert.ToBoolean(sliderValue);

            // Update display text
            UpdateText(sliderValue);

            // Add listener
            //toggle.onValueChanged.AddListener(UpdateText);
        }
    }

    // Method to update the text label when the slider changes
    void UpdateText(float value)
    {
        // Update the displayText string based on the slider value
        this.value.text = value.ToString("F0");

        if (sliderName == "Deadzone Right")
        {
            this.value.text = value.ToString("F2");
        }
    }

    public void AutoSprintToggle(bool toggle)
    {
        PlayerPrefs.SetInt("AutoSprint", toggle ? 1 : 0);
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
