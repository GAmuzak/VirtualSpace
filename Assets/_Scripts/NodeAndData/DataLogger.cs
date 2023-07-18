using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class DataLogger : SingletonMonoBehavior<DataLogger>
{
    [SerializeField] private string pathForPositionalLogs;
    [SerializeField] private string pathForNodeLogs;
    [SerializeField] private string pathForActivityLogs;
    [SerializeField] private string pathForKeyLogs;
    [SerializeField] private Transform playerTransform;
    [Range(0.1f,100)][Tooltip("Sampling rate per second")]
    [SerializeField] private float resolution;

    private bool loggingKilled;
    private float waitTime;
    private TextWriter tw;
    private string positionalFileName;
    private string nodeFileName;
    private string activityFileName;
    private string keyFileName;
    private Vector3 playerLoc;
    private Vector3 playerRot;
    private List<Array> positionalData;
    private List<Array> nodeData;
    private List<Array> activityData;
    private List<Array> keyData;
    private Camera mainCam;
    

    private void Start()
    {
        string logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logTime = logTime.Replace(" ", "-");
        logTime = logTime.Replace(":", "-");

        waitTime = 1.0f / resolution;
        positionalData = new List<Array>();
        nodeData = new List<Array>();
        activityData = new List<Array>();
        keyData = new List<Array>();
        if (!Directory.Exists(Application.persistentDataPath + "/logs"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/logs");
        }
        
        positionalFileName = Application.persistentDataPath + pathForPositionalLogs + logTime + ".csv";
        if(positionalFileName.Length>0)
        {
            tw = new StreamWriter(positionalFileName, false);
            tw.WriteLine("Time, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
            tw.Close();
        }
        
        nodeFileName = Application.persistentDataPath + pathForNodeLogs + logTime + ".csv";
        if(nodeFileName.Length>0){
            tw = new StreamWriter(nodeFileName, false);
            tw.WriteLine("Time, Node, Task, InOrOut, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
            tw.Close();
        }

        activityFileName = Application.persistentDataPath + pathForActivityLogs + logTime + ".csv";
        if(activityFileName.Length>0){
            tw = new StreamWriter(activityFileName, false);
            tw.WriteLine("Time, Location1, Location2, PassedTime, AngleOfDifference, Performance");
            tw.Close();
        }
        keyFileName = Application.persistentDataPath + pathForKeyLogs + logTime + ".csv";
        if(keyFileName.Length>0){
            tw = new StreamWriter(keyFileName, false);
            tw.WriteLine("Time, PressedButton, ButtonValue");
            tw.Close();
        }
        
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

    public void LogActivityData(string location1, string location2="", string passedTime="", string aod="", string performance="")
    {
        if (loggingKilled) return;
        string[] activityFrame = new string[6];
        activityFrame[0] = Time.timeSinceLevelLoad.ToString(CultureInfo.CurrentCulture);
        activityFrame[1] = location1;
        activityFrame[2] = location2;
        activityFrame[3] = passedTime;
        activityFrame[4] = aod;
        activityFrame[5] = performance;
        
        activityData.Add(activityFrame);
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
    public void LogKeyData(string keyName, string value)
    {
        if(loggingKilled) return;
        string[] keyFrame = new string[3];
        keyFrame[0] = Time.timeSinceLevelLoad.ToString(CultureInfo.CurrentCulture);
        keyFrame[1] = keyName;
        keyFrame[2] = ""+value;
        
        keyData.Add(keyFrame);
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
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]+","+frame[3]+","+frame[4]+","+frame[5]+","+frame[6]+","+frame[7]+","+frame[8]+","+frame[9]);
        }
        tw.Close();
        
        tw = new StreamWriter(activityFileName, true);
        foreach (Array array in activityData)
        {
            string[] frame = (string[]) array;
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]+","+frame[3]+","+frame[4]+","+frame[5]);
        }
        tw.Close();
        
        tw = new StreamWriter(keyFileName, true);
        foreach (Array array in keyData)
        {
            string[] frame = (string[]) array;
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]);
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
