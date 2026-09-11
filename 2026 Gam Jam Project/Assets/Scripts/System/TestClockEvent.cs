using UnityEngine;

public class TestClockEvent : MonoBehaviour
{
    //This gives us a place in the Inspector to connect our GameClock
    [SerializeField] private GameClock gameClock;

    private void OnEnable()
    {
        //Start listening for the clock's OnHourChanged event
        gameClock.OnHourChanged += HandleHourChanged;
    }

    private void OnDisable()
    {
        //Stop listening when this object is disabled
        gameClock.OnHourChanged -= HandleHourChanged;
    }

    private void HandleHourChanged(int hour)
    {
        //Whenever the clock changes, check if it has reached 2AM
        if (hour == 2)
        {
            Debug.Log("TEST EVENT: Something spooky happened at 2:00 AM!");
        }
    }
}