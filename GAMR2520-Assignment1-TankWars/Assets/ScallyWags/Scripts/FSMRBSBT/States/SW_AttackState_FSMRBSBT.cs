using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
using System.Data;
public class SW_AttackState_FSMRBSBT : SW_BaseState_FSMRBSBT
{
    private SW_SmartTank_FSMRBSBT tank;

    public SW_AttackState_FSMRBSBT(SW_SmartTank_FSMRBSBT tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered attack State");
        tank.stats["patrolState"] = false;
        tank.stats["chaseState"] = false;
        tank.stats["attackState"] = true;
        tank.stats["retreatState"] = false;
        tank.stats["resupplyState"] = false;
        return typeof(SW_AttackState_FSMRBS);
    }

    public override Type StateUpdate()
    {
        tank.attackTarget();


        foreach (var item in tank.rules.GetRules)
        {
            if (item.CheckRule(tank.stats) != null)
            {
                return item.CheckRule(tank.stats);
            }
        }

        return null;

    }
    public override Type StateExit()
    {
        tank.stats["attackState"] = false;
        return null;
    }
}