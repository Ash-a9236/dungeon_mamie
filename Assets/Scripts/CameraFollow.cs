using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Optional Player Sync")]
    public CharacterMovement player;

    [Header("Camera Distance")]
    public float distance = 4f;
    public float height = 4f;

    [Header("Camera Angle")]
    [Range(10f, 60f)]
    public float baseAngle = 30f;

    [Header("Smoothness")]
    public float moveSmooth = 8f;
    public float rotationSmooth = 12f;

    [Header("View Rotation")]
    public float currentAngle = 0f;
    public float targetAngle = 0f;
    public float rotateAmount = 90f;
    public float fineRotateAmount = 9f;

    [Header("View Mode")]
    public bool faceFront = false;

    [Header("Zoom")]
    public float minFov = 40f;
    public float maxFov = 80f;
    public float zoomSpeed = 10f;
    public float zoomLerpSpeed = 6f;

    private Vector3 currentVelocity;
    private float targetFov;

    private bool controllerConnected;

    void Start()
    {
        if (Camera.main)
            targetFov = Camera.main.fieldOfView;

        CheckController();
    }

    void Update()
    {
        HandleViewToggle();
        HandleRotationInput();
        HandleZoomInput();
        CheckController();
    }

    void LateUpdate()
    {
        if (!target)
            return;

        UpdateCameraPosition();
    }

    void HandleViewToggle()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            faceFront = !faceFront;
        }
    }

    void HandleRotationInput()
    {
        bool fineMode = Input.GetKey(KeyCode.RightShift);

        float step = fineMode ? fineRotateAmount : rotateAmount;

        if (!fineMode)
        {
            if (Input.GetKeyDown(KeyCode.P))
                targetAngle += rotateAmount;

            if (Input.GetKeyDown(KeyCode.I))
                targetAngle -= rotateAmount;

            currentAngle = targetAngle;
        }
        else
        {
            if (Input.GetKey(KeyCode.P))
                targetAngle += step * Time.deltaTime * 10f;

            if (Input.GetKey(KeyCode.I))
                targetAngle -= step * Time.deltaTime * 10f;

            currentAngle = Mathf.LerpAngle(
                currentAngle,
                targetAngle,
                Time.deltaTime * 6f
            );
        }
    }

    void UpdateCameraPosition()
    {
        float directionMultiplier = faceFront ? 1f : -1f;

        Quaternion rotation =
            Quaternion.Euler(baseAngle, currentAngle, 0f);

        Vector3 forward =
            rotation * Vector3.forward * directionMultiplier;

        forward.y = 0f;
        forward.Normalize();

        Vector3 desiredPosition =
            target.position
            - forward * distance
            + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / moveSmooth
        );

        Quaternion lookRotation =
            Quaternion.LookRotation(
                target.position - transform.position
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSmooth
        );

        if (player && !faceFront)
        {
            Vector3 flatForward = forward;
            flatForward.y = 0f;

            if (flatForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot =
                    Quaternion.LookRotation(flatForward);

                player.transform.rotation =
                    Quaternion.Slerp(
                        player.transform.rotation,
                        targetRot,
                        Time.deltaTime * 12f
                    );
            }
        }

        if (Camera.main)
        {
            Camera.main.fieldOfView = Mathf.Lerp(
                Camera.main.fieldOfView,
                targetFov,
                Time.deltaTime * zoomLerpSpeed
            );
        }
    }

    void HandleZoomInput()
    {
        float zoomInput = 0f;

        if (controllerConnected)
        {
            float stick = Input.GetAxis("RightStickVertical");
            float triggers = Input.GetAxis("LT") - Input.GetAxis("RT");

            zoomInput = Mathf.Abs(stick) > 0.1f
                ? stick
                : triggers;
        }
        else
        {
            zoomInput = Input.GetAxis("Mouse ScrollWheel");
        }

        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetFov -= zoomInput * zoomSpeed;
        }

        targetFov = Mathf.Clamp(
            targetFov,
            minFov,
            maxFov
        );
    }

    void CheckController()
    {
        string[] joysticks = Input.GetJoystickNames();

        controllerConnected = false;

        foreach (string j in joysticks)
        {
            if (!string.IsNullOrEmpty(j))
            {
                controllerConnected = true;
                break;
            }
        }
    }
}