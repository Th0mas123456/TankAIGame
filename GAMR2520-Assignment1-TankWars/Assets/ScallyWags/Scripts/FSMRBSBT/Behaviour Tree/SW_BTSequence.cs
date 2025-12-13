using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SW_BTSequence : SW_BTBaseNode
{
    protected List<SW_BTBaseNode> btNodes = new List<SW_BTBaseNode>(); //list of BT nodes

    //constructor
    public SW_BTSequence(List<SW_BTBaseNode> btNodes)
    {
        this.btNodes = btNodes;
    }

    //Evaluates if the state of the node is failure or success and either breaks the switch or continues nodes in the given sequence
    public override SW_BTNodeState Evaluate()
    {
        bool failed = false;
        foreach (SW_BTBaseNode btNode in btNodes)
        {
            if (failed == true)
            {
                break;
            }

            switch (btNode.Evaluate())
            {
                case SW_BTNodeState.FAILURE:
                    btNodeState = SW_BTNodeState.FAILURE;
                    failed = true;
                    break;
                case SW_BTNodeState.SUCCESS:
                    btNodeState = SW_BTNodeState.SUCCESS;
                    continue;
                default:
                    btNodeState = SW_BTNodeState.FAILURE;
                    failed = true;
                    break;
            }
        }
        return btNodeState;
    }
}