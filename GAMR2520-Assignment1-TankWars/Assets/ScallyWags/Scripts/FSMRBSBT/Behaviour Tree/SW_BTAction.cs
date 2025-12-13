using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SW_BTAction : SW_BTBaseNode
{
    public delegate SW_BTNodeState ActionNodeFunction();
    private ActionNodeFunction btAction;

    public SW_BTAction(ActionNodeFunction btAction)
    {
        this.btAction = btAction;
    }

    public override SW_BTNodeState Evaluate()
    {
        switch (btAction())
        {
            case SW_BTNodeState.SUCCESS:
                btNodeState = SW_BTNodeState.SUCCESS;
                return btNodeState;
            case SW_BTNodeState.FAILURE:
                btNodeState = SW_BTNodeState.FAILURE;
                return btNodeState;
            default:
                btNodeState = SW_BTNodeState.FAILURE;
                return btNodeState;
        }
    }
}
