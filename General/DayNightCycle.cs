
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public int dayStartTime = 240; // 4H du matin.
    public int dayEndTime = 1360; // 22H00.
    private int dayLength { get { return dayEndTime - dayStartTime; } }
    private float sunDayRotationPerMinute { get { return 180f / dayLength; } }
    private float sunNightRotationPerMinute { get { return 180f / (1440 - dayLength); } }

    //Directional lght
    [SerializeField]
    private Transform sun;

    public Text clock;

    [Range(4f, 0f)]
    public float clockSpeed = 1f;

    public int day = 1;

    [SerializeField]
    private int _timeOfDay;
    public int TimeOfDay
    {
        get { return _timeOfDay; }
        set
        {
            _timeOfDay = value;
            if (_timeOfDay > 1439)
            {
                _timeOfDay = 0;
                day++;
            }
            UpdateClock();

            float rotAmount;
            //the start of the "day" is zéro rotation on the sunLight, so that's the most straightforward.
            //calculation.
            if (_timeOfDay > dayStartTime && _timeOfDay < dayEndTime)
            {
                rotAmount = (_timeOfDay - dayStartTime) * sunDayRotationPerMinute;

            }
            //At the end of the "day" we switch to night rotation speed, but in order to keep the rotation
            //seamless, we need to account for the daytime rotation as well.
            else if (_timeOfDay >= dayEndTime)
            {
                //calculate the amount of rotation through the day so far.
                rotAmount = dayLength * sunDayRotationPerMinute;
                //Add the rotation since the end of the day.
                rotAmount += ((_timeOfDay - dayStartTime - dayLength) * sunNightRotationPerMinute);

                //Else we are at the start of a new day but because we're still in the same rotation cycle we need
                //to account for all the previous rotations since dayStartTime the previous day.
            }
            else
            {
                rotAmount = dayLength * sunDayRotationPerMinute;//previous day rotation.
                rotAmount += (1440 - dayEndTime) * sunNightRotationPerMinute;//previous night rotation.
                rotAmount += _timeOfDay * sunNightRotationPerMinute;//rotation since minuit.
            }


            sun.eulerAngles = new Vector3(rotAmount, 0f, 0f);
        }
    }//timeOfDay get

    void UpdateClock()
    {
        int hours = TimeOfDay / 60;
        int minutes = TimeOfDay - (hours * 60);

        string dayText;

        dayText = day.ToString();

        //Adding "D2" to the ToString() comand ensure that there will always be 2 digits displayed
        clock.text = string.Format("Day: {0} Time: {1}:{2}", dayText, hours.ToString("D2"), minutes.ToString("D2"));

    }//UpdateClock


    private float secondCounter = 0f;

    private void Update()
    {
        secondCounter += Time.deltaTime;
        if (secondCounter > clockSpeed)
        {
            TimeOfDay++;
            secondCounter = 0f;
        }
    }//Update

}//Class
