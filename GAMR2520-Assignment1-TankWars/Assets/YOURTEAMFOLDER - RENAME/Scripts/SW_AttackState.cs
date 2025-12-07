using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
public class SW_AttackState : SW_BaseState
{
    private SW_SmartTank tank;
    
        public SW_AttackState(SW_SmartTank tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        return null;
    }
    float t;
    public HeuristicMode heuristicMode;

    public override Type StateUpdate()
        {
            if (tank.VisibleEnemyTank.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
            {

                enemyTank = tank.VisibleEnemyTanks.First().Key;

                if (enemyTank != null)
                {
                    float dist = Vector3.Distance(transform.position, enemyTank.transform.position);
                    
                    if (dist < 25f)
                     {
                        TurretFireAtPoint(enemyTank);
                     }
                    else
                    {
                         FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
                    }
            }
        }
    }
    public override Type StateExit()
        { 
            return null; 
        }
}