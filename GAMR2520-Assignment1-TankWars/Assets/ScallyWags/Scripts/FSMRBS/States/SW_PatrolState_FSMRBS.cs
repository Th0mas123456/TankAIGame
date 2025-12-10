using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_PatrolState_FSMRBS : SW_BaseState_FSMRBS
{
    private SW_SmartTank_FSMRBS tank;
    private float t;

    public SW_PatrolState_FSMRBS(SW_SmartTank_FSMRBS tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Patrol State");
        t = 0;
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.TankCurrentHealth < 30 || tank.TankCurrentFuel < 30 || tank.TankCurrentAmmo < 3)
        {
            return typeof(SW_ResupplyState_FSMRBS);
        }
        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
        {
            return typeof(SW_ChaseState_FSMRBS);
        }
        else
        {
            tank.FollowPathToRandomWorldPoint(0.5f, tank.heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                Debug.Log(t);
                tank.GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
        return typeof(SW_PatrolState_FSMRBS);
    }
}

