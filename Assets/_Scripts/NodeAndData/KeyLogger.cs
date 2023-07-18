using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.XR;

public class KeyLogger : MonoBehaviour
{
    private DataLogger dataLogger;
    
    void Start()
    {
        dataLogger = FindObjectOfType<DataLogger>();
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One)){
            
            dataLogger.LogKeyData("Button.One",OVRInput.Get(OVRInput.Button.One).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.Two)){
            dataLogger.LogKeyData("Button.Two",OVRInput.Get(OVRInput.Button.Two).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.Three)){
            dataLogger.LogKeyData("Button.Three",OVRInput.Get(OVRInput.Button.Three).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.Four)){
            dataLogger.LogKeyData("Button.Four",OVRInput.Get(OVRInput.Button.Four).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.Start)){
            dataLogger.LogKeyData("Button.Start",OVRInput.Get(OVRInput.Button.Start).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.PrimaryThumbstick)){
            dataLogger.LogKeyData("Button.PrimaryThumbstick",OVRInput.Get(OVRInput.Button.PrimaryThumbstick).ToString());
        }
        if(OVRInput.Get(OVRInput.Button.SecondaryThumbstick)){
            dataLogger.LogKeyData("Button.SecondaryThumbstick",OVRInput.Get(OVRInput.Button.SecondaryThumbstick).ToString());
        }
        if(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest)){
            dataLogger.LogKeyData("Touch.PrimaryThumbRest",OVRInput.Get(OVRInput.Touch.PrimaryThumbRest).ToString());
        }
        if(OVRInput.Get(OVRInput.Touch.SecondaryThumbRest)){
            dataLogger.LogKeyData("Touch.SecondaryThumbRest",OVRInput.Get(OVRInput.Touch.SecondaryThumbRest).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0){
            dataLogger.LogKeyData("Axis1D.PrimaryIndexTrigger",OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != new Vector2(0,0)){
            dataLogger.LogKeyData("Axis2D.SecondaryThumbstick",OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).ToString());
        }
        if(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest)){
            dataLogger.LogKeyData("Touch.PrimaryThumbRest",OVRInput.Get(OVRInput.Touch.PrimaryThumbRest).ToString());
        }
        if(OVRInput.Get(OVRInput.Touch.SecondaryThumbRest)){
            dataLogger.LogKeyData("Touch.SecondaryThumbRest",OVRInput.Get(OVRInput.Touch.SecondaryThumbRest).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0){
            dataLogger.LogKeyData("Axis1D.PrimaryIndexTrigger",OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0){
            dataLogger.LogKeyData("Axis1D.SecondaryIndexTrigger",OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0){
            dataLogger.LogKeyData("Axis1D.SecondaryIndexTrigger",OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0){
            dataLogger.LogKeyData("Axis1D.SecondaryHandTrigger",OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != new Vector2(0,0)){
            dataLogger.LogKeyData("Axis2D.PrimaryThumbstick",OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).ToString());
        }
        if(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != new Vector2(0,0)){
            dataLogger.LogKeyData("Axis2D.SecondaryThumbstick",OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).ToString());
        }
    }
}
