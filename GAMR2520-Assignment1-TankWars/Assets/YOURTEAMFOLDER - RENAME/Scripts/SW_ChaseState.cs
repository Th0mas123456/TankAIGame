using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState : SW_BaseState
{
    private SW_SmartTank tank;
    public HeuristicMode heuristicMode;

    public SW_ChaseState(SW_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Chase State");
        return typeof(SW_ChaseState);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState);
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;
        if (tank.enemyTank != null)
        {
            if (Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position) < 25f)
            {
                return typeof(SW_AttackState);
            }
            else
            {
                tank.FollowPathToWorldPoint(tank.enemyTank, 1f, heuristicMode);
                return typeof(SW_ChaseState);
            }
        }
        else
        {
            return typeof (SW_PatrolState);
        }
    }
}
