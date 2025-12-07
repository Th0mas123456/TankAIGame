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
        states.Add(typeof(SW_AttackState), new SW_AttackState(this));

        GetComponent<SW_StateMachine>().SetStates(states);
    }
    

    public Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return a_TanksFound;
        }
    }
    public Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return a_ConsumablesFound;
        }
    }
    public Dictionary<GameObject, float> VisibleBases
    {
        get
        {
            return a_BasesFound;
        }
    }
}
