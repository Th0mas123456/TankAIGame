using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SW_ChaseState : SW_BaseState
{
    private SW_SmartTank tank;

    public SW_ChaseState(SW_SmartTank tank)
    {
        this.tank = tank;
    }
    public override Type StateEnter()
    {
        return null;
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
