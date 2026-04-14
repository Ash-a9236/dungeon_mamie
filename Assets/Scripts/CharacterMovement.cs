using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float airControlPercent = 0.6f;

    [Header("Jump Settings")]
    [SerializeField] private float gravity = -9.87f;
    [SerializeField] private float fallMultiplier = 2.2f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpHeightMin = 0.5f;
    [SerializeField] private float jumpHeightMax = 2f;
    [SerializeField] private float maxChargeTime = 0.2f;
    [SerializeField] private float doubleJumpBoost = 3f;
    [SerializeField] private float groundCheckDistance = 1.5f;

    [SerializeField] private float doubleJumpCooldown = 30f;
    private float energyTimer;
    private bool doubleJumpAvailable = true;

    public Image chargeBarFill;

    private Rigidbody rb;
    private Transform cameraTransform;
    private float moveX;
    private float moveZ;
    private Vector3 moveDirection;
    private bool isCharging;
    private float chargeTimer;
    private int jumpCount;
    private bool wasGroundedLastFrame;

    public enum MovementMode { TwoD, ThreeD }
    public MovementMode currentMode = MovementMode.TwoD;

    public bool IsGrounded => Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
    private bool IsRunning => Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (Camera.main)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        HandleModeToggle();
        HandleJumpInput();
        UpdateChargeUI();
        UpdateDoubleJumpEnergy();
    }

    private void FixedUpdate()
    {
        ApplyBetterGravity();
        HandleMovement();
    }

    private void HandleModeToggle()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentMode = currentMode == MovementMode.TwoD ? MovementMode.ThreeD : MovementMode.TwoD;
        }
    }

    private void HandleJumpInput()
    {
        bool grounded = IsGrounded;

        if (grounded && !wasGroundedLastFrame)
            jumpCount = 0;

        if (grounded && jumpCount == 0 && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (isCharging)
        {
            if (!grounded)
            {
                isCharging = false;
            }
            else
            {
                if (Input.GetButton("Jump") || Input.GetKey(KeyCode.UpArrow))
                {
                    chargeTimer += Time.deltaTime;
                    chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);
                }

                if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    PerformChargedJump();
                    isCharging = false;
                }
            }
        }

        if (!grounded && jumpCount == 1 && doubleJumpAvailable && rb.velocity.y < 0f &&
            (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            PerformDoubleJump();
        }

        wasGroundedLastFrame = grounded;
    }

    private void PerformChargedJump()
    {
        float percent = chargeTimer / maxChargeTime;
        float jumpHeight = Mathf.Lerp(jumpHeightMin, jumpHeightMax, percent);
        float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        jumpCount = 1;
    }

    private void PerformDoubleJump()
    {
        float newY = doubleJumpBoost;
        rb.velocity = new Vector3(rb.velocity.x, jumpHeightMin + newY, rb.velocity.z);

        jumpCount = 2;
        doubleJumpAvailable = false;
        energyTimer = 0f;
    }

    private void UpdateDoubleJumpEnergy()
    {
        if (doubleJumpAvailable) return;

        energyTimer += Time.deltaTime;

        if (energyTimer >= doubleJumpCooldown)
        {
            doubleJumpAvailable = true;
            energyTimer = 0f;
        }
    }

    public void RechargeDoubleJump()
    {
        doubleJumpAvailable = true;
        energyTimer = 0f;
    }

    private void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * gravity * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && !(Input.GetButton("Jump") || Input.GetKey(KeyCode.UpArrow)))
            rb.velocity += Vector3.up * gravity * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void HandleMovement()
    {
        CalculateMoveDirection();
        RotateCharacter();
        MoveCharacter();
    }

    private void CalculateMoveDirection()
    {
        if (!cameraTransform)
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        else
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }
    }

    private void RotateCharacter()
    {
        if (moveDirection.sqrMagnitude > 0.01f && currentMode == MovementMode.ThreeD)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void MoveCharacter()
    {
        float speed = IsRunning ? runSpeed : walkSpeed;
        float control = IsGrounded ? 1f : airControlPercent;

        Vector3 desiredVelocity = moveDirection * speed * control;

        if (currentMode == MovementMode.TwoD)
            rb.velocity = new Vector3(desiredVelocity.x, rb.velocity.y, 0f);
        else
            rb.velocity = new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z);
    }

    public void ApplySpeedBoost(float amount, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(amount, duration));
    }

    private System.Collections.IEnumerator SpeedBoostRoutine(float amount, float duration)
    {
        walkSpeed += amount;
        runSpeed += amount;

        yield return new WaitForSeconds(duration);

        walkSpeed -= amount;
        runSpeed -= amount;
    }

    public void ApplyJumpBoost(float amount, float duration)
    {
        StartCoroutine(JumpBoostRoutine(amount, duration));
    }

    private System.Collections.IEnumerator JumpBoostRoutine(float amount, float duration)
    {
        jumpHeightMax += amount;

        yield return new WaitForSeconds(duration);

        jumpHeightMax -= amount;
    }

    private void UpdateChargeUI()
    {
        if (!chargeBarFill) return;
        chargeBarFill.fillAmount = isCharging ? chargeTimer / maxChargeTime : 0f;
    }
}