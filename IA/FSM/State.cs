using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    // 'States' that the NPC could be in.
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, SLEEP, RUNAWAY
    };

    // 'Events' - where we are in the running of a STATE.
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name; // To store the name of the STATE.
    protected EVENT stage; // To store the stage the EVENT is in.
    protected GameObject npc; // To store the NPC game object.
    protected Animator anim; // To store the Animator component.
    protected Transform player; // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).
    protected State nextState; // This is NOT the enum above, it's the state that gets to run after the one currently running (so if IDLE was then going to PATROL, nextState would be PATROL).
    protected NavMeshAgent agent; // To store the NPC NavMeshAgent component.
    protected AudioSource audioSource;

    float visDist = 5.0f; // When the player is within a distance of 10 from the NPC, then the NPC should be able to see it...
    float visAngle = 120.0f; // ...if the player is within 30 degrees of the line of sight.
    float attackRange = 1.5f; // When the player is within a distance of 7 from the NPC, then the NPC can go into an ATTACK state.

    // Constructor for State
    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, AudioSource _audioSource)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
        audioSource = _audioSource;
    }

    // Phases as you go through the state.
    public virtual void Enter() { stage = EVENT.UPDATE; } // Runs first whenever you come into a state and sets the stage to whatever is next, so it will know later on in the process where it's going.
    public virtual void Update() { stage = EVENT.UPDATE; } // Once you are in UPDATE, you want to stay in UPDATE until it throws you out.
    public virtual void Exit() { stage = EVENT.EXIT; } // Uses EXIT so it knows what to run and clean up after itself.

    // The method that will get run from outside and progress the state through each of the different stages.
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState; // Notice that this method returns a 'state'.
        }
        return this; // If we're not returning the nextState, then return the same state.
    }

    // Can the NPC see the player, using a simple Line Of Sight calculation?
    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
        float angle = Vector3.Angle(direction, npc.transform.forward); // Provide angle of sight.

        // If player is close enough to the NPC AND within the visible viewing angle...
        if(direction.magnitude < visDist && angle < visAngle)
        {
            return true; // NPC CAN see the player.
        }
        return false; // NPC CANNOT see the player.
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
        if(direction.magnitude < attackRange)
        {
            return true; // NPC IS close enough to the player to attack.
        }
        return false; // NPC IS NOT close enough to the player to attack.
    }
}

// Constructor for Idle state.
public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, AudioSource _audioSource)
                : base(_npc, _agent, _anim, _player, _audioSource)
    {
        name = STATE.IDLE; // Set name of current state.
    }

    public override void Enter()
    {
        anim.SetTrigger("isIdle"); // Sets any current animation state back to Idle.
        base.Enter(); // Sets stage to UPDATE.
    }
    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, audioSource);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
        // The only place where Update can break out of itself. Set chance of breaking out at 10%.
        else if(Random.Range(0,100) < 10)
        {
            nextState = new Patrol(npc, agent, anim, player, audioSource);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isIdle"); // Makes sure that any events queued up for Idle are cleared out.
        base.Exit();
    }
}

// Constructor for Patrol state.
public class Patrol : State
{
    float patrolRadius = 30f;
    float patrolTimer = 10f;
    float timerCount;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, AudioSource _audioSource)
                : base(_npc, _agent, _anim, _player, _audioSource)
    {
        name = STATE.PATROL; // Set name of current state.
        agent.speed = 0.5f; // How fast your character moves ONLY if it has a path. Not used in Idle state since agent is stationary.
        agent.isStopped = false; // Start and stop agent on current path using this bool.
    }

    public override void Enter()
    {
        timerCount = patrolTimer;
        anim.SetTrigger("isWalking"); // Start agent walking animation.
        base.Enter();
    }

    public override void Update()
    {
        timerCount += Time.deltaTime;
        if (timerCount > patrolTimer)
        {
            SetNewRandomDestination();
            timerCount = 0f;
        }

        if (agent.velocity.sqrMagnitude == 0)
        {
            agent.speed = 1f;
            anim.ResetTrigger("isWalking");
            anim.SetTrigger("isIdle");
        }
        else
        {
            anim.ResetTrigger("isIdle");
            anim.SetTrigger("isWalking");
        }

        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player, audioSource);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking"); // Makes sure that any events queued up for Walking are cleared out.
        base.Exit();
    }

    void SetNewRandomDestination()
    {
        Vector3 newDestination = RandomNavSphere(agent.transform.position, patrolRadius, -1);
        agent.SetDestination(newDestination);
    }
    Vector3 RandomNavSphere(Vector3 originPos, float radius, int layerMask)
    {
        Vector3 ranDir = UnityEngine.Random.insideUnitSphere * radius;
        ranDir += originPos;

        NavMeshHit navHit;
        NavMesh.SamplePosition(ranDir, out navHit, radius, layerMask);

        return navHit.position;
    }//RandomNavSphere
}

