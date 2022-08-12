using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Vector3 ReturnAveragePosition(Transform parent)
    {
        Vector3 averagePosition = Vector3.zero;
        int numberOfChildren = parent.childCount;
        if (numberOfChildren <= 0) return parent.position;
        for (int i = 0; i < numberOfChildren; i++)
        {
            averagePosition += parent.GetChild(i).position;
        }
        averagePosition /= numberOfChildren;
        return averagePosition;
    }
}
