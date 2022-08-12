using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToTarget : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Transform anchor;
    

    private Quaternion rotGoal;
    private Vector3 dirn;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition=Utilities.ReturnAveragePosition(target);
    }

    private void Update()
    {
        Vector3 playerPosition = player.position;
        dirn = (targetPosition - playerPosition).normalized;
        rotGoal = Quaternion.LookRotation(dirn);
        // anchor.RotateAround(anchor.transform.position);
        transform.position = anchor.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed * Time.deltaTime);
    }
}
