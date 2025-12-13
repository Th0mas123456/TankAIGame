using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;
using System.Runtime.CompilerServices;

public class SW_RetreatState_FSMRBSBT : SW_BaseState_FSMRBSBT
{
    private SW_SmartTank_FSMRBSBT tank;

    public float safeDistance = 40f;

    public SW_RetreatState_FSMRBSBT(SW_SmartTank_FSMRBSBT tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("entered retreat state");
        tank.stats["patrolState"] = false;
        tank.stats["chaseState"] = false;
        tank.stats["attackState"] = false;
        tank.stats["retreatState"] = true;
        tank.stats["resupplyState"] = false;
        return typeof(SW_RetreatState_FSMRBSBT);

    }

    public override Type StateExit()
    {
        tank.stats["retreatState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        tank.retreat();

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