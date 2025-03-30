using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandler : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect component

    // This method will be called when the scrollbar's value changes
    public void OnScrollbarValueChanged(float value)
    {
        Debug.Log("Scrollbar Value Changed: " + value);

        // Here, you can use the value to change the position of the content in the ScrollRect
        // For example:
        scrollRect.verticalNormalizedPosition = value;  // Adjust the position of content based on scrollbar value
    }
}
