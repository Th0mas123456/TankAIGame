using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SW_BTSelector : SW_BTBaseNode
{
    protected List<SW_BTBaseNode> btNodes = new List<SW_BTBaseNode>(); //list of BT nodes

    //constructor
    public SW_BTSelector(List<SW_BTBaseNode> btNodes)
    {
        this.btNodes = btNodes;
    }

    //Evaluates the failure or success of the node its in and returns the state of the node
    public override SW_BTNodeState Evaluate()
    {
        foreach (SW_BTBaseNode btNode in btNodes)
        {
            switch (btNode.Evaluate())
            {
                case SW_BTNodeState.FAILURE:
                    continue;
                case SW_BTNodeState.SUCCESS:
                    btNodeState = SW_BTNodeState.SUCCESS;
                    return btNodeState;
                default:
                    continue;
            }
        }
        btNodeState = SW_BTNodeState.FAILURE;
        return btNodeState;
    }
}
