using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static AStar;

public class SW_ResupplyState : SW_BaseState
{
    private SW_SmartTank tank;
    public HeuristicMode heuristicMode;
    float t;

    public SW_ResupplyState(SW_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Resupply State");
        return typeof(SW_ResupplyState);
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
             tank.FollowPathToWorldPoint(tank.consumable, 1f, heuristicMode);
             t += Time.deltaTime;
             if (t > 10)
             {
                 tank.GenerateNewRandomWorldPoint();
                 t = 0;
             }
         }
        else
        {
            tank.FollowPathToRandomWorldPoint(1f, heuristicMode);
        }

            return null;
    }
}
    
       
