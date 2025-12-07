using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;

public class SW_AttackState : SW_BaseState
{

    public GameObject enemyTank;

    public GameObject consumable;

    public GameObject enemyBase;

    float t;
    public HeuristicMode heuristicMode;

    public void UpdateState()
        {
            if (VisibleEnemyTank.Count > 0 && VisibleEnemyTanks.First().Key != null)
            {

                enemyTank = VisibleEnemyTanks.First().Key;

                if (enemyTank != null)
                {
                    float dist = Vector3.Distance(transform.position, enemyTank.transform.position);
                    
                    if (dist < 25f)
                     {
                        TurretFireAtPoint(enemyTank);
                     }
                    else
                    {
                         FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
                    }
            }
        }
    }
}