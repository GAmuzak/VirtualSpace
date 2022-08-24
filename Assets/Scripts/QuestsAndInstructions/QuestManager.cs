using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName;
    [SerializeField] private string mainSceneName;
    [SerializeField] private string freeplaySceneName;
    [SerializeField] private List<Landmark> tutorialLocations;

    private List<Landmark> mainQuestLandmarkInts=new List<Landmark>();
    private string sceneName;
    private bool firstTaskTriggered;
    private Quest currentQuest;
    private int locIndex;
    private void Start()
    {
        //TODO: Generate validSequence list;
        List<int> validSequence = new List<int>(){1,2,3,4,5,6};
        foreach (int num in validSequence)
        {
            mainQuestLandmarkInts.Add((Landmark)num);
        }
        firstTaskTriggered = false;
        locIndex = 0;
        StartCoroutine(SceneGrabDelay()); //just to be on the safe side
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
        InitialiseNextQuest();
    }

    private void InitialiseNextQuest()
    {
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
        
        currentQuest.state = QuestState.NotStarted;

        #region Set Landmark

        switch (currentQuest.type)
        {
            case QuestType.Tutorial:
            {
                currentQuest.landmark=GetNextLandmark(tutorialLocations);
                break;
            }
            case QuestType.FreeRoam:
            {
                //No landmarks
                break;
            }
            case QuestType.PointToTarget:
            {
                currentQuest.landmark=GetNextLandmark(mainQuestLandmarkInts);
                break;
            }
            case QuestType.NavigateToTarget:
            {
                //This will come after a Point to Target
                //so no need to change the target landmark
                break;
            }
        }

        #endregion
        
        //TODO: Set notifications
    }

    private Landmark GetNextLandmark(List<Landmark> targetList)
    {
        if(locIndex>=targetList.Count) return Landmark.NULL;
        Landmark nextLandmark=targetList[locIndex];
        locIndex++;
        return nextLandmark;
    }

    private void LogMetrics()
    {
        
    }

    private void EndCurrentQuest()
    {

    }
}
