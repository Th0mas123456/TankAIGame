using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_PatrolState : SW_BaseState
{
    private SW_SmartTank tank;
    private float t;
    private HeuristicMode heuristicMode = HeuristicMode.Euclidean;

    public SW_PatrolState(SW_SmartTank tank)
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
        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
        {
            return typeof(SW_ChaseState);
        }
        else
        {
            tank.FollowPathToRandomWorldPoint(1f, heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                Debug.Log(t);
                tank.GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
        return typeof(SW_PatrolState);
    }
}
