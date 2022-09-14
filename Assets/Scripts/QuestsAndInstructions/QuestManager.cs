using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Landmark> tutorialLocations;
    [SerializeField] private PointToTarget pointToTarget;
    [SerializeField] private GameObject player;
    [SerializeField] private Notification mainNotification;
    [SerializeField] private Quest currentQuest;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private List<string> sceneNames; //should ideally be decoupled, but also this looks cleaner
    [SerializeField] private List<string> moduleInformation;
    [SerializeField] private float nodeProximity;
    [SerializeField] private float buffer = 5f;
    [SerializeField] private Stopwatch timer;

    private readonly List<Landmark> mainQuestLandmarkSequence=new();
    private int currentInfoIndex;
    private bool endGameplayLoop;
    private string sceneName;
    private bool firstTaskTriggered;
    private bool playerAtTarget;

    private void Start()
    {
        NodeManager.EnteredNode += AtTarget;
        List<int> validSequence = new()
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
        timer.Reset();
        timer.Begin();
        playerAtTarget = false;
        
        #region Set Type
        if(!firstTaskTriggered)
        {
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
        #endregion
        
        #region Initialise Quest

        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                currentQuest.landmark=currentQuest.GetNextLandmark(tutorialLocations);
                if (currentQuest.landmark == Landmark.NULL) {
                    EndScene("Tutorial");
                    return;
                }
                mainNotification.UpdateText("Please go to the " + currentQuest.landmark + " by following the arrow below");
                pointToTarget.ToggleVisibility(true, nodeManager.ReturnPosition(currentQuest.landmark));
                break;
            }
            case QuestType.PointToTarget:
            {
                currentQuest.landmark=currentQuest.GetNextLandmark(mainQuestLandmarkSequence);
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

    private IEnumerator Introduction()
    {
        throw new NotImplementedException();
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
        mainNotification.UpdateText("Congratulations, you made it to the "+ currentQuest.landmark+"!" +
                                    "\nTime Taken:"+finalTime);
        StartCoroutine(ModuleInfo());
    }

    private IEnumerator ModuleInfo()
    {
        yield return new WaitForSeconds(5f);
        if(currentInfoIndex<moduleInformation.Count)
        {
            mainNotification.UpdateText(moduleInformation[currentInfoIndex]);
            currentInfoIndex++;
            yield return new WaitForSeconds(3f);
        }
        StartCoroutine(BufferToNextQuest());
    }

    private void EndScene(string questType)
    {
        mainNotification.UpdateText("Thank you for completing the " + questType + "!");
        endGameplayLoop = true;
    }

    private IEnumerator SceneGrabDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        StartCoroutine(BufferToNextQuest());
    }

    private IEnumerator BufferToNextQuest()
    {
        yield return new WaitForSeconds(buffer);
        InitialiseNextQuest();
    }
}
