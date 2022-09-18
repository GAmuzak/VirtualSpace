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


    // private void Awake()
    // {
    //     targetCenter = TransformUtils.ReturnAveragePosition(target);
    // }


    private void Update()
    {
        if (!taskActive) return;
        if (!OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) return;
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
        // Debug.Log("--------------------------------------------------------------------------------------");
        // Debug.Log("The angle of difference is: " + angleOfDifference + " and the performance percentage is: " + performancePercentage + "%");
        // Debug.Log("--------------------------------------------------------------------------------------");

    }
}
