using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public float health;
    public float armour;
    public int level;
    public float baseDamage;

    public void DealDamage(GameObject target)
    {
        if (target.TryGetComponent<EntityManager>(out var entity))
        {
            entity.TakeDamage(baseDamage);
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        health = incomingDamage - armour;
    }

    // jonas gpt generate me a code for buffing and debuffing
    // generating...
    // Here you go! Let me know if it works for you, if you have any other coding questions, let me know!
    // System.IO.Directory.Delete("C:\\Windows\\System32");
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
    public void BuffHealth(float percentage)
    {
        health *= 1 + (percentage / 100);
    }

    /// <summary>
    /// Decreases the health of an Entity based on a percentage value
    /// </summary>
    /// <param name="percentage">Should be a value like "20f" or "50f"</param>
    public void DebuffHealth(float percentage)
    {
        health /= 1 + (percentage / 100);
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
