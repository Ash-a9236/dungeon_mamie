using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ISkill : MonoBehaviour
{
    public GameObject spell;
    public float lifetime;
    public float maxRange;
    public abstract void Spawn(Vector3 position);
    public abstract IEnumerator OnClickExecute();
    public abstract IEnumerator DestroySkill();
}
