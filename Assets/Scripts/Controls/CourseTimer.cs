using System;
using UnityEngine;
using System.Diagnostics;

public class CourseTimer : MonoBehaviour
{
    public static event Action<string> OnEnter;
    
    private bool hasStarted;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnEnter?.Invoke(gameObject.name);
        }
    }
}
