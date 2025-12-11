using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using System;
public class SW_AttackState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank; //Reference to the tank that owns this state
    private GameObject enemyTank; // This names the currently targeted enemy tank
    
    public SW_AttackState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }
    
    public override Type StateEnter()
    {
        // Called when this state becomes active
        Debug.Log("Entered attack State");
        return typeof(SW_AttackState_FSM);
    }
    

    public override Type StateUpdate()
        {
        // if there are no visible enemy tanks then switch to patrol state
        if (tank.VisibleEnemyTanks.Count == 0)
        {
            return typeof(SW_PatrolState_FSM);
        }
        // if tank on low health then switch to retreat state
        if (tank.TankCurrentHealth < 30)
        {
            return typeof(SW_RetreatState_FSM);
        }
        // verifies is an enemy is visible or not
        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null)
        {
            enemyTank = tank.VisibleEnemyTanks.First().Key;
            if (enemyTank != null)
            {
                // finds the distance between our tank and the enemy tank
                float dist = Vector3.Distance(tank.transform.position, enemyTank.transform.position);
                // if the enemy is within range then it will fire 
                if (dist < 25f)
                {
                    tank.TurretFireAtPoint(enemyTank);

                }
                else
                {
                    // if tank is too far the nswitch to Chase state
                    return typeof(SW_ChaseState_FSM);
                }
            }
        }
           // remain in attack state by default
        return typeof(SW_AttackState_FSM);

        }
    public override Type StateExit()
        { 
            return null; 
        }
}