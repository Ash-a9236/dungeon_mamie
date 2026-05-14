using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillFaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // point.x = 90;
        // point.z = 0;

        // transform.up = point;

    }

    public static void rotateSkill(Transform skill, Transform player)
    {
        Vector3 positionBetween = skill.transform.position - player.transform.position;

        float angle = Mathf.Atan2(positionBetween.x, positionBetween.z) * Mathf.Rad2Deg;

        skill.transform.eulerAngles = new Vector3(90f, angle * -1, 0f);
    }
}
