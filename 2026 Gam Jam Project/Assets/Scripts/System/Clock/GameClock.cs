using UnityEngine;
//TMPro is using the TextMesh Pro Unity UI package
using TMPro;
//System lets us use things like Action for the clock's events
using System;

public class GameClock : MonoBehaviour
{
    //These are just for me to see the settings in the inspector instead of changing in code all the time
    [Header("Clock Settings")]
    [SerializeField] private float secondsPerHour = 60f;
    [SerializeField] private int startingHour = 12; //Having the game start at 12AM
    [SerializeField] private int busArrivalHour = 6; //The bus will arrive at 6AM

    [Header("UI")]
    [SerializeField] private TMP_Text clockText; //This is for the text object

    private float timer;
    private int currentHour;

    //This is just a bool to check if the clock should freeze or not
    private bool isPaused = false;

    //We can use this to let other events know the hour changed
    //It also lets us send the new hour to any event trigger listening
    public event Action<int> OnHourChanged;

    //Other scripts can check what time it currently is
    public int CurrentHour => currentHour;

    //Same idea here, other scripts can check whether the clock is paused
    public bool IsPaused => isPaused;

    private void Start()
    {
        currentHour = startingHour;
        UpdateClockDisplay();
    }

    private void Update()
    {
        //If the clock is paused, this stops the timer from increasing
        if (isPaused)
        {
            return;
        }

        timer += Time.deltaTime;

        //Timer to just see if enough time passes to advance the hour
        if (timer >= secondsPerHour)
        {
            //Stops frame differences from incurring time loss
            timer -= secondsPerHour;
            AdvanceHour();
        }
    }

    private void AdvanceHour()
    {
        currentHour++;

        //Wrap the clock from 12 back around to 1
        if (currentHour > 12)
        {
            currentHour = 1;
        }

        UpdateClockDisplay();

        Debug.Log("The clock advanced to " + currentHour + ":00 AM"); //Just adding debugs to see if any lines break

        //This is just to stop me from getting the NullReferenceException in case we have no events listening
        OnHourChanged?.Invoke(currentHour);

        if (currentHour == busArrivalHour)
        {
            BusArrived();
        }
    }

    private void UpdateClockDisplay()
    {
        clockText.text = currentHour + ":00 AM";
    }

    //We can use this to pause the timer when an NPC convo starts, or maybe other events, like pausing the game
    public void PauseClock()
    {
        isPaused = true;
        Debug.Log("The clock has been paused."); //Debug Assist
    }

    //Other scripts can call this when the event/conversation ends to start the pressure again
    public void ResumeClock()
    {
        isPaused = false;
        Debug.Log("The clock has been resumed."); //Debug Assist
    }

    private void BusArrived()
    {
        PauseClock();
        Debug.Log("THE BUS HAS ARRIVED!");
    }
}