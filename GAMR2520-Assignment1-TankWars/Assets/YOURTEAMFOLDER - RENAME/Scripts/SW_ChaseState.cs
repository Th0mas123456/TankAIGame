using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState : SW_BaseState
{
    private SW_SmartTank tank;
    private GameObject enemyTank;
    public HeuristicMode heuristicMode;

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
        enemyTank = tank.VisibleEnemyTanks.First().Key;
        if (enemyTank != null)
        {
            if (Vector3.Distance(tank.transform.position, enemyTank.transform.position) < 25f)
            {
                return typeof(SW_AttackState);
            }
            else
            {
                tank.FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
            }
        }
        return null;
    }
}
