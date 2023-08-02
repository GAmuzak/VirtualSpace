using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

public class CameraFrameRate: MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private List<float> frameRateList = new List<float>{60.0f, 72.0f, 90.0f, 120.0f};
    private int count = 0;
    private float deltaTime;

    private void Start()
    {
        //OVRPlugin.systemDisplayFrequency = 90.0f;

    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(frameRateList[count]);
            count += 1;
            if (count > 3)
            {
                count = 0;
            }
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps)+" - " + count;
    


    }
}
