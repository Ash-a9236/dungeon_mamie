using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// using Unity.Burst.CompilerServices;
// using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class SpellManager : MonoBehaviour
{
    [SerializeReference, SerializeField]
    public ISkill currentSkill;
    [SerializeReference, SerializeField]
    List<ISkill> skills;
    [SerializeField] private int maxSpells = 5;
    [SerializeField] private GameObject player;
    [SerializeField] private bool debounce = false;
    [SerializeField] private float globalSkillCooldown = 0.5f;
    [SerializeField] private float timeElapsed = 0f;

    void Start()
    {
        
    }

    public bool CanUseSkill()
    {
        return !debounce;
    }

    public void UseSkill()
    {
        if (currentSkill.Equals(null) && !debounce)
        {
            Debug.Log("null skill");

            return;
        }

        debounce = true;
        
        Debug.Log("skill used");

        // getting mouse position and player position
        Vector3 position = MouseManager.GetRaycastHit().point;

        Vector3 playerPosition = player.transform.position;

        // range n stuff
        if (Vector3.Distance(playerPosition, position) > currentSkill.maxRange)
        {
            Vector3 distance = position - playerPosition;

            position = Vector3.ClampMagnitude(distance, currentSkill.maxRange) + playerPosition;
        }

        // spawning n stuff
        GameObject obj = ObjectPoolManager.SpawnObject(currentSkill.spell, position, Quaternion.identity);
        
        // change later
        SkillFaceCamera.rotateSkill(obj.transform, player.transform);
        
        // recycling the skill and resetting the cool down
        StartCoroutine(ResetCooldown());
        StartCoroutine(RecycleSkill(obj, currentSkill.lifetime));
    }

    public void ChangeSkill(int index = 0)
    {
        if (index > skills.Count || index < 0)
        {
            Debug.Log("idk when this triggers i'll be honest, but its good to have");

            return;
        }

        currentSkill = skills[index];
    }

    public void AddSkill(ISkill skill)
    {
        if (skills.Contains(skill))
        {
            Debug.Log("hey you already have that skill! Deleting C:\\Windows\\System32 ...");
            // System.IO.Directory.Delete("C:\\Windows\\System32");
            return;
        }

        if (skills.Count >= maxSpells)
        {
            Debug.Log("ur not that guy bro ur not that guy");
            return;
        }

        skills.Add(skill);
    }

    public void ReplaceSkill(ISkill skill)
    {
        if (skills.Contains(skill) || skill.Equals(currentSkill))
        {
            Debug.Log("do NOT");

            return;
        }

        int currentSkillInList = skills.BinarySearch(currentSkill);

        skills[currentSkillInList] = skill;

        currentSkill = skill;
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(globalSkillCooldown);

        Debug.Log("hahaha ronaldinho soccer");

        debounce = false;
    }

    IEnumerator RecycleSkill(GameObject skill, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        ObjectPoolManager.ReturnObjectToPool(skill);
    }
}
