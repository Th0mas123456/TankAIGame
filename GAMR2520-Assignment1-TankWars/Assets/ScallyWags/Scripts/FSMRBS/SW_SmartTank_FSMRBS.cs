using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class SW_SmartTank_FSMRBS : AITank
{
    // Reference to the currently targeted enemy tank
    public GameObject enemyTank;

    // Reference to a consumable (ammo / fuel / health pickup)
    public GameObject consumable;

    // Reference to the enemy base
    public GameObject enemyBase;
    public HeuristicMode heuristicMode;
    float t; //float the represents time

    //dictionary that will hold all of the stats that the rules will use to change states
    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    public SW_Rules rules = new SW_Rules();
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

    //Adds all of the used states to a dictionary so that the statemachine can cycle through them

    private void InitializeStateMachine()
    {
        Dictionary<Type, SW_BaseState_FSMRBS> states = new Dictionary<Type, SW_BaseState_FSMRBS>();
        states.Add(typeof(SW_PatrolState_FSMRBS), new SW_PatrolState_FSMRBS(this));
        states.Add(typeof(SW_ChaseState_FSMRBS), new SW_ChaseState_FSMRBS(this));
        states.Add(typeof(SW_AttackState_FSMRBS), new SW_AttackState_FSMRBS(this));
        states.Add(typeof(SW_RetreatState_FSMRBS), new SW_RetreatState_FSMRBS(this));
        states.Add(typeof(SW_ResupplyState_FSMRBS), new SW_ResupplyState_FSMRBS(this));

        GetComponent<SW_StateMachine_FSMRBS>().SetStates(states);
    }

    //Adds all of the stats the rules use to changes states
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

    //Adds all the rules so that it knows when to change states
    void InitialiseRules()
    {
        rules.AddRule(new SW_Rule("patrolState", "needResupply", typeof(SW_ResupplyState_FSMRBS), SW_Rule.Predicate.And));
        rules.AddRule(new SW_Rule("patrolState", "targetSpotted", typeof(SW_ChaseState_FSMRBS), SW_Rule.Predicate.And));

        rules.AddRule(new SW_Rule("chaseState", "noTarget", typeof(SW_PatrolState_FSMRBS), SW_Rule.Predicate.And));
        rules.AddRule(new SW_Rule("chaseState", "targetReached", typeof(SW_AttackState_FSMRBS), SW_Rule.Predicate.And));

        rules.AddRule(new SW_Rule("attackState", "noTarget", typeof(SW_PatrolState_FSMRBS), SW_Rule.Predicate.And));
        rules.AddRule(new SW_Rule("attackState", "lowHealth", typeof(SW_RetreatState_FSMRBS), SW_Rule.Predicate.And));

        rules.AddRule(new SW_Rule("retreatState", "noTarget", typeof(SW_PatrolState_FSMRBS), SW_Rule.Predicate.And));

        rules.AddRule(new SW_Rule("resupplyState", "needResupply", typeof(SW_PatrolState_FSMRBS), SW_Rule.Predicate.nAnd));
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
        if(TankCurrentHealth < 30 || TankCurrentFuel < 30 || TankCurrentAmmo < 3)
        {
            stats["needResupply"] = true;
        }
        else
        {
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
