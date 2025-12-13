using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;
using UnityEngine.XR;

public class SW_SmartTank_FSMRBSBT : AITank
{
    // Reference to the currently targeted enemy tank
    public GameObject enemyTank;

    // Reference to a consumable (ammo / fuel / health pickup)
    public GameObject consumable;

    // Reference to the enemy base
    public GameObject enemyBase;
    public HeuristicMode heuristicMode;
    float t;
    
    //Behaviour Tree actions and sequences being delared
    public SW_BTAction healthCheck;
    public SW_BTAction ammoCheck;
    public SW_BTAction fuelCheck;
    public SW_BTAction targetSpottedCheck;
    public SW_BTAction targetReachedCheck;
    public SW_BTSequence regenSequence;



    //dictionary that will hold all of the stats that the rules will use to change states
    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    public SW_RulesBT rules = new SW_RulesBT();
    public override void AITankStart()
    {
        InitializeStateMachine();
        InitialiseStats();
        InitialiseRules();
        InitialiseBT();
    }

    public override void AITankUpdate()
    {
    }

    public override void AIOnCollisionEnter(Collision collision)
    {
    }

    //Adds all of the used states to a dictionary so that the statemachine can cycle through them
    private void InitializeStateMachine()
    {
        Dictionary<Type, SW_BaseState_FSMRBSBT> states = new Dictionary<Type, SW_BaseState_FSMRBSBT>();
        states.Add(typeof(SW_PatrolState_FSMRBSBT), new SW_PatrolState_FSMRBSBT(this));
        states.Add(typeof(SW_ChaseState_FSMRBSBT), new SW_ChaseState_FSMRBSBT(this));
        states.Add(typeof(SW_AttackState_FSMRBSBT), new SW_AttackState_FSMRBSBT(this));
        states.Add(typeof(SW_RetreatState_FSMRBSBT), new SW_RetreatState_FSMRBSBT(this));
        states.Add(typeof(SW_ResupplyState_FSMRBSBT), new SW_ResupplyState_FSMRBSBT(this));

        GetComponent<SW_StateMachine_FSMRBSBT>().SetStates(states);
    }

    //Adds all of the stats the rules use to changes states
    void InitialiseStats()

    {
        stats.Add("lowHealth", false);
        stats.Add("lowFuel", false);
        stats.Add("lowAmmo", false);
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

    //Adds all the rules so that it knows when to change states
    void InitialiseRules()
    {
        rules.AddRule(new SW_RuleBT("patrolState", "needResupply", typeof(SW_ResupplyState_FSMRBSBT), SW_RuleBT.Predicate.And));
        rules.AddRule(new SW_RuleBT("patrolState", "targetSpotted", typeof(SW_ChaseState_FSMRBSBT), SW_RuleBT.Predicate.And));

        rules.AddRule(new SW_RuleBT("chaseState", "noTarget", typeof(SW_PatrolState_FSMRBSBT), SW_RuleBT.Predicate.And));
        rules.AddRule(new SW_RuleBT("chaseState", "targetReached", typeof(SW_AttackState_FSMRBSBT), SW_RuleBT.Predicate.And));

        rules.AddRule(new SW_RuleBT("attackState", "noTarget", typeof(SW_PatrolState_FSMRBSBT), SW_RuleBT.Predicate.And));
        rules.AddRule(new SW_RuleBT("attackState", "lowHealth", typeof(SW_RetreatState_FSMRBSBT), SW_RuleBT.Predicate.And));

        rules.AddRule(new SW_RuleBT("retreatState", "noTarget", typeof(SW_PatrolState_FSMRBSBT), SW_RuleBT.Predicate.And));

        rules.AddRule(new SW_RuleBT("resupplyState", "needResupply", typeof(SW_PatrolState_FSMRBSBT), SW_RuleBT.Predicate.nAnd));
    }

    //creates the actions and sequences of the behaviour tree so that it can check if actions or successes and can list importance of certain actions
    public void InitialiseBT()
    {
        healthCheck = new SW_BTAction(HealthCheck);
        ammoCheck = new SW_BTAction(AmmoCheck);
        fuelCheck = new SW_BTAction(FuelCheck);
        targetSpottedCheck = new SW_BTAction(TargetSpottedCheck);
        targetReachedCheck = new SW_BTAction(TargetReachedCheck);
        regenSequence = new SW_BTSequence(new List<SW_BTBaseNode> { healthCheck, fuelCheck , ammoCheck });
    }

    //checks if the target has been reached and sets the stat accordingly
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

    //checks if the target has been spotted and sets the stat accordingly
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

    //fires a projectile at the target
    public void attackTarget()
    {
        enemyTank = VisibleEnemyTanks.First().Key;
        TurretFireAtPoint(enemyTank);
        checkHealth();
        targetReached();
    }

    //moves around the map generating new world points to move to every 10 seconds
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

    //chases after the enemy position the checks if it should change states if it gets close or loses it
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

    //checks if it is low on supplies and adjust the stat based on that
    public void checkResupply()
    {
        if (TankCurrentHealth < 30 || TankCurrentFuel < 30 || TankCurrentAmmo < 3)
        {
            stats["lowHealth"] = true;
            stats["lowFuel"] = true;
            stats["lowAmmo"] = true;
            stats["needResupply"] = true;
        }
        else
        {
            stats["lowHealth"] = false;
            stats["lowFuel"] = false;
            stats["lowAmmo"] = false;
            stats["needResupply"] = false;
        }
    }

    //searchs for consumables then picks them up if it finds them
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

    //runs in a random direction away from the enemy
    public void retreat()
    {
        FollowPathToRandomWorldPoint(1f, heuristicMode);
        checkHealth();
        targetSpotted();
        targetReached();

    }

    //checks if health is low and sets the state based on that
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

    //uses the stat to check if the BTnode was a failure or success
    public SW_BTNodeState HealthCheck()
    {
        if (stats["lowHealth"])
        {
            return SW_BTNodeState.FAILURE;
        }
        else
        {
            return SW_BTNodeState.SUCCESS;
        }
    }

    //uses the stat to check if the BTnode was a failure or success
    public SW_BTNodeState AmmoCheck()
    {
        if (stats["lowAmmo"])
        {
            return SW_BTNodeState.FAILURE;
        }
        else
        {
            return SW_BTNodeState.SUCCESS;
        }
    }

    //uses the stat to check if the BTnode was a failure or success
    public SW_BTNodeState FuelCheck()
    {
        if (stats["lowFuel"])
        {
            return SW_BTNodeState.FAILURE;
        }
        else
        {
            return SW_BTNodeState.SUCCESS;
        }
    }

    //uses the stat to check if the BTnode was a failure or success
    public SW_BTNodeState TargetSpottedCheck()
    {
        if (stats["targetSpotted"])
        { 
            return SW_BTNodeState.SUCCESS;
        }
        else
        {
            return SW_BTNodeState.FAILURE;
        }
    }


    //uses the stat to check if the BTnode was a failure or success
    public SW_BTNodeState TargetReachedCheck()
    {
        if (stats["targetReached"])
        {
            return SW_BTNodeState.SUCCESS;
        }
        else
        {
            return SW_BTNodeState.FAILURE;
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
