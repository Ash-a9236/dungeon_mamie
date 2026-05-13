using System.Collections;
using System.Collections.Generic;
using Manicomio.Assets.Scripts.States;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : BaseState
{
    private AIController aiController;
    public StateType type => StateType.CHASING;

    public ChaseState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // no anims ig
    }

    public void Execute()
    {
        // if cannot see player, return to patrol
        if (!aiController.CanSeePlayer())
        {
            // but before that, get the last point the player was and move there
            aiController.Agent.SetDestination(aiController.LastKnownPlayerPosition);

            aiController.stateManager.ChangeState(StateType.PATROL);
            return;
        }

        if (aiController.CanAttack())
        {
            aiController.stateManager.ChangeState(StateType.ATTACKING);
            return;
        }

        aiController.Agent.SetDestination(aiController.LastKnownPlayerPosition);
    }

    public void Exit()
    {

    }
}
