using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public GameObject scoreboard;
    private GameObject pause;
    public GameObject itArrow;
    private Animator anim;
    public CapsuleCollider capCol;
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer head;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float climbSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Tagging")]
    public float tagCooldown;
    bool isTagger;
    bool readyToTag;
    bool inRange;


    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode tagKey = KeyCode.Mouse0;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        climbing,
        crouching,
        sliding,
        air
    }

    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            capCol = GetComponentInChildren<CapsuleCollider>();
            rb.freezeRotation = true;
            readyToJump = true;

            if (IsLocalPlayer)
                body.enabled = false;
            head.enabled = false;

            startYScale = transform.localScale.y;
            scoreboard = GameObject.FindWithTag("scoreboard");
            pause = GameObject.FindWithTag("Pause");

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //RaycastHit ray;
        //Physics.Raycast(transform.position, Vector3.down, out ray,  playerHeight * 0.3f + 0.2f, whatIsGround);
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - playerHeight * 0.005f + 0.2f, transform.position.z));
    }

    private void Update()
    {
        if (IsOwner)
        {
            //ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.0005f + 0.2f, whatIsGround);

            MyInput();

            //handle drag
            if (grounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;

            if (itArrow.activeInHierarchy == true)
                isTagger = true;
            else
                isTagger = false;
        }

    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            MovePlayer();
            SpeedControl();
            StateHandler();
        }

    }

    // Update is called once per frame
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        //start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            capCol.height = 1.3f;
            capCol.center = new Vector3(0, -.3f, 0);
            //transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            anim.SetBool("isCrouching", true);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            anim.SetBool("isCrouching", false);

            capCol.height = 2f;
            capCol.center = new Vector3(0, 0, 0);
        }

        //scoreboard
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.transform.GetChild(0).gameObject.SetActive(false);
        }

        //pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.transform.GetChild(0).gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
        }

    }

    private void StateHandler()
    {
        if (IsOwner)
        {
            //Mode - Climbing
            if (climbing)
            {
                state = MovementState.climbing;
                desiredMoveSpeed = climbSpeed;
            }

            // Mode - Wallrunning
            else if (wallrunning)
            {
                state = MovementState.wallrunning;
                desiredMoveSpeed = wallrunSpeed;
            }

            // Mode - Sliding
            else if (sliding)
            {
                state = MovementState.sliding;

                if (OnSlope() && rb.linearVelocity.y < 0.1f)
                    desiredMoveSpeed = slideSpeed;

                else
                    desiredMoveSpeed = sprintSpeed;
            }

            // Mode - Crouching
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                desiredMoveSpeed = crouchSpeed;
            }


            //Mode - Sprinting
            if (grounded && Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
            }

            else if (grounded)
            {
                state = MovementState.walking;
                desiredMoveSpeed = walkSpeed;
            }

            //Mode - Air
            else
            {
                state = MovementState.air;
            }

            //check if desiredMoveSpeed has changed drastically
            if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 8f && moveSpeed != 0)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;

        }
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movement speed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 15f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //no gravity whilst on slope
        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope())
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        //limiting speed on ground
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }



    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(tagKey) && readyToTag && isTagger)
        {
            readyToTag = false;

            if (GameManager.Instance.players.Contains(other.gameObject))
            {
                TagRpc(GameManager.Instance.players.IndexOf(other.gameObject));
            }

            Invoke(nameof(ResetTag), tagCooldown);
            Debug.Log("Tagged");
        }
        Debug.Log("INrANGE");   
    }

    [Rpc(SendTo.Everyone)]
    private void TagRpc(int taggedPlayer)
    {
        itArrow.SetActive(false);
        GameManager.Instance.players[taggedPlayer].GetComponent<PlayerController>().itArrow.SetActive(true);
        Debug.Log("Tagged");
    }

    private void ResetTag()
    {
        readyToTag = true;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

}
