using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_PatrolState_FSM : SW_BaseState_FSM // Patrol state references off base state
{
    private SW_SmartTank_FSM tank;
    private float t;

    public SW_PatrolState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank; // Allows the use of all functions stored in the smart tank script
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Patrol State"); // Debug log which shows in the unity console whether the state has been entered
        t = 0; // Timer set to 0
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (tank.TankCurrentHealth < 30 || tank.TankCurrentFuel < 30 || tank.TankCurrentAmmo < 3) // If supplies are low (below specified numbers)
        {
            return typeof (SW_ResupplyState_FSM); // Switch to resupply state
        }
        if (tank.VisibleEnemyTanks.Count > 0 && tank.VisibleEnemyTanks.First().Key != null) // If there's more than 0 visible enemy AND it is the first enemy tank it (Scallywag) sees then enter chase state
        {
            return typeof(SW_ChaseState_FSM); // Switch to chase state
        }
        else
        {
            tank.FollowPathToRandomWorldPoint(0.5f, tank.heuristicMode); // Goes to a random world point using pathfinding
            t += Time.deltaTime; // Timer starts
            if (t > 10) // If timer goes over 10 seconds
            {
                Debug.Log(t); // Displays time in unity console
                tank.GenerateNewRandomWorldPoint(); // Generates new random world point to go to
                t = 0; // Reset timer back to 0
            }
        }
        return typeof(SW_PatrolState_FSM);
    }
}
