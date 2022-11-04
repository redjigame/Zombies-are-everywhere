using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  // Added since we're using a navmesh.

public class AI : MonoBehaviour
{
    // Variables to handle what we need to send through to our state.
    AudioSource audioSource;
    NavMeshAgent agent; // To store the NPC NavMeshAgent component.
    Animator anim; // To store the Animator component.
    private Transform player;  // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).
    State currentState;

    [SerializeField] AudioClip idleClip;
    [SerializeField] AudioClip chaseClip;
    [SerializeField] AudioClip attackClip;
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        agent = this.GetComponent<NavMeshAgent>(); // Grab agents NavMeshAgent.
        anim = this.GetComponent<Animator>(); // Grab agents Animator component.
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentState = new Idle(this.gameObject, agent, anim, player, audioSource); // Create our first state.
    }

    void Update()
    {
        currentState = currentState.Process(); // Calls Process method to ensure correct state is set.

        if (PauseMenu.gameIsPaused)
        {
            audioSource.volume = 0f;
        }
        else
        {
            audioSource.volume = 1f;
        }
    }

    public void ApplyDamage()
    {
        Debug.Log("Aie");
    }

    public void EnableRagdoll()
    {
        anim.enabled = false;
        audioSource.enabled = false;
        agent.enabled = false;
        this.enabled = false;
    }

    public void PlayIdleClip()
    {
        audioSource.clip = idleClip;
        audioSource.Play();
    }

    public void PlayChaseClip()
    {
        audioSource.clip = chaseClip;
        audioSource.Play();
    }

    public void PlayAttackClip()
    {
        audioSource.clip = attackClip;
        audioSource.Play();
    }

}
