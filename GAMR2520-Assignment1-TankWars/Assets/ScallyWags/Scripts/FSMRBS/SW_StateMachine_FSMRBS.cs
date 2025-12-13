using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SW_StateMachine_FSMRBS : MonoBehaviour
{
    private Dictionary<Type, SW_BaseState_FSMRBS> states; //dictionary of states set in the smartTank script
    public SW_BaseState_FSMRBS currentState;

    //Setter and Getter for the state
    public SW_BaseState_FSMRBS CurrentState
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

    public void SetStates(Dictionary<Type, SW_BaseState_FSMRBS> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First(); //if there is no current state then go to the first one in the dictionary
        }
        else
        {
            Type nextState = CurrentState.StateUpdate();
            if (nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState); //changes states when a different state type is returned
            }
        }
    }
    //runs the functions created in baseState that exits then changes the state and enters the new one
    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }

}

