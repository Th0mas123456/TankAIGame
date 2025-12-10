using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static AStar;

public class SW_ResupplyState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank;
    float t;

    public SW_ResupplyState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Resupply State");
        return typeof(SW_ResupplyState_FSM);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        
         if (tank.VisibleConsumables.Count > 0)
         {
             tank.consumable = tank.VisibleConsumables.First().Key;
             tank.FollowPathToWorldPoint(tank.consumable, 1f, tank.heuristicMode);
             t += Time.deltaTime;
             if (t > 10)
             {
                 tank.GenerateNewRandomWorldPoint();
                 t = 0;
             }
         }
        else
        {
            tank.FollowPathToRandomWorldPoint(1f, tank.heuristicMode);
        }

        return typeof(SW_ResupplyState_FSM);
    }
}
    
       
