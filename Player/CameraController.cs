using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerRoot, lookRoot;

    //[SerializeField]
    // private bool Invert;

    [SerializeField]
    private bool Can_Unlock;

    [SerializeField]
    private float Sensivity = 5f;

    [SerializeField]
    private float roll_Angle = 0f;

    [SerializeField]
    private float roll_Speed = 0f;

    [SerializeField]
    private Vector2 default_Look_Limit = new Vector2(-70f, 80f);

    private Vector2 look_Angles;

    private Vector2 current_Mous_Look;

    private float current_Roll_Angle;


    public bool Invert { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        LockAndUnlockCursor();
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (!PauseMenu.gameIsPaused)
            {
                lookArround();
            }
        }
    } //update

    void LockAndUnlockCursor()
    {
        if (!PauseMenu.gameIsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    } //lock and unlock

    void lookArround()
    {
        current_Mous_Look = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        look_Angles.x += current_Mous_Look.x * Sensivity * (Invert ? 1f : -1f);
        look_Angles.y += current_Mous_Look.y * Sensivity;

        look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limit.x, default_Look_Limit.y);

        current_Roll_Angle = Mathf.Lerp(current_Roll_Angle, Input.GetAxisRaw("Mouse X") * roll_Angle, Time.deltaTime * roll_Speed);

        lookRoot.localRotation = Quaternion.Euler(look_Angles.x, 0f, current_Roll_Angle);
        playerRoot.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f);
    } // look arround
}//Class
