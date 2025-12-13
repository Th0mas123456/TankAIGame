using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SW_BTBaseNode
{
    //base class to create the functions for the other classes to inherrit
    protected SW_BTNodeState btNodeState;
    public SW_BTNodeState BTNodeState
    {
        get { return btNodeState; }
    }

    public abstract SW_BTNodeState Evaluate();
}