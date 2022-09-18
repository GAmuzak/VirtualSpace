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
    [SerializeField] private GameObject player;
    [SerializeField] private Notification mainNotification;
    [SerializeField] private Quest currentQuest;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private SnipeTarget snipeTarget;
    [SerializeField] private List<string> sceneNames; //should ideally be decoupled, but also this looks cleaner
    [SerializeField] private List<string> introductions;
    [SerializeField] private float nodeProximity;
    [SerializeField] private float buffer = 5f;
    [SerializeField] private Stopwatch timer;

    #endregion

    #region FieldVars

    private readonly List<Landmark> mainQuestLandmarkSequence= new List<Landmark>();
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

    private void OnEnable()
    {
        Notification.NotificationDismissed += IntroductionsLoop;
    }

    private void OnDisable()
    {
        Notification.NotificationDismissed -= IntroductionsLoop;
    }

    private void Update()
    {
        if (endGameplayLoop || currentQuest.state!=QuestState.Active) return;
        switch (currentQuest.type)
        {
            case QuestType.Tutorial or QuestType.NavigateToTarget:
            {
                if (playerAtTarget)
                {
                    EndCurrentQuest();
                }
                break;
            }
            case QuestType.PointToTarget:
                
                break;
        }
    }

    private void InitialiseNextQuest()
    {
        currentQuest.state = QuestState.NotStarted;

        FirstSetup();
        
        #region Initialise Quest

        timer.Reset();
        timer.Begin();
        playerAtTarget = false;
        
        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                currentQuest.landmark=currentQuest.GetNextLandmark(tutorialLocations);
                if (currentQuest.landmark == Landmark.NULL) {
                    EndScene("ISS Tour");
                    return;
                }
                string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
                mainNotification.UpdateText("Please go to the <b><color=#"+col+">" + currentQuest.landmark + "</color></b> by following the arrow below");
                pointToTarget.ToggleVisibility(true, nodeManager.ReturnPosition(currentQuest.landmark));
                break;
            }
            case QuestType.PointToTarget:
            {
                currentQuest.landmark=currentQuest.GetNextLandmark(mainQuestLandmarkSequence);
                string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
                mainNotification.UpdateText("Please point at the <b><color=#"+col+">" + currentQuest.landmark + "</color></b>");
                snipeTarget.SetTarget(NodeManager.Instance.ReturnPosition(currentQuest.landmark));
                if (currentQuest.landmark == Landmark.NULL) {
                    EndScene("Main Task");
                    return;
                }
                break;
            }
        }

        #endregion

        HandleActiveQuest();
    }

    private void FirstSetup()
    {
        if (firstTaskTriggered) return;
        
        if (sceneName == sceneNames[0])
        {
            currentQuest.type = QuestType.Tutorial;
        }
        else if (sceneName == sceneNames[1])
        {
            currentQuest.type = QuestType.FreeRoam;
        }
        else if (sceneName == sceneNames[2])
        {
            currentQuest.type = QuestType.PointToTarget;
        }
        firstTaskTriggered = true;
    }

    private void IntroductionsLoop()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(IntroPause());
    }

    private void NextModuleInfo(Landmark landmark)
    {
        mainNotification.UpdateText(NodeManager.Instance.ReturnModuleInfo(landmark));
        currentInfoIndex++;
    }

    private void AtTarget(string landmark)
    {
        playerAtTarget = string.Equals(landmark, currentQuest.landmark.ToString());
    }

    private void HandleActiveQuest()
    {
        currentQuest.state = QuestState.Active;
        
        switch (currentQuest.type)
        {
            case QuestType.PointToTarget:
                break;
            case QuestType.NavigateToTarget:
                break;
        }
    }

    private void EndCurrentQuest()
    {
        timer.Pause();
        currentQuest.state = QuestState.Finished;
        pointToTarget.ToggleVisibility(false, nodeManager.ReturnPosition(currentQuest.landmark));
        string finalTime = $"{timer.GetRawElapsedTime():0.##}";
        string col = ColorUtility.ToHtmlStringRGB(NodeManager.Instance.ReturnColor(currentQuest.landmark));
        mainNotification.UpdateText("Congratulations, you made it to the <color=#"+col+">"+ currentQuest.landmark+"</color>!" +
                                    "\nTime Taken: "+finalTime);
        StartCoroutine(ModuleInfo());
    }

    private void EndScene(string questType)
    {
        mainNotification.UpdateText("Thank you for completing the " + questType + "!");
        endGameplayLoop = true;
        EndGame?.Invoke();
        
    }

    private IEnumerator IntroPause()
    {
        yield return new WaitForSeconds(3f);
        hasTriggered = false;
        if(introIndex<introductions.Count)
        {
            mainNotification.UpdateText(introductions[introIndex]);
        }
        else if (introIndex == introductions.Count)
        {
            NextModuleInfo(Landmark.Airlock);
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

    private IEnumerator ModuleInfo()
    {
        if(currentInfoIndex<NodeManager.Instance.ModuleInfoCount)
        {
            yield return new WaitForSeconds(3f);
            NextModuleInfo(currentQuest.landmark);
            yield return new WaitForSeconds(5f);
        }
        StartCoroutine(BufferToNextQuest());
    }

    private IEnumerator SceneGrabDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        SimpleCapsuleWithStickMovement.Instance.EnableLinearMovement = false;
        SimpleCapsuleWithStickMovement.Instance.EnableRotation = false;
        IntroductionsLoop();
    }

    private IEnumerator BufferToNextQuest()
    {
        
        yield return new WaitForSeconds(buffer);
        InitialiseNextQuest();
    }
}
