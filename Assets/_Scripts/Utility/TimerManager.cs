using System;
using UnityEngine;
  
public class TimerManager : MonoBehaviour
{
    public static event Action timerRanOut;
    
    public float elapsedRunningTime;
    private float runningStartTime;
    private bool running;
    private int timerTime;

    void Update()
    {
        if (running)
        {
            elapsedRunningTime = Time.time - runningStartTime;
            isTimerUp();
        }
    }
  
    public void Begin(int timer = 0)
    {
        if (!running)
        {
            elapsedRunningTime = 0;
            runningStartTime = Time.time;
            timerTime = timer;
            running = true;
        }
    }
    public void Stop()
    {
        running = false;
        elapsedRunningTime = 0;
    }
    
    private void isTimerUp()
    {
        if (elapsedRunningTime >= timerTime)
        {
            timerRanOut?.Invoke();
            running = false;
        }
    }
}