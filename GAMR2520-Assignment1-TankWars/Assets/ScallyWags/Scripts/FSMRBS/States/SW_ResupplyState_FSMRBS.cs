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
        return typeof(SW_ResupplyState_FSMRBS);
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
            tank.FollowPathToWorldPoint(tank.consumable, 0.8f, tank.heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                tank.GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
        else if (tank.TankCurrentHealth < 30 || tank.TankCurrentFuel < 30 || tank.TankCurrentAmmo < 3)
        {

            tank.FollowPathToRandomWorldPoint(0.8f, tank.heuristicMode);
            return typeof(SW_ResupplyState_FSMRBS);
        }
        else
        {
            return typeof(SW_PatrolState_FSMRBS);
        }

        return typeof(SW_ResupplyState_FSMRBS);
    }
}



