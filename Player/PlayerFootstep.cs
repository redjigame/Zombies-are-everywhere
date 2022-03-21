using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    private CharacterController character_Controller;

    private AudioSource footstep_Sound;

    [SerializeField]
    private AudioClip[] current_Clip;

    [SerializeField]
    private AudioClip[] grass_Clip;
    [SerializeField]
    private AudioClip[] snow_Clip;

    [HideInInspector]
    public float volume_Min, volume_Max;

    private float accumulated_Distance;

    [HideInInspector]
    public float step_Distance;

    private void Awake()
    {
        footstep_Sound = GetComponent<AudioSource>();
        character_Controller = GetComponent<CharacterController>();
    }//Awake
    private void Start()
    {
        current_Clip = grass_Clip;

        StartCoroutine(FindGroundLayerWithDelay(0.5f));
    }//Start

    IEnumerator FindGroundLayerWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CheckCurrentClipListToPlay(character_Controller.transform.position);
        }
    }//FindGroundLayerWithDelay

    void Update()
    {
        CheckToPlayFootstepSound();
    } //update

    private void CheckCurrentClipListToPlay(Vector3 position)
    {
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Ray ray = new Ray(position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            string hitTag = hit.collider.gameObject.tag;
            if (hitTag == "Grass")
            {
                current_Clip = grass_Clip;
            }
            else if (hitTag == "Snow")
            {
                current_Clip = snow_Clip;
            }
        }
    }//CheckCurrentClipListToPlay

    void CheckToPlayFootstepSound()
    {
        if (!character_Controller.isGrounded)
            return;

        if (character_Controller.velocity.sqrMagnitude > 0f)
        {
            accumulated_Distance += Time.deltaTime;

            if (accumulated_Distance > step_Distance)
            {
                footstep_Sound.volume = Random.Range(volume_Min, volume_Max);
                footstep_Sound.clip = current_Clip[Random.Range(0, current_Clip.Length)];
                footstep_Sound.Play();

                accumulated_Distance = 0f;
            }
        }
        else
        {
            accumulated_Distance = 0f;
        }
    }//CheckToPlayFootstepSound
}//Class
