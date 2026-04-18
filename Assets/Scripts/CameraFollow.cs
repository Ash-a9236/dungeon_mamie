using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public CharacterMovement playerMovement;

    [Header("2D Mode")]
    public float offsetZ2D = -10f;

    [Header("3D Mode")]
    public Vector3 offset3D = new Vector3(0, 10, -10);
    public float tiltAngle = 90f;
    public float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null || playerMovement == null) return;

        bool is3DMode = playerMovement.currentMode == CharacterMovement.MovementMode.ThreeD;
        Vector3 desiredPosition;

        if (!is3DMode)
        {
            desiredPosition = new Vector3(target.position.x, target.position.y, offsetZ2D);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            desiredPosition = target.position + offset3D;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            Vector3 lookDirection = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            rotation = Quaternion.Euler(tiltAngle, rotation.eulerAngles.y, 0f);
            transform.rotation = rotation;
        }
        is3DMode = true;
    }
}