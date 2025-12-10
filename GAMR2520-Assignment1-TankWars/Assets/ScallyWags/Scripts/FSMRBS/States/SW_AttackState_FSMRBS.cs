using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
public class SW_AttackState_FSMRBS : SW_BaseState_FSMRBS
{
    private SW_SmartTank_FSMRBS tank;
    private GameObject enemyTank;

    public SW_AttackState_FSMRBS(SW_SmartTank_FSMRBS tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered attack State");
        return typeof(SW_AttackState_FSMRBS);
    }
    float t;
    public HeuristicMode heuristicMode;

    public override Type StateUpdate()
    {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSMRBS);
        }
        if (tank.TankCurrentHealth < 30)
        {
            return typeof(SW_RetreatState_FSMRBS);
        }

        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
        {
            enemyTank = tank.VisibleEnemyTanks.First().Key;
            if (enemyTank != null)
            {
                float dist = Vector3.Distance(tank.transform.position, enemyTank.transform.position);

                if (dist < 25f)
                {
                    tank.TurretFireAtPoint(enemyTank);

                }
                else
                {
                    return typeof(SW_ChaseState_FSMRBS);
                }
            }
        }

        return typeof(SW_AttackState_FSMRBS);

    }
    public override Type StateExit()
    {
        return null;
    }
}