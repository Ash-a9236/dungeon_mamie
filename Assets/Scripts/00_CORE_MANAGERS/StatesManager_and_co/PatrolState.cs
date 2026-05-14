using System.Collections;
using System.Collections.Generic;
using Manicomio.Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private AIController aiController;
    public StateType type => StateType.PATROL;

    public PatrolState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // reserved for animations 
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            if (aiController.Agent.pathPending)
            {
                aiController.Agent.ResetPath();
            }
            Debug.Log("I see things and they are telling me to reach for the officer's gun!");
            aiController.stateManager.ChangeState(StateType.CHASING);

            return;
        }

        if (!aiController.Agent.pathPending && aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
        {
            aiController.StartCoroutine(MoveToRandomPoint());
            return;
        }
    }

    public void Exit()
    {
        if (aiController.Agent.pathPending)
        {
            aiController.Agent.ResetPath();
        }
    }

    private IEnumerator MoveToRandomPoint()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        aiController.Agent.SetDestination(randomPoint + aiController.transform.position);
        Debug.Log("Moving to random point: " + randomPoint);
        yield return new WaitForSeconds(2f);
    }
}
