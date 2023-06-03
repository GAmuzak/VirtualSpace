using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static event Action EndGame; 

    #region SFields

    [SerializeField] private List<Landmark> tutorialLocations;
    [SerializeField] private PointToTarget pointToTarget;
    [SerializeField] private Notification mainNotification;
    [SerializeField] private Quest currentQuest;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private SnipeTarget snipeTarget;
    [SerializeField] private List<string> sceneNames; //should ideally be decoupled, but also this looks cleaner
    [SerializeField] private List<string> introductions;
    [SerializeField] private List<string> mainTaskInstructions;
    [SerializeField] private float buffer = 5f;
    [SerializeField] private float moduleInfoTimer;
    [SerializeField] private Stopwatch timer;

    #endregion

    #region FieldVars

    private readonly List<Landmark> mainQuestLandmarkSequence= new List<Landmark>();
    private Landmark previousLandmark = Landmark.Airlock;
    private int currentInfoIndex;
    private bool endGameplayLoop;
    private string sceneName;
    private bool firstTaskTriggered;
    private bool playerAtTarget;
    private int introIndex;
    private bool hasTriggered;
    
    #endregion

    private void Start()
    {
        NodeManager.EnteredNode += AtTarget;
        List<int> validSequence = new List<int>
        {
            0, 1, 2, 0, 3, 1, 0, 2, 1, 3, 0, 4, 1, 5, 2, 3, 4, 0, 5, 1, 4, 3, 5, 4, 2, 5, 3, 2, 4, 5
        }; //Note: getting people to replay the main quest might lead them to figure out patterns, but it's unlikely
        foreach (int num in validSequence)
        {
            mainQuestLandmarkSequence.Add((Landmark)num);
        }
        firstTaskTriggered = false;
        StartCoroutine(SceneGrabDelay());
    }

    private void OnDisable()
    {
        Notification.NotificationDismissed -= IntroductionsLoop;
        Notification.NotificationDismissed -= MainTaskLoop;
        SnipeTarget.Sniped -= OnSniped;
    }

    private void Update()
    {
        if (endGameplayLoop || currentQuest.state!=QuestState.Active) return;
        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                if (playerAtTarget)
                {
                    EndCurrentQuest();
                }
                break;
            }
            case QuestType.NavigateToTarget:
            {
                if (playerAtTarget)
                {
                    StartCoroutine(SwitchToNextPointTask());
                }
                break;
            }
        }
    }

    private void InitialiseNextQuest()
    {
        currentQuest.state = QuestState.NotStarted;

        FirstSetup();
        
        #region Initialise Quest
        
        playerAtTarget = false;
        
        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                timer.Reset();
                timer.Begin();
                previousLandmark = currentQuest.landmark;
                currentQuest.landmark=currentQuest.GetNextLandmark(tutorialLocations);
                if (currentQuest.landmark == Landmark.NULL) {
                    EndScene("ISS Tour");
                    return;
                }
                string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
                mainNotification.UpdateText("Please go to the <b><color=#"+col+">" + currentQuest.landmark + "</color></b> by following the arrow below", 1, 1);
                pointToTarget.ToggleVisibility(true, nodeManager.ReturnPosition(currentQuest.landmark));
                break;
            }
            case QuestType.PointToTarget:
            {
                SnipeTarget.Sniped += OnSniped;
                previousLandmark = currentQuest.landmark;
                currentQuest.landmark=currentQuest.GetNextLandmark(mainQuestLandmarkSequence);
                string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
                mainNotification.UpdateText("Please point at the <b><color=#"+col+">" + currentQuest.landmark + "</color></b>", 2, 2);
                snipeTarget.SetTarget(NodeManager.Instance.ReturnPosition(currentQuest.landmark));
                if (currentQuest.landmark == Landmark.NULL) {
                    EndScene("Main Task");
                    return;
                }
                break;
            }
        }

        #endregion

        currentQuest.state = QuestState.Active;
    }

    private void FirstSetup()
    {
        if (firstTaskTriggered) return;
        
        if (sceneName == sceneNames[0])
        {
            currentQuest.type = QuestType.Tutorial;
            SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = false;
            SimpleCapsuleWithStickMovement.Instance.EnableRotation = false;
            Notification.NotificationDismissed += IntroductionsLoop;
            IntroductionsLoop();
        }
        else if (sceneName == sceneNames[1])
        {
            currentQuest.type = QuestType.PointToTarget;
            SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = false;
            SimpleCapsuleWithStickMovement.Instance.EnableRotation = false;
            Notification.NotificationDismissed += MainTaskLoop;
            MainTaskLoop();
        }
        else if (sceneName == sceneNames[2])
        {
            currentQuest.type = QuestType.FreeRoam;
        }
        firstTaskTriggered = true;
    }

    private void EndCurrentQuest()
    {
        timer.Pause();
        currentQuest.state = QuestState.Finished;
        pointToTarget.ToggleVisibility(false, nodeManager.ReturnPosition(currentQuest.landmark));
        string finalTime = $"{timer.GetRawElapsedTime():0.##}";
        string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
        mainNotification.UpdateText("Congratulations, you made it to the <color=#"+col+">"+ currentQuest.landmark+"</color>!",1 , 3);
        string task = "from" + previousLandmark + "to" + currentQuest.landmark;
        DataLogger.Instance.LogActivityData(task, finalTime);
        StartCoroutine(ModuleInfo());
    }

    private void EndScene(string questType)
    {
        mainNotification.UpdateText("Thank you for completing the " + questType + "!", 1,1);
        endGameplayLoop = true;
        EndGame?.Invoke();
    }

    private IEnumerator SwitchToNextPointTask()
    {
        timer.Pause();
        currentQuest.state = QuestState.Finished;
        SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = false;
        string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
        mainNotification.UpdateText("Congratulations, you made it to the <color=#"+col+">"+ currentQuest.landmark+"</color>!", 1,2);
        string finalTime = $"{timer.GetRawElapsedTime():0.##}";
        string task = "from" + previousLandmark + "to" + currentQuest.landmark;
        DataLogger.Instance.LogActivityData(task, finalTime);
        yield return new WaitForSeconds(3f);
        currentQuest.type = QuestType.PointToTarget;
        InitialiseNextQuest();
    }

    private IEnumerator SwitchToNavigation()
    {
        yield return new WaitForSeconds(3f);
        timer.Reset();
        timer.Begin();
        currentQuest.type = QuestType.NavigateToTarget;
        SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = true;
        string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
        mainNotification.UpdateText("Please go to the <b><color=#"+col+">" + currentQuest.landmark + "</color></b>", 2, 2);
    }

    private void MainTaskLoop()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(MainTaskInstructionPause());
    }

    private IEnumerator MainTaskInstructionPause()
    {
        yield return new WaitForSeconds(3f);
        hasTriggered = false;
        if(introIndex<mainTaskInstructions.Count)
        {
            mainNotification.UpdateText(mainTaskInstructions[introIndex], introIndex+1, mainTaskInstructions.Count);
        }
        introIndex++;
        if(introIndex>mainTaskInstructions.Count)
        {
            Notification.NotificationDismissed -= MainTaskLoop;
            SimpleCapsuleWithStickMovement.Instance.EnableRotation = true;
            StartCoroutine(BufferToNextQuest());
        }
    }

    private void IntroductionsLoop()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(IntroPause());
    }

    private IEnumerator IntroPause()
    {
        yield return new WaitForSeconds(3f);
        hasTriggered = false;
        if(introIndex<introductions.Count)
        {
            mainNotification.UpdateText(introductions[introIndex], introIndex+1, introductions.Count+1);
        }
        else if (introIndex == introductions.Count)
        {
            NextModuleInfo(Landmark.Airlock, introIndex+1, introductions.Count+1);
        }
        introIndex++;
        if(introIndex>introductions.Count)
        {
            Notification.NotificationDismissed -= IntroductionsLoop;
            SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = true;
            SimpleCapsuleWithStickMovement.Instance.EnableRotation = true;
            StartCoroutine(BufferToNextQuest());
        }
    }

    private void NextModuleInfo(Landmark landmark, int index, int totalCount)
    {
        mainNotification.UpdateText(NodeManager.Instance.ReturnModuleInfo(landmark), index, totalCount);
        currentInfoIndex++;
    }

    private void AtTarget(string landmark)
    {
        playerAtTarget = string.Equals(landmark, currentQuest.landmark.ToString());
    }

    private void OnSniped(float angleOfDifference, float performancePercentage)
    {
        SnipeTarget.Sniped -= OnSniped;
        string task= "PointToTarget:"+currentQuest.landmark;
        string data = "Angle of Difference: " + angleOfDifference + ", Performance:" + performancePercentage+"%";
        DataLogger.Instance.LogActivityData(task, data);
        // mainNotification.UpdateText("AOD:"+angleOfDifference+";\n Perf:"+performancePercentage+"%" ,1, 2);
        mainNotification.UpdateText("Thank you" ,1, 2);
        StartCoroutine(SwitchToNavigation());
    }

    private IEnumerator ModuleInfo()
    {
        if(currentInfoIndex<NodeManager.Instance.ModuleInfoCount)
        {
            yield return new WaitForSeconds(5f);
            NextModuleInfo(currentQuest.landmark, 2, 3);
            yield return new WaitForSeconds(moduleInfoTimer);
            mainNotification.UpdateText("Press A to dismiss this notification\nwhen you are ready to move on, press A again", 3, 3);
            Notification.NotificationDismissed += OnObservationStarted;
        }
        else
        {
            StartCoroutine(BufferToNextQuest());
        }
        
    }

    private void OnObservationStarted()
    {
        Notification.NotificationDismissed -= OnObservationStarted;
        Debug.Log("Balls");
        Notification.Pain += OnObservationFinished;
    }

    private void OnObservationFinished()
    {
        Notification.Pain -= OnObservationFinished;
        Debug.Log("Balls2");
        InitialiseNextQuest();
    }

    private IEnumerator SceneGrabDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        FirstSetup();
    }

    private IEnumerator BufferToNextQuest()
    {
        yield return new WaitForSeconds(buffer);
        InitialiseNextQuest();
    }
}
