using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_PatrolState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank;
    private float t;

    public SW_PatrolState_FSM(SW_SmartTank_FSM tank)
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
            return typeof (SW_ResupplyState_FSM);
        }
        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
        {
            return typeof(SW_ChaseState_FSM);
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
        return typeof(SW_PatrolState_FSM);
    }
}
