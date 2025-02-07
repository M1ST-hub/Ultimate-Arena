using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerController pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideTimer;

    public float slideYScale;
    private float startYScale;

    public bool isSliding;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = pm.horizontalInput;
        verticalInput = pm.verticalInput;

        if (pm.sliding && (horizontalInput != 0 || verticalInput != 0) && isSliding == false)
            StartSlide();

        if (pm.sliding == false && isSliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (isSliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        isSliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // noraml slide
        if(!pm.OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            
        }


        //sliding on slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0)
            StopSlide();

        
    }

    private void StopSlide()
    {
        isSliding = false;
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
