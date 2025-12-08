using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;

public class SW_ChaseState : SW_BaseState
{
    private SW_SmartTank tank;
    private GameObject enemyTank;
    private HeuristicMode heuristic;

    public SW_ChaseState(SW_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        enemyTank = VisibleEnemyTanks.First().Key;
        if (enemyTank != null)
        {
            if (Vector3.Distance(transform.position, tank.transform.position) < 25f)
            {
                // canAttack
            }
            else
            {
                FollowPathToWorldPoint(enemyTank, 1f, heauristicMode);
            }
        }
        return null;
    }
}
