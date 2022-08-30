using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public QuestState state;
    public QuestType type;
    public Landmark landmark;

    private int locIndex;

    private void Start()
    {
        locIndex = 0;
    }

    public Landmark GetNextLandmark(List<Landmark> targetList)
    {
        if(locIndex>=targetList.Count) return Landmark.NULL;
        Landmark nextLandmark=targetList[locIndex];
        locIndex++;
        return nextLandmark;
    }
}