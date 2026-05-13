using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float doubleJumpForce = 7f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 18f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundDistance = 1.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private bool billboardSprite = true;
    [SerializeField] private bool flipSprite = true;

    [Header("Boost Settings")]
    [SerializeField] private float speedBoostDuration = 5f;
    [SerializeField] private float jumpBoostDuration = 5f;

    [Header("Animation")]
    public Animator animator;

    [Header("UI")]
    public Image chargeBarFill;

    private Rigidbody rb;
    private Transform cam;

    private Vector3 moveDirection;

    private bool isGrounded;
    private bool canDoubleJump;

    private bool isDashing;
    private bool canDash = true;

    private float moveX;
    private float moveZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        cam = Camera.main ? Camera.main.transform : null;
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleJump();
        HandleDash();
        HandleVisuals();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            HandleMovement();

        ApplyBetterGravity();
    }

    void HandleInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        if (!cam && Camera.main)
            cam = Camera.main.transform;

        if (!cam)
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
            return;
        }

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveZ + right * moveX).normalized;
    }

    void HandleMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        Vector3 targetVelocity = moveDirection * speed;

        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        float control = isGrounded ? 1f : airControl;

        Vector3 smoothed = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            acceleration * control * Time.fixedDeltaTime
        );

        rb.velocity = new Vector3(smoothed.x, rb.velocity.y, smoothed.z);
    }

    void HandleJump()
    {
        if (isGrounded)
            canDoubleJump = true;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump(jumpForce);
            }
            else if (canDoubleJump)
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
            }
        }
    }

    void Jump(float force)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * force, ForceMode.Impulse);

        if (animator)
            animator.SetTrigger("Jump");
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.G) && canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        Vector3 dir = moveDirection.sqrMagnitude > 0.01f
            ? moveDirection
            : transform.forward;

        rb.velocity = Vector3.zero;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * 2f * Time.fixedDeltaTime;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundDistance,
            groundMask
        );
    }

    void HandleVisuals()
    {
        if (!visual) return;

        if (billboardSprite && cam)
            visual.LookAt(visual.position + cam.forward);

        if (flipSprite)
        {
            if (moveX > 0.1f)
                visual.localScale = new Vector3(Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
            else if (moveX < -0.1f)
                visual.localScale = new Vector3(-Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
        }
    }

    void UpdateAnimations()
    {
        if (!animator) return;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        animator.SetFloat("Speed", flatVel.magnitude);
        animator.SetBool("Grounded", isGrounded);
    }

    public void ApplySpeedBoost(float amount, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(amount, duration));
    }

    IEnumerator SpeedBoostRoutine(float amount, float duration)
    {
        walkSpeed += amount;
        sprintSpeed += amount;

        yield return new WaitForSeconds(duration);

        walkSpeed -= amount;
        sprintSpeed -= amount;
    }

    public void ApplyJumpBoost(float amount, float duration)
    {
        StartCoroutine(JumpBoostRoutine(amount, duration));
    }

    IEnumerator JumpBoostRoutine(float amount, float duration)
    {
        jumpForce += amount;
        doubleJumpForce += amount;

        yield return new WaitForSeconds(duration);

        jumpForce -= amount;
        doubleJumpForce -= amount;
    }

    public void RechargeDoubleJump()
    {
        canDoubleJump = true;
    }
}