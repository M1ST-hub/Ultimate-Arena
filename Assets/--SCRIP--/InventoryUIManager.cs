using UnityEngine;
using UnityEngine.InputSystem;

public class StickDebug : MonoBehaviour
{
    public InputActionReference leftStickRef;
    public InputActionReference rightStickRef;

    void OnEnable()
    {
        leftStickRef.action.Enable();
        rightStickRef.action.Enable();
    }

    void Update()
    {
        Vector2 left = leftStickRef.action.ReadValue<Vector2>();
        Vector2 right = rightStickRef.action.ReadValue<Vector2>();

        Debug.Log($"Left Stick: {left} | Right Stick: {right}");
    }
}