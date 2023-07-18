using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipeTarget : MonoBehaviour
{
    public static event Action<float, float> Sniped;
    
    [SerializeField] private Transform playerEye;

    private Vector3 targetCenter;
    private float angleOfDifference;
    private float performancePercentage;
    private bool taskActive;

    private void Update()
    {
        if (!taskActive) return;
        if (!OVRInput.GetDown(OVRInput.Button.Three)) return;
        CheckPerformance();
        Sniped?.Invoke(angleOfDifference, performancePercentage);
        taskActive = false;
    }

    public void SetTarget(Vector3 target)
    {
        targetCenter = target;
        taskActive = true;
        
    }

    private void CheckPerformance()
    {
        Vector3 playerLookDirection = playerEye.forward;
        Vector3 targetVector = targetCenter - playerEye.position;
        angleOfDifference = Vector3.Angle(playerLookDirection, targetVector);
        performancePercentage = (180f - angleOfDifference)/180f * 100f;
        performancePercentage = (float)Math.Round(performancePercentage * 100f) / 100f;
        angleOfDifference = (float)Math.Round(angleOfDifference * 100f) / 100f;
    }
}
