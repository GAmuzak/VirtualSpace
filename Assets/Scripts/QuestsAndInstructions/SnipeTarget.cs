using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipeTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform playerEye;

    private Vector3 targetCenter;
    private void Awake()
    {
        targetCenter = Utilities.ReturnAveragePosition(target);
    }

    private void Update()
    {
        if (!OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) return;
        CheckPerformance();
    }

    private void CheckPerformance()
    {
        Vector3 playerLookDirection = playerEye.forward;
        Vector3 targetVector = targetCenter - playerEye.position;
        float angleOfDifference = Vector3.Angle(playerLookDirection, targetVector);
        float performancePercentage = (180f - angleOfDifference)/180f * 100f;
        Debug.Log("--------------------------------------------------------------------------------------");
        Debug.Log("The angle of difference is: " + angleOfDifference + " and the performance percentage is: " + performancePercentage + "%");
        Debug.Log("--------------------------------------------------------------------------------------");

    }
}
