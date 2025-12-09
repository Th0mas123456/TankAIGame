using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
public class SW_AttackState : SW_BaseState
{
    private SW_SmartTank tank;
    private GameObject enemyTank;
    
    public SW_AttackState(SW_SmartTank tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        Debug.Log("Entered attack State");
        return typeof(SW_AttackState);
    }
    float t;
    public HeuristicMode heuristicMode;

    public override Type StateUpdate()
        {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState);
        }
        if (tank.TankCurrentHealth < 50)
        {
            return typeof(SW_RetreatState);
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
                    return typeof(SW_ChaseState);
                }
            }
        }
           
        return typeof(SW_AttackState);

        }
    public override Type StateExit()
        { 
            return null; 
        }
}