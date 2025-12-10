using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
public class SW_AttackState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank;
    private GameObject enemyTank;
    
    public SW_AttackState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        Debug.Log("Entered attack State");
        return typeof(SW_AttackState_FSM);
    }
    float t;
    public HeuristicMode heuristicMode;

    public override Type StateUpdate()
        {
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSM);
        }
        if (tank.TankCurrentHealth < 50)
        {
            return typeof(SW_RetreatState_FSM);
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
                    return typeof(SW_ChaseState_FSM);
                }
            }
        }
           
        return typeof(SW_AttackState_FSM);

        }
    public override Type StateExit()
        { 
            return null; 
        }
}