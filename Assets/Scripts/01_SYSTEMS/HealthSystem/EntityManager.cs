using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float armour;
    public int level;
    public float baseDamage;
    private readonly float ONE_SHOT_THRESHOLD = 0.9f;
    readonly float ONE_SHOT_RECOVERY_HEALTH = 5f;

    public void DealDamage(GameObject target, float additionalDmg = 0)
    {
        if (target.TryGetComponent<EntityManager>(out var entity) && entity != this && entity.currentHealth >= 1)
        {
            entity.TakeDamage(baseDamage + additionalDmg);
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        if (currentHealth <= 0 || incomingDamage <= 0 || incomingDamage - armour <= 0) return;

        if (incomingDamage > (currentHealth * ONE_SHOT_THRESHOLD))
        {
            currentHealth = ONE_SHOT_RECOVERY_HEALTH;
            return;
        }

        currentHealth -= incomingDamage - armour;
    }

    /// <summary>
    /// Increases the damage of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20" or "50"</param>
    public void BuffDamage(float percentage)
    {
        baseDamage *= 1 + (percentage / 100);
    }

    /// <summary>
    /// Decreases the damage of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20f" or "50f"</param>
    public void DebuffDamage(float percentage)
    {
        baseDamage /= 1 + (percentage / 100);
    }

    /// <summary>
    /// Increases the health of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20" or "50"</param>
    public void BuffMaxHealth(float percentage)
    {
        maxHealth *= 1 + (percentage / 100);
    }

    /// <summary>
    /// Decreases the health of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20f" or "50f"</param>
    public void DebuffMaxHealth(float percentage)
    {
        maxHealth /= 1 + (percentage / 100);
    }

    /// <summary>
    /// Increases the armor of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20" or "50"</param>
    public void BuffArmor(float percentage)
    {
        baseDamage *= 1 + (percentage / 100);
    }

    /// <summary>
    /// Decreases the armor of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20f" or "50f"</param>
    public void DebuffArmor(float percentage)
    {
        baseDamage /= 1 + (percentage / 100);
    }
}
