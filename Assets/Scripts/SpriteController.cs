using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool billboard = true;
    [SerializeField] private bool flipSprite = true;

    [Header("References")]
    [SerializeField] private Rigidbody targetRb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Sprites")]
    [SerializeField] private Sprite[] backSprites;
    [SerializeField] private Sprite[] frontSprites;

    private Camera cam;
    private bool lastFrontState;

    void Start()
    {
        cam = Camera.main;

        if (!targetRb)
            targetRb = GetComponentInParent<Rigidbody>();

        if (!animator)
            animator = GetComponent<Animator>();

        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (!cameraFollow)
            cameraFollow = FindObjectOfType<CameraFollow>();

        if (cameraFollow)
            lastFrontState = cameraFollow.faceFront;
    }

    void LateUpdate()
    {
        HandleBillboard();
        HandleFlip();
        HandleSpriteSwap();
        HandleAnimations();
    }

    void HandleBillboard()
    {
        if (!billboard || !cam)
            return;

        transform.forward = cam.transform.forward;
    }

    void HandleFlip()
    {
        if (!flipSprite || !targetRb || !cam)
            return;

        Vector3 velocity = targetRb.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 localVelocity =
            cam.transform.InverseTransformDirection(velocity);

        Vector3 scale = transform.localScale;

        if (localVelocity.x > 0.05f)
            scale.x = Mathf.Abs(scale.x);
        else if (localVelocity.x < -0.05f)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void HandleSpriteSwap()
    {
        if (!cameraFollow || !spriteRenderer || !cam || !targetRb)
            return;

        Vector3 velocity = targetRb.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        float forwardDot =
            Vector3.Dot(velocity.normalized, cam.transform.forward);

        bool currentFront = forwardDot > 0f;

        if (currentFront == lastFrontState)
            return;

        lastFrontState = currentFront;

        Sprite currentSprite = spriteRenderer.sprite;

        int index = FindSpriteIndex(currentSprite);
        if (index < 0) index = 0;

        if (currentFront)
        {
            if (index < frontSprites.Length)
                spriteRenderer.sprite = frontSprites[index];
        }
        else
        {
            if (index < backSprites.Length)
                spriteRenderer.sprite = backSprites[index];
        }
    }

    int FindSpriteIndex(Sprite sprite)
    {
        for (int i = 0; i < backSprites.Length; i++)
            if (backSprites[i] == sprite)
                return i;

        for (int i = 0; i < frontSprites.Length; i++)
            if (frontSprites[i] == sprite)
                return i;

        return -1;
    }

    void HandleAnimations()
    {
        if (!animator || !targetRb)
            return;

        Vector3 velocity = targetRb.velocity;
        velocity.y = 0f;

        float speed = velocity.magnitude;

        bool moving = speed > 0.1f;

        animator.SetBool("IsMoving", moving);
        animator.SetFloat("Speed", speed);

        bool isRunning = speed > 6f;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsWalking", moving && !isRunning);
        animator.SetBool("IsIdle", !moving);

        PlayerController player =
            GetComponentInParent<PlayerController>();

        if (player)
        {
            animator.SetBool("IsDashing", player.isDashing);
        }
    }
}