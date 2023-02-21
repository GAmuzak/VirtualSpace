using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsRunManager : MonoBehaviour
{
    [SerializeField] private Notification mainNotif;
    [SerializeField] private List<string> notifications;
    [SerializeField] private GameObject start;
    [SerializeField] private Material lighterMat;

    private Stopwatch timer;
    private int notifIndex;
    private bool alreadyTriggered;

    private void Start()
    {
        timer = FindObjectOfType<Stopwatch>();
        TutorialLoop();
    }

    private void OnEnable()
    {
        CourseTimer.OnEnter += TimeManager;
        Notification.NotificationDismissed += TutorialLoop;
    }

    private void OnDisable()
    {
        CourseTimer.OnEnter -= TimeManager;
        Notification.NotificationDismissed -= TutorialLoop;
    }

    private void Update()
    {
        if (!OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) ||
            !OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) return;
        SkipTutorial();
    }

    public void SkipTutorial()
    {
        notifIndex = -1;
        // mainNotif.UpdateText("Look at this speedrunner sheesh");
        EndTutorial();
    }

    private void TutorialLoop()
    {
        if (alreadyTriggered) return;
        alreadyTriggered = true;
        StartCoroutine(NextNotifDelay());
    }

    private IEnumerator NextNotifDelay()
    {
        yield return new WaitForSeconds(2f);
        alreadyTriggered = false;
        mainNotif.UpdateText(notifications[notifIndex]);
        if (notifIndex + 1 < notifications.Count)
            notifIndex++;
        else
        {
            EndTutorial();
        }
    }

    private void EndTutorial()
    {
        start.GetComponent<Collider>().isTrigger = true;
        start.GetComponent<MeshRenderer>().material = lighterMat;
        Notification.NotificationDismissed -= TutorialLoop;
    }

    private void TimeManager(string objName)
    {
        if(objName=="Start") timer.Begin();
        else
        {
            timer.Pause();
            DisplayStats();
        }
    }

    private void DisplayStats()
    {
        string finalTime = $"{timer.GetRawElapsedTime():0.##}";
        mainNotif.UpdateText("Time: "+finalTime);
    }
}
