using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;
using System.Runtime.CompilerServices;

public class SW_RetreatState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank; //tank object so the state can use and return instructions for the tank

    public float safeDistance = 40f; //float used to check if the tank is safe

    //states constructor
    public SW_RetreatState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        return typeof(SW_RetreatState_FSM);
        
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        // no visible enemies, return to patrol
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSM);
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;

        if (tank.enemyTank == null)
        {
            return typeof(SW_PatrolState_FSM);
        }
        float dist = Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position);

        // if within safe distance, return to patrolling
        if (dist > safeDistance)
        {
            return typeof(SW_PatrolState_FSM);
        }
        // retreat movement
        tank.FollowPathToRandomWorldPoint(1f, tank.heuristicMode);

        return typeof(SW_RetreatState_FSM);
    }
}