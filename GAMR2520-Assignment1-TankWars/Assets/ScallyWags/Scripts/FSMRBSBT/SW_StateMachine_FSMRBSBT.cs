using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SW_StateMachine_FSMRBSBT : MonoBehaviour
{
    private Dictionary<Type, SW_BaseState_FSMRBSBT> states;
    public SW_BaseState_FSMRBSBT currentState;
    public SW_BaseState_FSMRBSBT CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }

    public void SetStates(Dictionary<Type, SW_BaseState_FSMRBSBT> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            Type nextState = CurrentState.StateUpdate();
            if (nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }
    }
    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }

}