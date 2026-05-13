using System.Collections;
using System.Collections.Generic;
using Manicomio.Assets.Scripts.States;
using UnityEngine;

public interface BaseState
{
    StateType type { get; }
    void Enter();
    void Execute();
    void Exit();
}
