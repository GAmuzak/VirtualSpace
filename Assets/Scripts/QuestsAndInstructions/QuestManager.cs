using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName;
    [SerializeField] private string mainSceneName;
    [SerializeField] private string freeplaySceneName;
    [SerializeField] private List<Landmark> tutorialLocations;
    [SerializeField] private PointToTarget pointToTarget;
    [SerializeField] private List<Transform> landmarkTransforms;
    [SerializeField] private GameObject player;
    [SerializeField] private float nodeProximity;
    [SerializeField] private Notification mainNotification;
    [SerializeField] private Quest currentQuest;
    [SerializeField] private float buffer = 5f;

    private List<Landmark> mainQuestLandmarkSequence=new();
    private Dictionary<Landmark, Vector3> landMarkToTarget=new();
    private bool endGameplayLoop;
    private string sceneName;
    private bool firstTaskTriggered;
    private int locIndex;

    private void Start()
    {
        for(int i=0; i<landmarkTransforms.Count; i++)
        {
            Debug.Log(tutorialLocations[i]+"---"+Utilities.ReturnAveragePosition(landmarkTransforms[i]));
            landMarkToTarget.Add(tutorialLocations[i],Utilities.ReturnAveragePosition(landmarkTransforms[i]));
        }
        List<int> validSequence = new(){0, 1, 2, 0, 3, 1, 0, 2, 1, 3, 0, 4, 1, 5, 2, 3, 4, 0, 5, 1, 4, 3, 5, 4, 2, 5, 3, 2, 4, 5};
        foreach (int num in validSequence)
        {
            mainQuestLandmarkSequence.Add((Landmark)num);
        }
        firstTaskTriggered = false;
        locIndex = 0;
        StartCoroutine(SceneGrabDelay());
    }

    private void Update()
    {
        if (endGameplayLoop) return;
        switch (currentQuest.state)
        {
            case QuestState.NULL:
                break;
            case QuestState.NotStarted:
                break;
            case QuestState.Active:
                if (AtTarget(currentQuest.landmark))
                {
                    EndCurrentQuest();
                }
                break;
            case QuestState.Finished:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator SceneGrabDelay()
    {
        yield return new WaitForSeconds(0.1f);
        GrabScene();
    }

    private void GrabScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        StartCoroutine(BufferTimer());
        
    }

    private void InitialiseNextQuest()
    {
        currentQuest.state = QuestState.NotStarted;
        
        #region Set Type
        if(!firstTaskTriggered)
        {
            if (sceneName == tutorialSceneName)
            {
                currentQuest.type = QuestType.Tutorial;
            }
            else if (sceneName == freeplaySceneName)
            {
                currentQuest.type = QuestType.FreeRoam;
            }
            else if (sceneName == mainSceneName)
            {
                currentQuest.type = QuestType.PointToTarget;
            }
            firstTaskTriggered = true;
        }
        #endregion
        
        #region Set Landmark

        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                TutorialLocationUpdate();
                break;
            }
            case QuestType.FreeRoam:
            {
                //No landmarks
                break;
            }
            case QuestType.PointToTarget:
            {
                currentQuest.landmark=GetNextLandmark(mainQuestLandmarkSequence);
                break;
            }
            case QuestType.NavigateToTarget:
            {
                //This will come after a Point to Target
                //so no need to change the target landmark
                break;
            }
            case QuestType.NULL:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        #endregion

        HandleActiveQuest();

    }

    private void TutorialLocationUpdate()
    {
        currentQuest.landmark = GetNextLandmark(tutorialLocations);
        if (currentQuest.landmark == Landmark.NULL) {
            EndTutorial();
            return;
        }
        mainNotification.UpdateText("Please go to the " + currentQuest.landmark + " by following the arrow below");
        pointToTarget.ToggleVisibility(true, landMarkToTarget[currentQuest.landmark]);
    }

    private Landmark GetNextLandmark(List<Landmark> targetList)
    {
        if(locIndex>=targetList.Count) return Landmark.NULL;
        Landmark nextLandmark=targetList[locIndex];
        locIndex++;
        return nextLandmark;
    }

    private bool AtTarget(Landmark landmark)
    {
        return Vector3.Distance(landMarkToTarget[landmark], player.transform.position) < nodeProximity;
    }

    private void HandleActiveQuest()
    {
        currentQuest.state = QuestState.Active;
        switch (currentQuest.type)
        {
            case QuestType.NULL:
                break;
            case QuestType.Tutorial:
                break;
            case QuestType.PointToTarget:
                break;
            case QuestType.NavigateToTarget:
                break;
            case QuestType.FreeRoam:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void EndCurrentQuest()
    {
        currentQuest.state = QuestState.Finished;
        pointToTarget.ToggleVisibility(false, landMarkToTarget[currentQuest.landmark]);
        StartCoroutine(BufferTimer());
    }

    private IEnumerator BufferTimer()
    {
        
        yield return new WaitForSeconds(buffer);
        InitialiseNextQuest();
    }

    private void EndTutorial()
    {
        mainNotification.UpdateText("Thank you for completing the tutorial!");
        endGameplayLoop = true;
    }
}
