using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class Settings : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI value;
    public string sliderName;
    public int defaultValue;
    private float sliderValue;
    private Toggle toggle;
    public CameraController camController;

    void Start()
    {
        slider = GetComponent<Slider>();
        value = GetComponentInChildren<TextMeshProUGUI>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                camController = player.GetComponentInChildren<CameraController>();
            }
        }

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

        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            // Load value from PlayerPrefs or use default
            sliderValue = PlayerPrefs.GetInt(sliderName, defaultValue);

            toggle.isOn = Convert.ToBoolean(sliderValue);
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

        // Apply settings immediately
        ApplySettings();
    }

    public void ApplySettings()
    {
        // Apply settings directly when slider values change
        if (sliderName == "Deadzone Right")
        {
            camController.SetRightDeadzone(slider.value);
        }
        else if (sliderName == "FOV")
        {
            camController.SetFOV(slider.value);
        }
        else if (sliderName == "Mouse Sensitivity X")
        {
            camController.SetMouseSensX(slider.value);
        }
        else if (sliderName == "Mouse Sensitivity Y")
        {
            camController.SetMouseSensY(slider.value);
        }
        else if (sliderName == "Controller Sensitivity X")
        {
            camController.SetControllerSensX(slider.value);
        }
        else if (sliderName == "Controller Sensitivity Y")
        {
            camController.SetControllerSensY(slider.value);
        }
    }

    // Methods for saving the settings on slider change
    public void OnDeadzoneSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("Deadzone Right", value);
    }

    public void OnFOVSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
    }

    public void OnMouseSensitivityXSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity X", value);
    }

    public void OnMouseSensitivityYSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity Y", value);
    }

    public void OnControllerSensitivityXSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("Controller Sensitivity X", value);
    }

    public void OnControllerSensitivityYSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("Controller Sensitivity Y", value);
    }
}
