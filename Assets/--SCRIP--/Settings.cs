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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        value = GetComponentInChildren<TextMeshProUGUI>();

        if (slider != null && value != null)
        {
            // Set the initial text value based on the current slider value
            UpdateText(slider.value);

            // Add listener to update the text when the slider value changes
            slider.onValueChanged.AddListener(UpdateText);
        }

        sliderValue = PlayerPrefs.GetFloat(sliderName, defaultValue); //default fov
        slider.value = sliderValue;

    }
    public void SetFOVValue(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateText(float value)
    {
        // Update the displayText string based on the slider value
        this.value.text = value.ToString("F0");
    }
}
