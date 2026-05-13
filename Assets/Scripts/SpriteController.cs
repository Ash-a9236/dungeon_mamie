using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool SpriteSprite = true;
    [SerializeField] private bool flipSprite = true;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody targetRigidbody;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (!target)
        {
            target = transform.parent;
        }

        if (!targetRigidbody && target)
        {
            targetRigidbody =
                target.GetComponent<Rigidbody>();
        }
    }

    void LateUpdate()
    {
        HandleSprite();

        HandleFlip();
    }

    void HandleSprite()
    {
        if (!SpriteSprite)
            return;

        if (!cam)
            return;

        transform.forward =
            cam.transform.forward;
    }

    void HandleFlip()
    {
        if (!flipSprite)
            return;

        if (!targetRigidbody)
            return;

        Vector3 velocity =
            targetRigidbody.velocity;

        velocity.y = 0f;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        if (velocity.x > 0.05f)
        {
            transform.localScale =
                new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
        }
        else if (velocity.x < -0.05f)
        {
            transform.localScale =
                new Vector3(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
        }
    }
}