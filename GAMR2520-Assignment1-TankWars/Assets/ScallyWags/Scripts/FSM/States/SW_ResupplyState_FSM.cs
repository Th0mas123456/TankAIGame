using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static AStar;

public class SW_ResupplyState_FSM : SW_BaseState_FSM // Resupply state references off the Base state
{
    private SW_SmartTank_FSM tank;
    float t;

    public SW_ResupplyState_FSM(SW_SmartTank_FSM tank)
    {
        this.tank = tank; // Reference to the Smart Tank script, which allows the use of all the functions stored in that script in other scripts
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Resupply State"); // Debug log, shows whether the state is entered. Great for testing as you can see if the tank is stuck in a state or not entering a different state.
        return typeof(SW_ResupplyState_FSM);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        // Overall this checks for visible consumables, if there are any it will go towards it.
        // Otherwise it generates a new random point in the world (every 10 seconds) in order to cover most of the map to find consumables

         if (tank.VisibleConsumables.Count > 0) // If there are visible consumables (more than 0)
         {
             tank.consumable = tank.VisibleConsumables.First().Key; // Get the first consumable from the list
             tank.FollowPathToWorldPoint(tank.consumable, 0.8f, tank.heuristicMode); // Moves / follows toward the consumable
             t += Time.deltaTime; // Timer starts
             if (t > 10) // If timer goes over 10 seconds
             {
                 tank.GenerateNewRandomWorldPoint(); // Generate new random point in the world
                 t = 0; // Reset timer
             }
         }
        else if (tank.TankCurrentHealth < 30 || tank.TankCurrentFuel < 30 || tank.TankCurrentAmmo < 3) // Checks if all supplies are low
        {
            
            tank.FollowPathToRandomWorldPoint(0.8f, tank.heuristicMode); // Goes to random world point, in hopes of spotting a consumable
            return typeof(SW_ResupplyState_FSM); // If it is then it stays in resupply state
        }
        else
        {
            return typeof(SW_PatrolState_FSM); // If the supplies are NOT low, it will switch to patrol state
        }

            return typeof(SW_ResupplyState_FSM);
    }
}
    
       
