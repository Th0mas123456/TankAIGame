using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SW_BTBaseNode
{
    protected SW_BTNodeState btNodeState;
    public SW_BTNodeState BTNodeState
    {
        get { return btNodeState; }
    }

    public abstract SW_BTNodeState Evaluate();
}