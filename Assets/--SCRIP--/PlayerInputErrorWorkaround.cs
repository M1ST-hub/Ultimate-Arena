using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputErrorWorkaround : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputDevice lastDeviceUsed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        // Initial device assignment
        StartCoroutine(DelayedDeviceAssign());

        // Listen for device connection/disconnection
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private IEnumerator DelayedDeviceAssign()
    {
        // Wait until PlayerInput's user is initialized
        while (playerInput.user == default || !playerInput.user.valid)
            yield return null;

        AssignInitialDevice(); // Your method to pick Gamepad or Keyboard
    }

    private void AssignInitialDevice()
    {
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput is null. Cannot assign initial device.");
            return;
        }

        if (Gamepad.current != null)
        {
            SwitchToGamepad(Gamepad.current);
        }
        else if (Keyboard.current != null && Mouse.current != null)
        {
            SwitchToKeyboardMouse();
        }
        else
        {
            Debug.LogWarning("No input devices detected.");
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed)
        {
            if (device == lastDeviceUsed)
            {
                Debug.Log($"Device {device.displayName} disconnected.");

                // Fallback to Keyboard+Mouse
                if (Keyboard.current != null && Mouse.current != null && playerInput.user.valid)
                {
                    SwitchToKeyboardMouse();
                }
            }
        }
        else if (change == InputDeviceChange.Reconnected || change == InputDeviceChange.Added)
        {
            // Only switch to gamepad if not already using one
            if (device is Gamepad && lastDeviceUsed is not Gamepad && playerInput.user.valid)
            {
                Debug.Log($"Gamepad {device.displayName} reconnected.");
                SwitchToGamepad(device as Gamepad);
            }
        }
    }

    private void SwitchToGamepad(Gamepad gamepad)
    {
        playerInput.SwitchCurrentControlScheme("Gamepad");
        lastDeviceUsed = gamepad;
        Debug.Log("Switched to Gamepad.");
    }

    private void SwitchToKeyboardMouse()
    {
        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse");
        lastDeviceUsed = Keyboard.current;
        Debug.Log("Switched to Keyboard + Mouse.");
    }

    private void OnDisable()
    {
        playerInput.actions = null;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
