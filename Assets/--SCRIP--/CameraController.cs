using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    public CameraController Instance { get; private set; }

    public float mouseSensX;
    public float mouseSensY;
    public float controllerSensX;
    public float controllerSensY;

    public Transform orientation;
    public Transform camHolder;

    private Camera mainCamera;

    private float rightDeadzone = 0.2f;

    private float xRotation;
    private float yRotation;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mainCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 80f);
            rightDeadzone = PlayerPrefs.GetFloat("Deadzone Right", 0.2f);

            // Apply saved settings for sensitivities and deadzone
            ApplySavedSettings();
        }
        else
        {
            mainCamera.enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        

#if (UNITY_IOS || UNITY_ANDROID)
        float xRotationInput = horizontalInput * controllerSensX * Time.deltaTime;
        float yRotationInput = verticalInput * controllerSensY * Time.deltaTime;
#else

        // Handle mouse and controller inputs for camera rotation
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensX * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensY * Time.deltaTime;

        float controllerX = (Gamepad.current == null) ? 0 : Gamepad.current.rightStick.x.ReadValue() * controllerSensX * Time.deltaTime;
        float controllerY = (Gamepad.current == null) ? 0 : Gamepad.current.rightStick.y.ReadValue() * controllerSensY * Time.deltaTime;
        // Combine both mouse and controller inputs for camera rotation
        float xRotationInput = (Gamepad.current == null) ? mouseX : controllerX;
        float yRotationInput = (Gamepad.current == null) ? mouseY : controllerY;
#endif

        // Apply rotation limits
        xRotation -= yRotationInput;
        yRotation += xRotationInput;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.root.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    // Update methods to set values dynamically
    public void SetRightDeadzone(float value)
    {
        rightDeadzone = value;
    }

    public void SetFOV(float value)
    {
        mainCamera.fieldOfView = value;
    }

    public void SetMouseSensX(float value)
    {
        mouseSensX = value;
    }

    public void SetMouseSensY(float value)
    {
        mouseSensY = value;
    }

    public void SetControllerSensX(float value)
    {
        controllerSensX = value;
    }

    public void SetControllerSensY(float value)
    {
        controllerSensY = value;
    }

    // Apply the saved settings from PlayerPrefs
    private void ApplySavedSettings()
    {
        mouseSensX = PlayerPrefs.GetFloat("Mouse Sensitivity X", 5f);
        mouseSensY = PlayerPrefs.GetFloat("Mouse Sensitivity Y", 5f);
        controllerSensX = PlayerPrefs.GetFloat("Controller Sensitivity X", 10f);
        controllerSensY = PlayerPrefs.GetFloat("Controller Sensitivity Y", 10f);
        rightDeadzone = PlayerPrefs.GetFloat("Deadzone Right", 0.2f);
    }

    // Method to apply deadzone for controller
    private Vector2 ApplyDeadzone(Vector2 input, float threshold)
    {
        return input.magnitude < threshold ? Vector2.zero : input;
    }

    // Method to apply Field of View (FOV) transition
    public void DoFov(float endValue)
    {
        mainCamera.DOFieldOfView(endValue, 0.25f);
    }

    // Method to apply camera tilt transition
    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void OnLook(InputValue context)
    {
        horizontalInput = context.Get<Vector2>().x;
        verticalInput = context.Get<Vector2>().y;
    }
}
