using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manicomio.Assets.Scripts.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    StateType currentState;
    public Transform Player;
    public float radius;
    public float viewDistance;
    public LayerMask obstacleMask;
    public float AttackRange;
    public Vector3 LastKnownPlayerPosition { get; private set; }
    Renderer rend;
    public StateManager stateManager;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

        stateManager = new StateManager();

        rend = GetComponent<Renderer>();

        stateManager.AddState(new ChaseState(this));
        stateManager.AddState(new AttackState(this));
        stateManager.AddState(new PatrolState(this));
        stateManager.AddState(new IdleState(this));

        stateManager.ChangeState(StateType.IDLE);
    }

    void Update()
    {
        stateManager.Update();
        currentState = stateManager.GetCurrentState();
        CanSeePlayer();
    }

    public bool CanSeePlayer()
    {

        Vector3 toPlayer = Player.position - transform.position;

        // Step 1 — Distance
        if (toPlayer.magnitude > viewDistance)
            return false;

        // Step 2 — Angle (dot product)
        float dot = Vector3.Dot(transform.forward, toPlayer.normalized);

        float threshold = Mathf.Cos(360 * Mathf.Deg2Rad);

        if (dot < threshold)
            return false;

        // Step 3 — Line of sight (raycast)
        if (Physics.Raycast(transform.position, toPlayer.normalized,
                            toPlayer.magnitude, obstacleMask))
            return false;

        LastKnownPlayerPosition = Player.position;

        Debug.Log("I see things and they are telling me to reach for the officer's gun!");

        return true;
    }

    public bool CanAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        return distanceToPlayer <= AttackRange;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -360 / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 360 / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }
}
