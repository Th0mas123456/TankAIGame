using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_PatrolState : SW_BaseState
{
    private SW_SmartTank tank;
    private GameObject enemyTank;
    private float t;
    private HeuristicMode heuristic;
    public SW_PatrolState(SW_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        t = 0;
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (Vector3.Distance(tank.transform.position, enemyTank.transform.position) < 3f)
        {
            return typeof(SW_ChaseState);
        }
        else
        {
            tank.FollowPathToRandomWorldPoint(1f, heuristic);
            return null;
        }
    }
}
