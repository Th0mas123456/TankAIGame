using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank;

    public SW_ChaseState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Chase State");
        return typeof(SW_ChaseState_FSM);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSM);
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;
        if (tank.enemyTank != null)
        {
            if (Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position) < 25f)
            {
                return typeof(SW_AttackState_FSM);
            }
            else
            {
                tank.FollowPathToWorldPoint(tank.enemyTank, 1f, tank.heuristicMode);
                return typeof(SW_ChaseState_FSM);
            }
        }
        else
        {
            return typeof (SW_PatrolState_FSM);
        }
    }
}
