using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
[Serializable]
public class SkillTest : ISkill
{
    [SerializeField] private Collider hitbox;

    void Start()
    {
        
    }

    public override void Spawn(Vector3 position)
    {
        ObjectPoolManager.SpawnObject(spell, position, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
    }

    // Reserved for vfx outside 
    public override IEnumerator OnClickExecute() 
    {
        yield return 1;
    }

    public override IEnumerator DestroySkill()
    {
        yield return new WaitForSeconds(lifetime);
                
        ObjectPoolManager.ReturnObjectToPool(spell, ObjectPoolManager.PoolType.GameObjects);
    }

    void OnTriggerEnter(Collider other)
    {
        // change player for enemy or smth
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("HAHAHAHA");
            // Do something
        }
    }
}
