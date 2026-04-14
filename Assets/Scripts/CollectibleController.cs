using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    public enum CollectibleType
    {
        Score,
        SpeedBoost,
        JumpBoost,
        EnergyRecharge
    }

    [Header("Collectible Type")]
    public CollectibleType type;

    [Header("Values")]
    public int scoreValue = 50;
    public float speedBoostAmount = 3f;
    public float jumpBoostAmount = 1.5f;
    public float boostDuration = 5f;

    [Header("Hover")]
    public float hoverHeight = 0.25f;
    public float hoverSpeed = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 90f;

    [Header("Effects")]
    public GameObject pickupEffect;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Hover();
        Rotate();
    }

    private void Hover()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterMovement player = other.GetComponent<CharacterMovement>();

        switch (type)
        {
            case CollectibleType.Score:
                GameManager.Instance.AddScore(scoreValue);
                break;

            case CollectibleType.SpeedBoost:
                player?.ApplySpeedBoost(speedBoostAmount, boostDuration);
                break;

            case CollectibleType.JumpBoost:
                player?.ApplyJumpBoost(jumpBoostAmount, boostDuration);
                break;

            case CollectibleType.EnergyRecharge:
                player?.RechargeDoubleJump();
                break;
        }

        TriggerEffect();
        Destroy(gameObject);
    }

    private void TriggerEffect()
    {
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
    }
}