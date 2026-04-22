using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // TODO: ORGANIZE THE HIERARCHY
    [Header("Components")] 
    [SerializeField] private Transform transform;
    [SerializeField] private Rigidbody2D rb;
    // public Collider hitbox;

    [Header("Speed")]
    public float speed = 0.1f;
    // public bool isDashing = false;

    // [Header("Dash")]
    // [SerializeField] private float DASH_SPEED;
    // public GameObject dashPrefab;
    // private bool canDash = true;
    
    // [Header("Mouse Reticle")]
    // public Transform reticle;

    // [Header("Spell")]
    // [SerializeField] private SpellManager spellManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        rb = GetComponent<Rigidbody2D>();
        //pellManager = GetComponent<SpellManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        rb.MovePosition(direction * speed + rb.position);

        // RaycastHit hit = MouseManager.GetRaycastHit();

        // reticle.position = hit.point;

        // ? Dash based on the mouse's position
        // if (Input.GetKeyDown(KeyCode.Mouse1) && canDash && !isDashing)
        // {
        //     Vector3 dashDirection = hit.point - transform.position;

        //     DashStart(dashDirection.normalized);

        //     Invoke("DashStop", 0.5f);
        //     Invoke("DashCooldownReset", 0.75f);
        // }
        // else if (!isDashing)
        // {
            // rb.velocity = Vector2.Lerp(rb.velocity, direction * SPEED, 2 * Time.deltaTime);
        // }

        // if (direction.normalized == Vector2.zero)
        // {
        //     rb.velocity = Vector2.Lerp(rb.velocity, Vector3.zero, 2 * Time.deltaTime);
        // }

        // if (Input.GetButton("Fire1"))
        // {
        //     if (spellManager.CanUseSkill())
        //     {
        //         spellManager.UseSkill();
        //     }
        // }

        // rb.velocity = direction * SPEED * Time.deltaTime;
    }

    // public void DashStart(Vector3 dashDirection)
    // {
    //     canDash = false;
    //     hitbox.enabled = false;
    //     isDashing = true;

    //     rb.velocity = dashDirection * DASH_SPEED;

    //     ObjectPoolManager.SpawnObject(dashPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

    //     InvokeRepeating("DashUpdate", 0.1f, 0.1f);

    //     gameObject.GetComponentInChildren<Renderer>().enabled = false;
    // }
    // public void DashUpdate()
    // {

    //     ObjectPoolManager.SpawnObject(dashPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    // }
    // public void DashStop()
    // {
    //     hitbox.enabled = true;

    //     isDashing = false;

    //     CancelInvoke("DashUpdate");

    //     ObjectPoolManager.SpawnObject(dashPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);

    //     gameObject.GetComponentInChildren<Renderer>().enabled = true;
    // }
    // public void DashCooldownReset()
    // {
    //     canDash = true;
    // }
}
