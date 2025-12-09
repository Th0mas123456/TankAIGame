using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;
using System.Runtime.CompilerServices;

public class SW_RetreatState : SW_BaseState
{
    private SW_SmartTank tank;
    public HeuristicMode heuristicMode = HeuristicMode.Euclidean;

    public float safeDistance = 40f;

    public SW_RetreatState(SW_SmartTank tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        Debug.Log("entered retreat state");
        Debug.Log(tank.TankCurrentHealth);
        return typeof(SW_RetreatState);
        
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
            return typeof(SW_PatrolState);
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;

        if (tank.enemyTank == null)
        {
            return typeof(SW_PatrolState);
        }
        float dist = Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position);

        // if within safe distance, return to patrolling
        if (dist > safeDistance)
        {
            return typeof(SW_PatrolState);
        }
        // retreat movement
        //Vector3 dirAway = (tank.transform.position - tank.enemyTank.transform.position).normalized;

        //tank.FollowPathToWorldPoint(retreatPoint, 1f, heuristicMode);
        tank.FollowPathToRandomWorldPoint(1f, heuristicMode);

        return typeof(SW_RetreatState);
    }
}