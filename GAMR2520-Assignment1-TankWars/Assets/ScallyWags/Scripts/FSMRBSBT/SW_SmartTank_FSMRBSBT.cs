using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class SW_SmartTank_FSMRBSBT : AITank
{
    public GameObject enemyTank;
    public GameObject consumable;
    public GameObject enemyBase;
    public HeuristicMode heuristicMode;
    float t;


    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    public Rules rules = new Rules();
    public override void AITankStart()
    {
        InitializeStateMachine();
        InitialiseStats();
        InitialiseRules();
    }

    public override void AITankUpdate()
    {
    }

    public override void AIOnCollisionEnter(Collision collision)
    {
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, SW_BaseState_FSMRBSBT> states = new Dictionary<Type, SW_BaseState_FSMRBSBT>();
        states.Add(typeof(SW_PatrolState_FSMRBSBT), new SW_PatrolState_FSMRBSBT(this));
        states.Add(typeof(SW_ChaseState_FSMRBSBT), new SW_ChaseState_FSMRBSBT(this));
        states.Add(typeof(SW_AttackState_FSMRBSBT), new SW_AttackState_FSMRBSBT(this));
        states.Add(typeof(SW_RetreatState_FSMRBSBT), new SW_RetreatState_FSMRBSBT(this));
        states.Add(typeof(SW_ResupplyState_FSMRBSBT), new SW_ResupplyState_FSMRBSBT(this));

        GetComponent<SW_StateMachine_FSMRBS>().SetStates(states);
    }

    void InitialiseStats()

    {
        stats.Add("lowHealth", false);
        stats.Add("needResupply", false);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("noTarget", true);
        stats.Add("retreatState", false);
        stats.Add("chaseState", false);
        stats.Add("patrolState", true);
        stats.Add("attackState", false);
        stats.Add("resupplyState", false);
    }

    void InitialiseRules()
    {
        rules.AddRule(new Rule("patrolState", "needResupply", typeof(SW_ResupplyState_FSMRBS), Rule.Predicate.And));
        rules.AddRule(new Rule("patrolState", "targetSpotted", typeof(SW_ChaseState_FSMRBS), Rule.Predicate.And));

        rules.AddRule(new Rule("chaseState", "noTarget", typeof(SW_PatrolState_FSMRBS), Rule.Predicate.And));
        rules.AddRule(new Rule("chaseState", "targetReached", typeof(SW_AttackState_FSMRBS), Rule.Predicate.And));

        rules.AddRule(new Rule("attackState", "noTarget", typeof(SW_PatrolState_FSMRBS), Rule.Predicate.And));
        rules.AddRule(new Rule("attackState", "lowHealth", typeof(SW_RetreatState_FSMRBS), Rule.Predicate.And));

        rules.AddRule(new Rule("retreatState", "noTarget", typeof(SW_PatrolState_FSMRBS), Rule.Predicate.And));

        rules.AddRule(new Rule("resupplyState", "needResupply", typeof(SW_PatrolState_FSMRBS), Rule.Predicate.nAnd));
    }


    public void targetReached()
    {
        if (VisibleEnemyTanks.Count > 0)
        {
            GameObject enemyTank = VisibleEnemyTanks.First().Key;

            if (Vector3.Distance(transform.position, enemyTank.transform.position) < 25f)
                stats["targetReached"] = true;
            else
                stats["targetReached"] = false;
        }
        else
        {
            stats["targetReached"] = false;
        }
    }

    public void targetSpotted()
    {
        if (VisibleEnemyTanks.Count > 0)
        {
            stats["noTarget"] = false;
            stats["targetSpotted"] = true;
        }
        else
        {
            stats["noTarget"] = true;
            stats["targetSpotted"] = false;
        }
    }

    public void attackTarget()
    {
        enemyTank = VisibleEnemyTanks.First().Key;
        TurretFireAtPoint(enemyTank);
        checkHealth();
        targetReached();
    }

    public void patrolMap()
    {
        FollowPathToRandomWorldPoint(0.5f, heuristicMode);
        t += Time.deltaTime;
        if (t > 10)
        {
            GenerateNewRandomWorldPoint();
            t = 0;
        }
        targetSpotted();
        targetReached();
        checkResupply();
        checkHealth();
    }

    public void chaseTarget()
    {
        if (VisibleEnemyTanks.Count > 0)
        {
            enemyTank = VisibleEnemyTanks.First().Key;
            FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
        }
        checkHealth();
        targetSpotted();
        targetReached();
    }

    public void checkResupply()
    {
        if (TankCurrentHealth < 30 || TankCurrentFuel < 30 || TankCurrentAmmo < 3)
        {
            stats["needResupply"] = true;
        }
        else
        {
            stats["needResupply"] = false;
        }
    }
    public void resupply()
    {
        if (VisibleConsumables.Count > 0)
        {
            consumable = VisibleConsumables.First().Key;
            FollowPathToWorldPoint(consumable, 0.8f, heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
        else if (TankCurrentHealth < 30 || TankCurrentFuel < 30 || TankCurrentAmmo < 3)
        {

            FollowPathToRandomWorldPoint(0.8f, heuristicMode);
        }
        checkResupply();
    }

    public void retreat()
    {
        FollowPathToRandomWorldPoint(1f, heuristicMode);
        checkHealth();
        targetSpotted();
        targetReached();

    }

    public void checkHealth()
    {
        if (TankCurrentHealth < 30)
        {
            stats["lowHealth"] = true;
        }
        else
        {
            stats["lowHealth"] = false;
        }
    }



    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        a_FindPathToPoint(pointInWorld);
    }

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld, HeuristicMode heuristic)
    {
        a_FindPathToPoint(pointInWorld, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). 
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield. 
    ///If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        a_FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToRandomPoint(normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        a_GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        a_StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        a_StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        a_FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        a_ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        a_FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return a_IsFiring;
    }

    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    /// <returns>Current health.</returns>
    public float TankCurrentHealth
    {
        get
        {
            return a_GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    /// <returns>Current ammo.</returns>
    public float TankCurrentAmmo
    {
        get
        {
            return a_GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    /// <returns>Current fuel level.</returns>
    public float TankCurrentFuel
    {
        get
        {
            return a_GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases. Does not include bases which have been destroyed.
    /// </summary>
    /// <returns>List of your own bases which are. </returns>
    public List<GameObject> MyBases
    {
        get
        {
            return a_GetMyBases;
        }
    }
    public Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return a_TanksFound;
        }
    }
    public Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return a_ConsumablesFound;
        }
    }
    public Dictionary<GameObject, float> VisibleBases
    {
        get
        {
            return a_BasesFound;
        }
    }
}
