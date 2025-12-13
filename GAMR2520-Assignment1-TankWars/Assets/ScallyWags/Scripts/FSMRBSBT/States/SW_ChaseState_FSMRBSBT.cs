using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState_FSMRBSBT : SW_BaseState_FSMRBSBT
{
    private SW_SmartTank_FSMRBSBT tank;

    public SW_ChaseState_FSMRBSBT(SW_SmartTank_FSMRBSBT tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Chase State");
        tank.stats["patrolState"] = false;
        tank.stats["chaseState"] = true;
        tank.stats["attackState"] = false;
        tank.stats["retreatState"] = false;
        tank.stats["resupplyState"] = false;
        return typeof(SW_ChaseState_FSMRBSBT);
    }

    public override Type StateExit()
    {
        tank.stats["chaseState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        tank.chaseTarget();

        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
            {
                return item.CheckRule(tank.stats);
            }
        }

        return null;
    }
}