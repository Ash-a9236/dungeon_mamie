using System.Collections;
using System.Collections.Generic;
using Manicomio.Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
    private AIController aiController;
    public StateType type => StateType.IDLE;
    public float idleDuration = 1.5f;
    public float idleTimer = 0f;

    public IdleState(AIController aiController)
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
            aiController.stateManager.ChangeState(StateType.CHASING);
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            aiController.stateManager.ChangeState(StateType.PATROL);
        }
    }

    public void Exit()
    {
        idleTimer = 0f;
    }
}
