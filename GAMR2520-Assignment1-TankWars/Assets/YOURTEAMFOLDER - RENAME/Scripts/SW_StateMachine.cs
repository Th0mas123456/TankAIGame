using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SW_StateMachine : MonoBehaviour
{
    private Dictionary<Type, SW_BaseState> states;
    public SW_BaseState currentState;
    public SW_BaseState CurrentState
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

    public void SetStates(Dictionary<Type, SW_BaseState> states)
    {
        this.states = states;
    }

    void Update()
    {
        if(CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            Type nextState = CurrentState.GetType();
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
