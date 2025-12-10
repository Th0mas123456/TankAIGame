using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState_FSMRBS : SW_BaseState_FSMRBS
{
    private SW_SmartTank_FSMRBS tank;

    public SW_ChaseState_FSMRBS(SW_SmartTank_FSMRBS tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Chase State");
        return typeof(SW_ChaseState_FSMRBS);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSMRBS);
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;
        if (tank.enemyTank != null)
        {
            if (Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position) < 25f)
            {
                return typeof(SW_AttackState_FSMRBS);
            }
            else
            {
                tank.FollowPathToWorldPoint(tank.enemyTank, 1f, tank.heuristicMode);
                return typeof(SW_ChaseState_FSMRBS);
            }
        }
        else
        {
            return typeof(SW_PatrolState_FSMRBS);
        }
    }
}
