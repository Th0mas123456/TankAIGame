using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static AStar;

public class SW_ResupplyState : SW_BaseState
{
    private SW_SmartTank tank;
    public HeuristicMode heuristicMode;

    public SW_ResupplyState(SW_SmartTank tank)
    {  
        this.tank = tank;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered Resupply State");
        return typeof(SW_ResupplyState);
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        return null;
    }
}
