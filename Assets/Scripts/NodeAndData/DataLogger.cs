using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    [SerializeField] private string pathForPositionalLogs;
    [SerializeField] private string pathForNodeLogs;
    [SerializeField] private Transform playerTransform;
    [Range(0.1f,100)][Tooltip("Sampling rate per second")]
    [SerializeField] private float resolution;

    private Coroutine routine;
    private bool loggingKilled;
    private float waitTime;
    private TextWriter tw;
    private string positionalFileName;
    private string nodeFileName;
    private Vector3 playerLoc;
    private Vector3 playerRot;
    private List<Array> positionalData;
    private List<Array> nodeData;
    private Camera mainCam;
    private void Start()
    {
        waitTime = 1.0f / resolution;
        positionalData = new List<Array>();
        nodeData = new List<Array>();
        positionalFileName = Application.dataPath + pathForPositionalLogs;
        nodeFileName = Application.dataPath + pathForNodeLogs;
        tw = new StreamWriter(positionalFileName, false);
        tw.WriteLine("Time, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
        tw.Close();
        tw = new StreamWriter(nodeFileName, false);
        tw.WriteLine("Time, Node, InOrOut, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
        tw.Close();

        routine=StartCoroutine(CallLogger(waitTime));
    }

    private void Update()
    {
        playerLoc = playerTransform.position;
        playerRot = playerTransform.eulerAngles;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            KillLogging();
        }
    }

    private IEnumerator CallLogger(float timeToWait)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToWait);
            LogLocationalData();
        }
    }

    public void LogNodeData(string nodeName, int inOrOut)
    {
        if(loggingKilled) return;
        string[] nodeTriggerFrame = new string[9];
        nodeTriggerFrame[0] = Time.timeSinceLevelLoad.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[1] = nodeName;
        nodeTriggerFrame[2] = inOrOut.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[3] = playerLoc.x.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[4] = playerLoc.y.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[5] = playerLoc.z.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[6] = playerRot.x.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[7] = playerRot.y.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[8] = playerRot.z.ToString(CultureInfo.CurrentCulture);
        nodeData.Add(nodeTriggerFrame);
    }
    
    //TODO: Add way to stop logging, current is hitting space
    private void KillLogging()
    {
        if (loggingKilled) return;
        loggingKilled = true;
        StopCoroutine(routine);
        tw = new StreamWriter(positionalFileName, true);
        foreach (Array array in positionalData)
        {
            float[] frame = (float[]) array;
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]+","+frame[3]+","+frame[4]+","+frame[5]+","+frame[6]);
        }
        tw.Close();
        tw = new StreamWriter(nodeFileName, true);
        foreach (Array array in nodeData)
        {
            string[] frame = (string[]) array;
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]+","+frame[3]+","+frame[4]+","+frame[5]+","+frame[6]+","+frame[7]+","+frame[8]);
        }
        tw.Close();
        Debug.Log("-------------ended logging session-------------\nWARNING: No further information will be recorded for this session!");

    }
    
    private void LogLocationalData()
    {
        float[] singleFrame = new float[7];
        singleFrame[0] = Time.timeSinceLevelLoad;
        singleFrame[1] = playerLoc.x;
        singleFrame[2] = playerLoc.y;
        singleFrame[3] = playerLoc.z;
        singleFrame[4] = playerRot.x;
        singleFrame[5] = playerRot.y;
        singleFrame[6] = playerRot.z;
        positionalData.Add(singleFrame);
    }
}
