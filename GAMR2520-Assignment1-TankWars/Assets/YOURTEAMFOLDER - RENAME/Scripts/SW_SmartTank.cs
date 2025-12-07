using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;

public class SW_SmartTank : AITank
{
    public GameObject enemyTank;
    public GameObject consumable;
    public GameObject enemyBase;
    public SW_StateMachine StateMachine;
    

    public override void AITankStart()
    {
        InitializeStateMachine();
    }

    public override void AITankUpdate()
    {
        
    }

    public override void AIOnCollisionEnter(Collision collision)
    {
        
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, SW_BaseState> states = new Dictionary<Type, SW_BaseState>();
        states.Add(typeof(SW_PatrolState), new SW_PatrolState(this));
        states.Add(typeof(SW_ChaseState), new SW_ChaseState(this));

        GetComponent<SW_StateMachine>().SetStates(states);
    }
    

}
