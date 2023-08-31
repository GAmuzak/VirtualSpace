using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    LocationPointer locationPointer;
    private void Start()
    {
        locationPointer = GetComponent<LocationPointer>();
    }

    public void DisplayStar(Vector3 position, bool hide=false)
    {
        if(hide == true)
        {            
            locationPointer.ToggleVisibility(false);
        }
        else
        {
            locationPointer.ToggleVisibility(true);
            transform.position = position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        DisplayStar(Vector3.zero, true);
    }
}
