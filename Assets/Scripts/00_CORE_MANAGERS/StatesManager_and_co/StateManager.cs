using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manicomio.Assets.Scripts.States;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class StateManager
{
    // stores all the states 
    private Dictionary<StateType, BaseState> states = new Dictionary<StateType, BaseState>();
    private BaseState currentState;

    public StateType GetCurrentState()
    {
        return currentState.type;
    }

    public void AddState(BaseState state)
    {
        if (!states.ContainsKey(state.type))
        {
            states.Add(state.type, state);
        }
    }

    public void ChangeState(StateType type)
    {
        // when null is caught, it will not be executed
        currentState?.Exit();
        // gets the state as BaseState from the list
        currentState = states[type];
        // executes the "Enter" method of a script
        currentState.Enter();
    }

    // why update constantly? Depends on the script no? Wouldn't "on input" be better? Ask later lmao
    public void Update()
    {
        // constantly execute them
        currentState?.Execute();
    }
}
