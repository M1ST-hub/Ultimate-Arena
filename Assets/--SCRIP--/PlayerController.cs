using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public GameObject scoreboard;
    private GameObject pause;
    public GameObject itArrow;
    private Animator anim;
    public CapsuleCollider capCol;
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer head;
    public Scoring scoreManager;

    public float taggerExp = 5f;
    public float surviveExp = 8f;
    private float untaggedMult = 0f;  // Time the player has been untagged
    public float untaggedTime = 0f;
    public float taggedTime = 0f;
    private float multiplier = 1f;  // Multiplier starts at 1
    private float maxMultiplier = 5f;  // Maximum multiplier cap
    public int mostTags = 0;

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

    public float horizontalInput;
    public float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public PlayerInput playerInput;

    public bool isPaused;

    public PauseManager pauseManager;

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
    public bool tagging;
    public bool jumping;
    public bool sprinting;
    public bool interacting;
    public bool scoreboarding;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            capCol = GetComponentInChildren<CapsuleCollider>();
            playerInput = GetComponent<PlayerInput>();
            pauseManager = GameObject.Find("Pause Manager").GetComponent<PauseManager>();
            rb.freezeRotation = true;
            readyToJump = true;
            readyToTag = true;

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

            if (GameManager.Instance.isGameStarted && TheItArrow.Instance != null && TheItArrow.Instance.transform.parent == transform)
                isTagger = true;
            else
                isTagger = false;

            if (pauseManager.isPaused == false && playerInput.currentActionMap.name != "Player")
            {
                playerInput.SwitchCurrentActionMap("Player");
            }

            if (isTagger == true)
            {
                taggedTime += Time.deltaTime;

                // Calculate the experience gained as 2/3 of the tagged time
                int experienceGained = (int)((taggedTime * (5f / 6f)) * taggerExp * Time.deltaTime);

                // Add the experience gained to the GameManager's total experience
                GameManager.Instance.gainedExperience += experienceGained;

                // Debugging line to check the XP gained and tagged time
                //Debug.Log($"XP Gained (Tagger): {experienceGained}, Tagged Time: {taggedTime}");
            }

            if (isTagger == false)
            {
                untaggedMult += Time.deltaTime;
                untaggedTime += Time.deltaTime;

                // Increase the multiplier based on how long the player has been untagged
                multiplier = Mathf.Min(1f + (untaggedMult / 10f), maxMultiplier);

                // Calculate the experience gained (base XP * multiplier * time)
                int experienceGained = (int)(surviveExp * multiplier * Time.deltaTime);
                GameManager.Instance.gainedExperience += experienceGained;

                // Debugging line to check the XP and multiplier
                //Debug.Log($"XP Gained: {experienceGained}, Multiplier: {multiplier}");
            }
            else
            {
                // Reset untagged time when the player is tagged
                untaggedMult = 0f;
                multiplier = 1f;  // Reset multiplier when tagged
            }
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
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        //verticalInput = Input.GetAxisRaw("Vertical");

        if (jumping && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        //start crouch
        if (crouching)
        {
            capCol.height = 1.3f;
            capCol.center = new Vector3(0, -.3f, 0);
            //transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            anim.SetBool("isCrouching", true);
        }

        // stop crouch
        if (crouching == false)
        {
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            anim.SetBool("isCrouching", false);

            capCol.height = 2f;
            capCol.center = new Vector3(0, 0, 0);
        }

        //scoreboard
        if (scoreboarding)
        {
            scoreboard.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (scoreboarding == false)
        {
            scoreboard.transform.GetChild(0).gameObject.SetActive(false);
        }


    }

    public void Pause()
    {
        pauseManager.isPaused = !pauseManager.isPaused;
        if (pauseManager.isPaused)
        {
            pause.transform.GetChild(0).gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;

            EventSystem.current.SetSelectedGameObject(pauseManager.resumeButton);

            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;

            pause.transform.GetChild(0).gameObject.SetActive(false);

            playerInput.SwitchCurrentActionMap("Player");
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
            else if (crouching)
            {
                state = MovementState.crouching;
                desiredMoveSpeed = crouchSpeed;
            }


            //Mode - Sprinting
            if (grounded && sprinting)
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
        if (other.GetType() == typeof(CapsuleCollider) && other.CompareTag("Player") && tagging && readyToTag && isTagger)
        {
            readyToTag = false;

            if (GameManager.Instance.players.Contains(other.transform.parent.gameObject))
            {
                TagRpc(GameManager.Instance.players.IndexOf(other.transform.parent.gameObject));
            }

            GameManager.Instance.gainedExperience += 30;

            //string taggedPlayerName = other.gameObject.name;
            //scoreManager.IncrementScore(taggedPlayerName);

            Invoke(nameof(ResetTag), tagCooldown);
            Debug.Log("Tagged");
        }

    }

    [Rpc(SendTo.Everyone)]
    private void TagRpc(int taggedPlayer)
    {
        if (IsServer)
        {
            TheItArrow.Instance.transform.SetParent(GameManager.Instance.players[taggedPlayer].transform);
            Debug.Log("Switched Arrow");
        }
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

    public void OnMove(InputValue context)
    {
        horizontalInput = context.Get<Vector2>().x;
        verticalInput = context.Get<Vector2>().y;
    }

    public void OnJump(InputValue context)
    {
        jumping = context.isPressed;
    }

    public void OnSprint(InputValue context)
    {
        sprinting = context.isPressed;
    }

    public void OnCrouch(InputValue context)
    {
        crouching = context.isPressed;
    }

    public void OnTag(InputValue context)
    {
        tagging = context.isPressed;
    }

    public void OnInteract(InputValue context)
    {
        interacting = context.isPressed;
    }

    public void OnSlide(InputValue context)
    {
        sliding = context.isPressed;
    }

    public void OnScoreboard(InputValue context)
    {
        scoreboarding = context.isPressed;
    }

    public void OnPause(InputValue context)
    {
        Pause();
    }
}
