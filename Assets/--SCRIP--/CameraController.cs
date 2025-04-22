using DG.Tweening;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    public float mouseSensX;
    public float mouseSensY;
    public float controllerSensX;
    public float controllerSensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    public float vertLook;
    public float horizLook;

    private Camera mainCamera;

    private float rightDeadzone = 0.2f;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mainCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 80f);
            rightDeadzone = PlayerPrefs.GetFloat("Deadzone Right", 0.2f);
            LoadSensitivitySettings();
            StartCoroutine(DelayedInit());
        }
        else
        {
            mainCamera.enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }

        // Load the initial settings for FOV and sensitivities
        

        // Load initial sensitivity values from PlayerPrefs
        
        Debug.Log($"mouseY {mouseSensY}, mouseX {mouseSensX},contY {controllerSensY},contX {controllerSensX}");
        Debug.Log($"deadzone {rightDeadzone}");
    }

    void Update()
    {
        // Re-load the sensitivity values from PlayerPrefs in case they were changed during gameplay (from the UI)
        LoadSensitivitySettings();

        if (!IsOwner) return;

        // Handle mouse and controller inputs for camera rotation
        float mouseX = horizLook * Time.deltaTime * mouseSensX;
        float mouseY = vertLook * Time.deltaTime * mouseSensY;

        float controllerX = horizLook * Time.deltaTime * controllerSensX;
        float controllerY = vertLook * Time.deltaTime * controllerSensY;

        // Combine both mouse and controller inputs
        float xRotationInput = (Gamepad.current == null) ? mouseX : controllerX;
        float yRotationInput = (Gamepad.current == null) ? mouseY : controllerY;

        yRotation += xRotationInput;
        xRotation -= yRotationInput;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        float loadedFov = PlayerPrefs.GetFloat("FOV", 80f);

        // Clamp to a valid range
        if (loadedFov < 30f || loadedFov > 120f)
            loadedFov = 80f;

        mainCamera.fieldOfView = loadedFov;

        LoadSensitivitySettings();
    }

    public void OnLook(InputValue context)
    {
        Vector2 rawInput = context.Get<Vector2>();
        Vector2 filteredInput = ApplyDeadzone(rawInput, rightDeadzone);

        horizLook = filteredInput.x;
        vertLook = filteredInput.y;
    }

    private Vector2 ApplyDeadzone(Vector2 input, float threshold)
    {
        Debug.Log($"magnitude{input.magnitude}");
        return input.magnitude < threshold ? Vector2.zero : input;
    }

    public void DoFov(float endValue)
    {
        mainCamera.DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    // Load the sensitivity values from PlayerPrefs
    private void LoadSensitivitySettings()
    {
        mouseSensX = PlayerPrefs.GetFloat("Mouse Sensitivity X", 5f);
        mouseSensY = PlayerPrefs.GetFloat("Mouse Sensitivity Y", 5f);
        controllerSensX = PlayerPrefs.GetFloat("Controller Sensitivity X", 10f);
        controllerSensY = PlayerPrefs.GetFloat("Controller Sensitivity Y", 10f);
    }
}
