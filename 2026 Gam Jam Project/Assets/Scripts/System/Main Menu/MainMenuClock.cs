using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenuClock : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text clockText;

    [Header("Fast Forward Settings")]
    [SerializeField] private float fastForwardSpeed = 14400f;
    [SerializeField] private float slowForwardSpeed = 120f;
    [SerializeField] private float slowdownMinutes = 30f;

    [Header("Clock Movement")]
    [SerializeField] private RectTransform clockRectTransform;
    [SerializeField] private Vector2 targetPosition = new Vector2(100f, 80f);

    [Header("Scene Transition")]
    [SerializeField] private string gameSceneName = "Game";

    private DateTime displayedTime;
    private DateTime transitionStartTime;
    private DateTime targetMidnight;

    private Vector2 startingPosition;

    private bool isFastForwarding = false;

    private void Start()
    {
        displayedTime = DateTime.Now;

        //Remember where the clock starts on the menu
        startingPosition = clockRectTransform.anchoredPosition;

        UpdateClockDisplay();
    }

    private void Update()
    {
        if (!isFastForwarding)
        {
            //Keep player PC time current
            displayedTime = DateTime.Now;
            UpdateClockDisplay();
            return;
        }

        FastForwardClock();
        MoveClock();
    }

    //Play Button
    public void StartGameTransition()
    {
        if (isFastForwarding)
        {
            return;
        }

        //Player Computer time
        displayedTime = DateTime.Now;

        //Remember exactly when the transition started
        transitionStartTime = displayedTime;

        //Calculate time to next 12:00AM
        targetMidnight = displayedTime.Date.AddDays(1);

        isFastForwarding = true;

        Debug.Log("Fast forwarding clock to midnight.");
    }

    private void FastForwardClock()
    {
        //Check time to midnight
        TimeSpan timeRemaining = targetMidnight - displayedTime;

        float currentSpeed = fastForwardSpeed;

        //Slow down ticks
        if (timeRemaining.TotalMinutes <= slowdownMinutes)
        {
            float slowdownProgress =
                1f - ((float)timeRemaining.TotalMinutes / slowdownMinutes);

            currentSpeed = Mathf.Lerp(
                fastForwardSpeed,
                slowForwardSpeed,
                slowdownProgress
            );
        }

        //Speed up fake transition clock
        displayedTime = displayedTime.AddSeconds(
            currentSpeed * Time.deltaTime
        );

        //Stop at 12:00AM
        if (displayedTime >= targetMidnight)
        {
            displayedTime = targetMidnight;

            //Make absolutely sure the clock reaches its final position
            MoveClock();

            isFastForwarding = false;

            UpdateClockDisplay();

            Debug.Log("Clock reached midnight!");

            SceneManager.LoadScene(gameSceneName);
            return;
        }

        UpdateClockDisplay();
    }

    private void MoveClock()
    {
        //Total fake time between pressing Play and midnight
        double totalSeconds =
            (targetMidnight - transitionStartTime).TotalSeconds;

        //How much fake time has passed so far
        double elapsedSeconds =
            (displayedTime - transitionStartTime).TotalSeconds;

        //Compress progress to value
        float progress = (float)(elapsedSeconds / totalSeconds);

        progress = Mathf.Clamp01(progress);

        //Lerp changes depending on player clock to midnight
        clockRectTransform.anchoredPosition = Vector2.Lerp(
            startingPosition,
            targetPosition,
            progress
        );
    }

    private void UpdateClockDisplay()
    {
        clockText.text = displayedTime.ToString("h:mm tt");
    }
}