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
    [SerializeField] private float nodeProximity;
    [SerializeField] private float buffer = 5f;

    private readonly List<Landmark> mainQuestLandmarkSequence=new();
    private bool endGameplayLoop;
    private string sceneName;
    private bool firstTaskTriggered;

    private void Start()
    {
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
                if (AtTarget(currentQuest.landmark))
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

    private bool AtTarget(Landmark landmark)
    {
        float currentDifference = Vector3.Distance(nodeManager.ReturnPosition(landmark), player.transform.position);
        return currentDifference < nodeProximity;
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
        currentQuest.state = QuestState.Finished;
        pointToTarget.ToggleVisibility(false, nodeManager.ReturnPosition(currentQuest.landmark));
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
