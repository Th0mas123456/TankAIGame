using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static AStar;

public class SW_ResupplyState_FSMRBS : SW_BaseState_FSMRBS
{
    private SW_SmartTank_FSMRBS tank;
    float t;

    public SW_ResupplyState_FSMRBS(SW_SmartTank_FSMRBS tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Resupply State");
        tank.stats["patrolState"] = false;
        tank.stats["chaseState"] = false;
        tank.stats["attackState"] = false;
        tank.stats["retreatState"] = false;
        tank.stats["resupplyState"] = true;
        return typeof(SW_ChaseState_FSMRBS);
    }

    public override Type StateExit()
    {
        tank.stats["resupplyState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {

        tank.resupply();

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



