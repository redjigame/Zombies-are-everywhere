using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaFootstep : MonoBehaviour
{
    AudioSource audioSource;
    public Transform leftFoot;
    public Transform rightFoot;

    [SerializeField] List<AudioClip> footStepsGrassSound = new List<AudioClip>();
    [SerializeField] List<AudioClip> footStepsSnowSound = new List<AudioClip>();
    [SerializeField] List<AudioClip> footStepsConcreteSound = new List<AudioClip>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CharacterFootstepSound(int footIndex)
    {
        if (footIndex == 0)
        {
            RaycastTerrain(leftFoot.position);
        }
        else if (footIndex == 1)
        {
            RaycastTerrain(rightFoot.position);
        }

    }//PlayerFootstepSound

    private void RaycastTerrain(Vector3 position)
    {
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Ray ray = new Ray(position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            string hitTag = hit.collider.gameObject.tag;
            if (hitTag == "Grass")
            {
                PlayRandomSound(footStepsGrassSound);
            }
            else if (hitTag == "Smow")
            {
                PlayRandomSound(footStepsSnowSound);

            }
            else if (hitTag == "Concrete")
            {
                PlayRandomSound(footStepsConcreteSound);
            }
        }
    }//RaycastTerrain

    void PlayRandomSound(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0 && audioSource)
        {
            int randomNum = UnityEngine.Random.Range(0, audioClips.Count - 1);
            audioSource.PlayOneShot(audioClips[randomNum]);
        }
    }//PlayRandomSound
}//Class
