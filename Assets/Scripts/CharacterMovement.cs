using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float airControl = 0.65f;
    [SerializeField] private float rotationSpeed = 14f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float chargedJumpForce = 14f;
    [SerializeField] private float doubleJumpForce = 10f;

    [Header("Charge Jump")]
    [SerializeField] private float maxChargeTime = 0.5f;

    [Header("Gravity")]
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 3f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 18f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundDistance = 0.8f;
    [SerializeField] private LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    [Header("UI")]
    public Image chargeBarFill;

    private Rigidbody rb;
    private Transform cam;

    private Vector3 moveDirection;

    private bool isGrounded;
    private bool canDoubleJump;

    private bool isCharging;
    private float chargeTimer;

    private bool isDashing;
    private bool canDash = true;

    private bool isBlocking;

    private float moveX;
    private float moveZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode =
            CollisionDetectionMode.Continuous;

        if (Camera.main)
        {
            cam = Camera.main.transform;
        }
    }

    void Update()
    {
        CheckGround();

        HandleInput();

        HandleJumpInput();

        HandleDashInput();

        HandleBlockInput();

        UpdateAnimations();

        UpdateChargeUI();
    }

    void FixedUpdate()
    {
        ApplyBetterGravity();

        if (!isDashing && !isBlocking)
        {
            HandleMovement();
        }
    }

    void HandleInput()
    {
        moveX = 0f;
        moveZ = 0f;

        if (Input.GetKey(KeyCode.W))
            moveZ += 1f;

        if (Input.GetKey(KeyCode.S))
            moveZ -= 1f;

        if (Input.GetKey(KeyCode.A))
            moveX -= 1f;

        if (Input.GetKey(KeyCode.D))
            moveX += 1f;

        if (!cam && Camera.main)
        {
            cam = Camera.main.transform;
        }

        if (!cam)
        {
            moveDirection =
                new Vector3(moveX, 0, moveZ).normalized;

            return;
        }

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection =
            (forward * moveZ + right * moveX).normalized;
    }

    void HandleMovement()
    {
        float speed = walkSpeed;

        if (
            Input.GetKey(KeyCode.LeftShift) &&
            moveDirection.sqrMagnitude > 0.01f
        )
        {
            speed = sprintSpeed;
        }

        Vector3 targetVelocity =
            moveDirection * speed;

        Vector3 currentVelocity =
            new Vector3(
                rb.velocity.x,
                0f,
                rb.velocity.z
            );

        float control =
            isGrounded
            ? 1f
            : airControl;

        Vector3 smoothedVelocity =
            Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                acceleration *
                control *
                Time.fixedDeltaTime
            );

        rb.velocity = new Vector3(
            smoothedVelocity.x,
            rb.velocity.y,
            smoothedVelocity.z
        );

        RotateCharacter();
    }

    void RotateCharacter()
    {
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        rb.rotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    void HandleJumpInput()
    {
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump(jumpForce);

                isCharging = true;
                chargeTimer = 0f;
            }
            else if (canDoubleJump)
            {
                DoubleJump();
            }
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            chargeTimer += Time.deltaTime;

            chargeTimer = Mathf.Clamp(
                chargeTimer,
                0f,
                maxChargeTime
            );

            float percent =
                chargeTimer / maxChargeTime;

            float extraForce =
                Mathf.Lerp(
                    0f,
                    chargedJumpForce - jumpForce,
                    percent
                );

            rb.velocity = new Vector3(
                rb.velocity.x,
                jumpForce + extraForce,
                rb.velocity.z
            );
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCharging = false;
        }
    }
    void Jump(float force)
    {
        rb.velocity = new Vector3(
            rb.velocity.x,
            0f,
            rb.velocity.z
        );

        rb.AddForce(
            Vector3.up * force,
            ForceMode.Impulse
        );

        isGrounded = false;

        if (animator)
        {
            animator.SetTrigger("Jump");
        }
    }
    void DoubleJump()
    {
        rb.velocity = new Vector3(
            rb.velocity.x,
            0f,
            rb.velocity.z
        );

        rb.AddForce(
            Vector3.up * doubleJumpForce,
            ForceMode.Impulse
        );

        canDoubleJump = false;

        if (animator)
        {
            animator.SetTrigger("DoubleJump");
        }
    }

    void HandleDashInput()
    {
        if (
            Input.GetKeyDown(KeyCode.G) &&
            canDash &&
            !isDashing &&
            !isBlocking
        )
        {
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        if (animator)
        {
            animator.SetTrigger("Dash");
        }

        Vector3 dashDirection =
            moveDirection.sqrMagnitude > 0.01f
            ? moveDirection
            : transform.forward;

        rb.velocity = new Vector3(
            0f,
            rb.velocity.y,
            0f
        );

        rb.AddForce(
            dashDirection * dashForce,
            ForceMode.Impulse
        );

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void HandleBlockInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBlocking = true;

            rb.velocity = Vector3.zero;

            if (animator)
            {
                animator.SetBool("Blocking", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            isBlocking = false;

            if (animator)
            {
                animator.SetBool("Blocking", false);
            }
        }
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity +=
                Vector3.up *
                Physics.gravity.y *
                (fallMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
        else if (
            rb.velocity.y > 0 &&
            !Input.GetKey(KeyCode.Space)
        )
        {
            rb.velocity +=
                Vector3.up *
                Physics.gravity.y *
                (gravityMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
    }
    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            Vector3.down,
            groundDistance,
            groundMask
        );
    }

    public void ApplySpeedBoost(
        float amount,
        float duration
    )
    {
        StartCoroutine(
            SpeedBoostRoutine(amount, duration)
        );
    }

    IEnumerator SpeedBoostRoutine(
        float amount,
        float duration
    )
    {
        walkSpeed += amount;
        sprintSpeed += amount;

        yield return new WaitForSeconds(duration);

        walkSpeed -= amount;
        sprintSpeed -= amount;
    }

    public void ApplyJumpBoost(
        float amount,
        float duration
    )
    {
        StartCoroutine(
            JumpBoostRoutine(amount, duration)
        );
    }

    IEnumerator JumpBoostRoutine(
        float amount,
        float duration
    )
    {
        jumpForce += amount;
        chargedJumpForce += amount;
        doubleJumpForce += amount;

        yield return new WaitForSeconds(duration);

        jumpForce -= amount;
        chargedJumpForce -= amount;
        doubleJumpForce -= amount;
    }

    public void RechargeDoubleJump()
    {
        canDoubleJump = true;
    }

    void UpdateAnimations()
    {
        if (!animator)
            return;

        Vector3 horizontalVelocity =
            new Vector3(
                rb.velocity.x,
                0f,
                rb.velocity.z
            );

        animator.SetFloat(
            "Speed",
            horizontalVelocity.magnitude
        );

        animator.SetBool(
            "Grounded",
            isGrounded
        );

        animator.SetFloat(
            "VerticalSpeed",
            rb.velocity.y
        );
    }

    void UpdateChargeUI()
    {
        if (!chargeBarFill)
            return;

        if (isCharging)
        {
            chargeBarFill.fillAmount =
                chargeTimer / maxChargeTime;
        }
        else
        {
            chargeBarFill.fillAmount = 0f;
        }
    }
}