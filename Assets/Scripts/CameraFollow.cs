using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public CharacterMovement playerMovement;

    [Header("Mode Override (Zones)")]
    public bool overrideMode = false;
    public Mode forcedMode = Mode.Mode3D;

    public enum Mode
    {
        Mode2D,
        Mode3D
    }

    [Header("2D Settings")]
    public Vector3 offset2D = new Vector3(0, 0, -10f);
    public float orthoSize2D = 5f;
    public float smooth2D = 0.1f;

    [Header("3D Settings")]
    public Vector3 offset3D = new Vector3(0, 6f, -8f);
    public float rotationX3D = 20f;
    public float smooth3D = 0.12f;
    public float rotationSmooth = 8f;

    [Header("Zoom Settings")]
    public float minFov = 40f;
    public float maxFov = 80f;

    public float minOrtho = 3f;
    public float maxOrtho = 8f;

    public float zoomSpeed = 10f;
    public float zoomLerpSpeed = 6f;

    private Vector3 velocity;

    private float targetFov;
    private float targetOrtho;

    private bool controllerConnected;

    void Start()
    {
        targetFov = Camera.main.fieldOfView;
        targetOrtho = Camera.main.orthographicSize;

        CheckController();
    }

    void Update()
    {
        CheckController();
        HandleZoomInput();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Mode mode = GetMode();

        if (mode == Mode.Mode2D)
            Handle2D();
        else
            Handle3D();
    }

    Mode GetMode()
    {
        if (overrideMode)
            return forcedMode;

        if (playerMovement == null)
            return Mode.Mode3D;

        return playerMovement.currentMode == CharacterMovement.MovementMode.ThreeD
            ? Mode.Mode3D
            : Mode.Mode2D;
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

    void HandleZoomInput()
    {
        float zoomInput = 0f;

        if (controllerConnected)
        {
            float stick = Input.GetAxis("RightStickVertical");
            float triggers = Input.GetAxis("LT") - Input.GetAxis("RT");

            zoomInput = Mathf.Abs(stick) > 0.1f ? stick : triggers;
        }
        else
        {
            zoomInput = Input.GetAxis("Mouse ScrollWheel");
        }

        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetFov -= zoomInput * zoomSpeed;
            targetOrtho -= zoomInput * (zoomSpeed * 0.1f);
        }

        targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
        targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
    }

    void Handle2D()
    {
        Vector3 desiredPos = new Vector3(
            target.position.x + offset2D.x,
            target.position.y + offset2D.y,
            offset2D.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smooth2D
        );

        transform.rotation = Quaternion.identity;

        Camera.main.orthographic = true;

        Camera.main.orthographicSize = Mathf.Lerp(
            Camera.main.orthographicSize,
            targetOrtho,
            Time.deltaTime * zoomLerpSpeed
        );
    }

    void Handle3D()
    {
        Vector3 desiredPos = target.position + offset3D;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smooth3D
        );

        Vector3 lookDir = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);

        Quaternion finalRot = Quaternion.Euler(
            rotationX3D,
            lookRot.eulerAngles.y,
            0f
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRot,
            Time.deltaTime * rotationSmooth
        );

        Camera.main.orthographic = false;

        Camera.main.fieldOfView = Mathf.Lerp(
            Camera.main.fieldOfView,
            targetFov,
            Time.deltaTime * zoomLerpSpeed
        );
    }
}