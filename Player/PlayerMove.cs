using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerFootstep player_footsteps;
    private PlayerStats playerStats;
    private CharacterController character_Controller;

    private Vector3 move_Direction;

    private float normalHeight = 1.8f;
    private float crouchHeight = 1.2f;

    private float crouchSpeed = 2f;
    private float runSpeed = 6f;
    private float walkSpeed = 4f;
    public float currentSpeed;

    private float gravity = 9.8f;

    public float jump_Force = 5f;
    private float vertical_Velocity;

    public bool isCrouching = false;
    public bool isRunning = false;

    private float sprint_Volume = 1f;
    private float crouch_Volume = 0.1f;
    private float walk_Volume_Min = 0.2f, walk_Volume_Max = 0.6f;

    private float walk_Step_Distance = 0.4f;
    private float sprint_Step_Distance = 0.25f;
    private float crouch_Step_Distance = 0.5f;

    public void Awake()
    {
        player_footsteps = GetComponent<PlayerFootstep>();
        playerStats = GetComponent<PlayerStats>();
        character_Controller = GetComponent<CharacterController>();

    } //awake

    private void Start()
    {
        currentSpeed = walkSpeed;
        character_Controller.height = normalHeight;

        player_footsteps.volume_Min = walk_Volume_Min;
        player_footsteps.volume_Max = walk_Volume_Max;
        player_footsteps.step_Distance = walk_Step_Distance;

    }//Start



    void Update()
    {
        MoveThePlayer();
    }//Update

    void MoveThePlayer()
    {
        move_Direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        move_Direction = transform.TransformDirection(move_Direction);
        move_Direction *= currentSpeed * Time.deltaTime;

        ApplyGravity();
        if (!isCrouching)
        {
            CheckRunning();
        }
        if (!isRunning)
        {
            CheckCrouching();
        }

        character_Controller.Move(move_Direction);
    } //move the player

    private void CheckCrouching()
    {
        if (Input.GetKey(KeyCode.C))
        {
            currentSpeed = crouchSpeed;
            character_Controller.height = crouchHeight;
            isCrouching = true;

            player_footsteps.step_Distance = crouch_Step_Distance;
            player_footsteps.volume_Min = crouch_Volume;
            player_footsteps.volume_Max = crouch_Volume;
        }
        else
        {
            currentSpeed = walkSpeed;
            character_Controller.height = normalHeight;
            isCrouching = false; ;

            player_footsteps.step_Distance = walk_Step_Distance;
            player_footsteps.volume_Min = walk_Volume_Min;
            player_footsteps.volume_Max = walk_Volume_Max;
        }
    }//CheckCrouching

    private void CheckRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && character_Controller.velocity.sqrMagnitude > 0f && character_Controller.isGrounded && playerStats.canRun)
        {
            currentSpeed = runSpeed;
            isRunning = true;

            player_footsteps.step_Distance = sprint_Step_Distance;
            player_footsteps.volume_Min = sprint_Volume;
            player_footsteps.volume_Max = sprint_Volume;
        }
        else
        {
            currentSpeed = walkSpeed;
            isRunning = false;

            player_footsteps.step_Distance = walk_Step_Distance;
            player_footsteps.volume_Min = walk_Volume_Min;
            player_footsteps.volume_Max = walk_Volume_Max;
        }
    }//CheckRunning

    void ApplyGravity()
    {
        if (character_Controller.isGrounded)
        {
            vertical_Velocity -= gravity * Time.deltaTime;

            PlayerJump();

        }
        else
        {
            vertical_Velocity -= gravity * Time.deltaTime;

        }

        move_Direction.y = vertical_Velocity * Time.deltaTime;
    } // apply gravity

    void PlayerJump()
    {
        if (character_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            vertical_Velocity = jump_Force;
        }
    }//PlayerJump
}//Class
