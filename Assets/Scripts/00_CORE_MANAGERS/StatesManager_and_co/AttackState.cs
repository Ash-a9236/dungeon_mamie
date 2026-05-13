using System.Collections;
using System.Collections.Generic;
using Manicomio.Assets.Scripts.States;
using UnityEngine;

public class AttackState : BaseState
{
    private AIController aiController;
    public StateType type => StateType.ATTACKING;
    private float attackCooldown = 1.5f; // seconds between attacks
    private float lastAttackTime = -999f;

    public AttackState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // no anims ig
        aiController.Agent.ResetPath();
        aiController.Agent.isStopped = true;
    }

    public void Execute()
    {
        if (!aiController.CanAttack())
        {
            aiController.stateManager.ChangeState(StateType.CHASING);
            return;
        }

        if (Time.deltaTime - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.deltaTime;
            // aiController.animationController.SetTrigger("Attack");
        }
    }

    public void Exit()
    {

    }
}
