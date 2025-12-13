using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;
using UnityEngine.XR;

public class SW_PatrolState_FSMRBSBT : SW_BaseState_FSMRBSBT
{
    private SW_SmartTank_FSMRBSBT tank;//referance to the tank script

    //constructor of the state
    public SW_PatrolState_FSMRBSBT(SW_SmartTank_FSMRBSBT tank)
    {
        this.tank = tank;
    }

    //upon state entry it sets all the state stats to false except the current one to prevent errors
    public override Type StateEnter()
    {
        Debug.Log("Entered patrol state");
        tank.stats["patrolState"] = true;
        tank.stats["chaseState"] = false;
        tank.stats["attackState"] = false;
        tank.stats["retreatState"] = false;
        tank.stats["resupplyState"] = false;
        return typeof(SW_PatrolState_FSMRBSBT);
    }

    //exits the state and sets the state stat to false
    public override Type StateExit()
    {
        tank.stats["patrolState"] = false;
        tank.stats["noTarget"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.regenSequence.Evaluate() == SW_BTNodeState.SUCCESS)//checks to see if the resupply/regen sequence completed
        {
            tank.patrolMap();//runs the function thats in smart tank

            //goes through all the rules and checks the rules with the stats to see if the state needs chaging

            foreach (var item in tank.rules.GetRules)
            {
                if (item.CheckRule(tank.stats) != null)
                {
                    return item.CheckRule(tank.stats);
                }
            }
        }
        return null;
    }
}