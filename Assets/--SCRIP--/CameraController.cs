using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : NetworkBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    public float vertLook;
    public float horizLook;

    private Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
           
           mainCamera.enabled = false;
           GetComponent<AudioListener>().enabled = false;
        }

        mainCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 80f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float mouseX = horizLook * Time.deltaTime * sensX;
            float mouseY = vertLook * Time.deltaTime * sensY;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //can rotate
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void OnLook(InputValue context)
    {
        horizLook = context.Get<Vector2>().x;
        vertLook = context.Get<Vector2>().y;
    }

}
