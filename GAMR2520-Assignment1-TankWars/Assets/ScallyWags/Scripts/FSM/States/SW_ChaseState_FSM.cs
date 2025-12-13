using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ChaseState_FSM : SW_BaseState_FSM
{
    private SW_SmartTank_FSM tank;  //tank object so the state can use and return instructions for the tank

    //constructor for the state
    public SW_ChaseState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank;
    }

    //runs when this state is switched to
    public override Type StateEnter()
    {
        return typeof(SW_ChaseState_FSM);
    }

    //ran when the state is left
    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.VisibleEnemyTanks.Count == 0) 
        {
            return typeof(SW_PatrolState_FSM); //if there are no enemies then it changes to patrol state
        }
        tank.enemyTank = tank.VisibleEnemyTanks.First().Key;
        if (tank.enemyTank != null)
        {
            if (Vector3.Distance(tank.transform.position, tank.enemyTank.transform.position) < 25f)
            {
                return typeof(SW_AttackState_FSM); //if they are close enough to the enemy then they switch to attack state
            }
            else
            {
                tank.FollowPathToWorldPoint(tank.enemyTank, 1f, tank.heuristicMode); //if not close enough they follow after the enemy
                return typeof(SW_ChaseState_FSM);
            }
        }
        else
        {
            return typeof (SW_PatrolState_FSM); //as a redundency this is here to return to patrol state
        }
    }
}
