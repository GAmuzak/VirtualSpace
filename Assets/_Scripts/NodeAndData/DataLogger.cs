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
        string logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logTime = logTime.Replace(" ", "-");
        logTime = logTime.Replace(":", "-");

        waitTime = 1.0f / resolution;
        positionalData = new List<Array>();
        nodeData = new List<Array>();
        if (!Directory.Exists(Application.persistentDataPath + "/logs"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/logs");
        }
        positionalFileName = Application.persistentDataPath + pathForPositionalLogs + logTime + ".csv";
        tw = new StreamWriter(positionalFileName, false);
        tw.WriteLine("Time, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
        tw.Close();
        nodeFileName = Application.persistentDataPath + pathForNodeLogs + logTime + ".csv";

        if(nodeFileName.Length>0){
            tw = new StreamWriter(nodeFileName, false);
            tw.WriteLine("Time, Node, Task, InOrOut, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
            tw.Close();
        }

        Debug.Log(positionalFileName);

        StartCoroutine(CallLogger(waitTime));
    }

    private void OnEnable()
    {
        QuestManager.EndGame += KillLogging;
    }

    private void OnDisable()
    {
        QuestManager.EndGame -= KillLogging;
    }

    private void Update()
    {
        playerLoc = playerTransform.position;
        playerRot = playerTransform.eulerAngles;
        if (!OVRInput.GetDown(OVRInput.Button.Start)) return;
        KillLogging();
    }

    private IEnumerator CallLogger(float timeToWait)
    {
        while (!loggingKilled)
        {
            yield return new WaitForSeconds(timeToWait);
            LogPositionalData();
        }
    }

    public void LogNodeData(string nodeName, int inOrOut, string task)
    {
        if(loggingKilled) return;
        string[] nodeTriggerFrame = new string[10];
        nodeTriggerFrame[0] = Time.timeSinceLevelLoad.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[1] = nodeName;
        nodeTriggerFrame[2] = task;
        nodeTriggerFrame[3] = inOrOut.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[4] = playerLoc.x.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[5] = playerLoc.y.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[6] = playerLoc.z.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[7] = playerRot.x.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[8] = playerRot.y.ToString(CultureInfo.CurrentCulture);
        nodeTriggerFrame[9] = playerRot.z.ToString(CultureInfo.CurrentCulture);
        nodeData.Add(nodeTriggerFrame);
    }
    
    private void KillLogging()
    {
        if (loggingKilled) return;
        loggingKilled = true;
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
    }
    
    private void LogPositionalData()
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
