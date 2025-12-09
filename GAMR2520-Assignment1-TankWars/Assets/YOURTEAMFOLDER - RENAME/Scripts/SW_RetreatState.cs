using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_RetreatState : SW_BaseState
{
    private SW_SmartTank tank;
    private GameObject enemyTank;

    public HeuristicMode heuristicMode = HeuristicMode.Euclidean;

    public float safeDistance = 40f;

    public SW_RetreatState(SW_SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("entered retreat state");
        return typeof(SW_RetreatState);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        // no visible enemies, return to patrol
        if (tank.VisibleEnemyTanks.Count == 0)
            return typeof(SW_PatrolState);

        enemyTank = tank.VisibleEnemyTanks.First().Key;

        if (enemyTank == null)
            return typeof(SW_PatrolState);

        float dist = Vector3.Distance(tank.transform.position, enemyTank.transform.position);

        // if within safe distance, return to patrolling
        if (dist > safeDistance)
            return typeof(SW_PatrolState);

        // retreat movement
        Vector3 dirAway = (tank.transform.position - enemyTank.transform.position).normalized;
        Vector3 retreatPoint = tank.transform.position + dirAway * 12f;

        tank.FollowPathToWorldPoint(retreatPoint, 1f, heuristicMode);

        return typeof(SW_RetreatState);
    }
}