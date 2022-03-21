using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    PlayerMove playerMove;

    private float endurenceMax = 100f;
    public float endurenceActual;
    private float endurenceMin = 0f;

    [Header("Unity Stuff")]
    public Image endurenceBar;

    public bool canRun;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }
    void Start()
    {
        endurenceActual = endurenceMax;
        canRun = true;
    }//Start

   
    void Update()
    {
        
        if (PauseMenu.gameIsPaused)
            return;

        EnduranceHandler();


    }

    void EnduranceHandler()
    {
        endurenceBar.fillAmount = endurenceActual / 100f;

        CheckIfRunAutorized();
        CheckBarreEndurence();



        if (playerMove.isRunning && endurenceActual > endurenceMin)
        {
            endurenceActual -= 0.25f + Time.deltaTime;
        }
        else if (!playerMove.isRunning && endurenceActual < endurenceMax)
        {
            endurenceActual += 0.01f + Time.deltaTime;
        }
    }//EnduranceHandler

    void CheckIfRunAutorized()
    {
        if (endurenceActual == endurenceMin)
        {
            StartCoroutine(canRunNew());
        }

    }//CheckIfRunAutorized

    void CheckBarreEndurence()
    {
        if (endurenceActual <= endurenceMin)
        {
            endurenceActual = endurenceMin;
        }
        else if (endurenceActual >= endurenceMax)
        {
            endurenceActual = endurenceMax;
        }
    }//CheckBarreEndurence

    IEnumerator canRunNew()
    {
        canRun = false;
        yield return new WaitForSeconds(1.5f);
        canRun = true;
    }
}//Class