public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, AudioSource _audioSource)
                : base(_npc, _agent, _anim, _player, _audioSource)
    {
        name = STATE.PURSUE; // State set to match what NPC is doing.
        agent.speed = 5; // Speed set to make sure NPC appears to be running.
        agent.isStopped = false; // Set bool to determine NPC is moving.
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning"); // Set running trigger to change animation.
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);  // Set goal for NPC to reach but navmesh processing might not have taken place, so...
        if(agent.hasPath)                       // ...check if agent has a path yet.
        {
            if (CanAttackPlayer())
            {
                nextState = new Attack(npc, agent, anim, player, audioSource); // If NPC can attack player, set correct nextState.
                stage = EVENT.EXIT; // Set stage correctly as we are finished with Pursue state.
            }
            // If NPC can't see the player, switch back to Patrol state.
            else if (!CanSeePlayer())
            {
                nextState = new Patrol(npc, agent, anim, player, audioSource); // If NPC can't see player, set correct nextState.
                stage = EVENT.EXIT; // Set stage correctly as we are finished with Pursue state.
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning"); // Makes sure that any events queued up for Running are cleared out.
        base.Exit();
    }
}

public class Attack : State
{
    float rotationSpeed = 2.0f; // Set speed that NPC will rotate around to face player.
    AudioSource shoot; // To store the AudioSource component.
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, AudioSource _audioSource)
                : base(_npc, _agent, _anim, _player, _audioSource)
    {
        name = STATE.ATTACK; // Set name to correct state.
        shoot = _npc.GetComponent<AudioSource>(); // Get AudioSource component for shooting sound.
    }

    public override void Enter()
    {
        anim.SetTrigger("isAttaking"); // Set shooting trigger to change animation.
        agent.isStopped = true; // Stop NPC so he can shoot.
        shoot.Play(); // Play shooting sound.
        base.Enter();
    }

    public override void Update()
    {
        // Calculate direction and angle to player.
        Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
        float angle = Vector3.Angle(direction, npc.transform.forward); // Provide angle of sight.
        direction.y = 0; // Prevent character from tilting.

        // Rotate NPC to always face the player that he's attacking.
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                                            Quaternion.LookRotation(direction),
                                            Time.deltaTime * rotationSpeed);

        if(!CanAttackPlayer())
        {
            nextState = new Idle(npc, agent, anim, player, audioSource); // If NPC can't attack player, set correct nextState.
            stage = EVENT.EXIT; // Set stage correctly as we are finished with Attack state.
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isAttaking"); // Makes sure that any events queued up for Shooting are cleared out.
        shoot.Stop(); // Stop shooting sound.
        base.Exit();
    }
}

/*public class RunAway : State
{
    GameObject safeLocation; // Store object used for safe location.
    
    public RunAway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RUNAWAY; // Set name to correct state.
        safeLocation = GameObject.FindGameObjectWithTag("Safe"); // Find object that was tagged with "Safe" and assign top safeLocation.
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning"); // Set running trigger to change animation.
        agent.isStopped = false; // Set bool to determine NPC is moving.
        agent.speed = 6; // Set speed slightly fsater than when running towards player.
        agent.SetDestination(safeLocation.transform.position); // Set goal for agent to be the safe location.
        base.Enter();
    }

    public override void Update()
    {
        // When the NPC hits the top of the cube, return to the Idle state that has a 10% chance of going into Patrol state.
        if (agent.remainingDistance < 1)
        {
            nextState = new Idle(npc, agent, anim, player); // If NPC can't attack player, set correct nextState.
            stage = EVENT.EXIT; // Set stage correctly as we are finished with Runaway state.
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning"); // Makes sure that any events queued up for Running are cleared out.
        base.Exit();
    }
}*/