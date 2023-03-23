using System;
using UnityEngine;
  
public class Stopwatch : MonoBehaviour
{
    private float elapsedRunningTime;
    private float runningStartTime;
    private float pauseStartTime;
    private float elapsedPausedTime;
    private float totalElapsedPausedTime;
    private bool running;
    private bool paused;

    void Update()
    {
        if (running)
        {
            elapsedRunningTime = Time.time - runningStartTime - totalElapsedPausedTime;
        }
        else if (paused)
        {
            elapsedPausedTime = Time.time - pauseStartTime;
        }
    }
  
    public void Begin()
    {
        if (!running && !paused)
        {
            runningStartTime = Time.time;
            running = true;
        }
    }
  
    public void Pause()
    {
        if (running && !paused)
        {
            running = false;
            pauseStartTime = Time.time;
            paused = true;
        }
    }
  
    public void Unpause()
    {
        if (!running && paused)
        {
            totalElapsedPausedTime += elapsedPausedTime;
            running = true;
            paused = false;
        }
    }
  
    public void Reset()
    {
        elapsedRunningTime = 0f;
        runningStartTime = 0f;
        pauseStartTime = 0f;
        elapsedPausedTime = 0f;
        totalElapsedPausedTime = 0f;
        running = false;
        paused = false;
    }
  
    public int GetMinutes()
    {
        return (int)(elapsedRunningTime / 60f);
    }
  
    public int GetSeconds()
    {
        return (int)(elapsedRunningTime);
    }
  
    public float GetMilliseconds()
    {
        return (float)(elapsedRunningTime - System.Math.Truncate(elapsedRunningTime));
    }
 
    public float GetRawElapsedTime()
    {
        return elapsedRunningTime;
    }
}