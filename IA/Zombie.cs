using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    FieldOfView fov;
    NavMeshAgent agent;
    Animator anim;
    AudioSource audioSource;

    private Transform player;

    public string statut;

    public float speed;
    private float chaseSpeed = 4f;
    private float walkSpeed = 1f;

    private float patrolRadius = 30f;
    private float patrolTimer = 10f;
    private float timerCount;

    //distance entre le joueur et l'ennemi
    [SerializeField] private float distance;
    //portee des attaques
    public float attackRange = 1f;
    //cooldownd des attaques
    public float attackrepeatTime = 5f;
    private float attackTime;
    public float damage;

    private void Awake()
    {
        fov = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Start()
    {
        timerCount = patrolTimer;
    }

    
    void Update()
    {

        if (fov.hasDetected)
        {
            Vector3 dir = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
            fov.partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            Chase();
        }
        else
        {
            Patrol();
        }


    }//Update

    private void Patrol()
    {
        statut = "patrol";

        agent.speed = walkSpeed;
        timerCount += Time.deltaTime;

        if (timerCount > patrolTimer)
        {
            SetNewRandomDestination();

            timerCount = 0f;
        }

        if (agent.velocity.sqrMagnitude == 0)
        {
            anim.SetFloat("Blend", 0f);
        }
        else
        {
            anim.SetFloat("Blend", 0.5f);
        }

    }

    public void Chase()
    {
        statut = "chase";

        distance = Vector3.Distance(player.position, transform.position);

        /*if (healthHandler.isDead)
        {
            Mort();
            return;
        }*/
        if (distance <= attackRange)
        {
            Attack();
            return;
        }

        agent.isStopped = false;
        agent.destination = player.position;
        agent.speed = chaseSpeed;
        anim.SetBool("atak", false);
        anim.SetFloat("Blend", 1f);
    }//Chase

    public void Attack()
    {
        if (distance > attackRange)
        {
            Chase();
            return;
        }

        statut = "attack";

        agent.isStopped = true;
        anim.SetBool("atak", true);
    }//Attack

    void SetNewRandomDestination()
    {
        Vector3 newDestination = RandomNavSphere(transform.position, patrolRadius, -1);
        agent.SetDestination(newDestination);
    }//SetNewRandomDestination
    Vector3 RandomNavSphere(Vector3 originPos, float radius, int layerMask)
    {
        Vector3 ranDir = UnityEngine.Random.insideUnitSphere * radius;
        ranDir += originPos;

        NavMeshHit navHit;
        NavMesh.SamplePosition(ranDir, out navHit, radius, layerMask);

        return navHit.position;
    }//RandomNavSphere



}//Class
