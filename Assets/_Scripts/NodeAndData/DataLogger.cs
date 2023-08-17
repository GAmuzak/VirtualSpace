using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLogger : SingletonMonoBehavior<DataLogger>
{
    // IF CHANGING THE PATH THEN CHANGE IT IN UserManger.cs AS WELL
    [SerializeField] private string pathForDataLogs;
    [SerializeField] private string pathForPositionalLogs;
    [SerializeField] private string pathForNodeLogs;
    [SerializeField] private string pathForActivityLogs;
    [SerializeField] private string pathForKeyLogs;
    [SerializeField] private Transform playerTransform;
    [Range(0.1f,100)][Tooltip("Sampling rate per second")]
    [SerializeField] private float resolution;

    private bool loggingKilled;
    private float waitTime;
    private float timePassed;
    private TextWriter tw;
    private string currentUser;
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
        timePassed = 0.001f;
        if (!PlayerPrefs.HasKey("currentUser"))
        {
            Debug.Log("Error in current user not set");
        }

        currentUser = "user"+ PlayerPrefs.GetInt("currentUser").ToString();

        string logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logTime = logTime.Replace(" ", "-");
        logTime = logTime.Replace(":", "-");

        waitTime = 1.0f / resolution;
        positionalData = new List<Array>();
        nodeData = new List<Array>();
        activityData = new List<Array>();
        keyData = new List<Array>();
       
        string userName = Application.persistentDataPath + pathForDataLogs + currentUser;
        if(!Directory.Exists(userName))
        {
            Directory.CreateDirectory(userName);
        }

        if (!File.Exists(userName + "/info.txt"))
        {
            tw = new StreamWriter(userName + "/info.txt", false); 
            tw.WriteLine("SavedTime, ControlDone, TourDone, MainTaskDone");
            tw.Close();
        }
        else
        {
            string file = File.ReadAllText(userName + "/info.txt");
            string[] lines = file.Split('\n');
            float lastTimeStamp = float.Parse(lines[1].Split(',')[0]);
            timePassed = lastTimeStamp;
        }

        positionalFileName = Application.persistentDataPath + pathForDataLogs + currentUser  + pathForPositionalLogs +  SceneManager.GetActiveScene().name + ".csv";
        if(positionalFileName.Length>0 && !File.Exists(positionalFileName))
        {
            tw = new StreamWriter(positionalFileName, false);
            tw.WriteLine("Time, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
            tw.Close();
        }
        
        nodeFileName = Application.persistentDataPath + pathForDataLogs + currentUser  + pathForNodeLogs +  SceneManager.GetActiveScene().name + ".csv";
        if(nodeFileName.Length>0 && !File.Exists(nodeFileName)){
            tw = new StreamWriter(nodeFileName, false);
            tw.WriteLine("Time, Node, Task, InOrOut, Pos.x, Pos.y, Pos.z, Rot.x, Rot.y, Rot.z");
            tw.Close();
        }

        activityFileName = Application.persistentDataPath + pathForDataLogs + currentUser  + pathForActivityLogs +  SceneManager.GetActiveScene().name + ".csv";
        if(activityFileName.Length>0 && !File.Exists(activityFileName)){
            tw = new StreamWriter(activityFileName, false);
            tw.WriteLine("Time, Location1, Location2, PassedTime, AngleOfDifference, Performance, TimeTakenToComplete");
            tw.Close();
        }
        keyFileName = Application.persistentDataPath + pathForDataLogs + currentUser + pathForKeyLogs +  SceneManager.GetActiveScene().name + ".csv";
        if(keyFileName.Length>0 && !File.Exists(keyFileName)){
            tw = new StreamWriter(keyFileName, false);
            tw.WriteLine("Time, PressedButton, ButtonValue");
            tw.Close();
        }
        
        StartCoroutine(CallLogger(waitTime));
    }

    public (int, float) GetTaskDoneNTimeStamp()
    {
        float timeStamp = 0f;
        int noOfTaskDone = 0;
        string fileName = "";
        currentUser =  currentUser = "user"+ PlayerPrefs.GetInt("currentUser").ToString();
        string worldsFolder = Application.persistentDataPath + pathForDataLogs + currentUser + "\\";
        // find timestamp of last file
        if (Directory.Exists(worldsFolder))
        {
            DirectoryInfo d = new DirectoryInfo(worldsFolder);
            fileName = d.GetFiles("MainTaskActivityData*.csv")[0].ToString();
            string fileContents = File.ReadAllText(fileName);
            string[] lines = fileContents.Split("\n");

            if (lines.Length > 3)
            {
                string[] lastRow = lines[lines.Length - 2].Split(',');
                string[] secondLastRow = lines[lines.Length - 3].Split(',');

                timeStamp = float.Parse(lastRow[0]);
                noOfTaskDone = (lines.Length - 2) / 2;
                if (lastRow[1] == secondLastRow[2])
                {
                    timeStamp = float.Parse(secondLastRow[0]);
                }
            }
            else
            {
                timeStamp = 0;
                noOfTaskDone = 0;
            }
        }
        print("this -> " + noOfTaskDone + " " + timeStamp + " this");
        return (noOfTaskDone, timeStamp);
    }
    private void DeleteLastDataEntry(float timeStamp)
    {
        string worldsFolder = Application.persistentDataPath + pathForDataLogs + currentUser + "\\";
        
        // delete all the values from the files after the timestamp
        if (Directory.Exists(worldsFolder))
        {
            DirectoryInfo d = new DirectoryInfo(worldsFolder);
            FileInfo[] files = d.GetFiles("*.csv");
            foreach (FileInfo file in files)
            {

                string fileContents = File.ReadAllText(file.FullName);
                string[] lines = fileContents.Split("\n");
                string newFileContents = "";
                foreach (string line in lines)
                {
                    string[] row = line.Split(',');
                    if (row.Length > 1)
                    {
                        if (!float.TryParse(row[0], out float f) || float.Parse(row[0]) <= timeStamp)
                        {
                            
                            newFileContents += line + "\n";
                        }
                        
                    }
                }
                File.WriteAllText(file.FullName, newFileContents);
            }

        }
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
        timePassed += Time.deltaTime;
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

    public void LogActivityData(string location1, string location2="", string passedTime="", string aod="", string performance="", string timeToComplete="")
    {
        if (loggingKilled) return;
        string[] activityFrame = new string[7];
        activityFrame[0] = (timePassed).ToString(CultureInfo.CurrentCulture);
        activityFrame[1] = location1;
        activityFrame[2] = location2;
        activityFrame[3] = passedTime;
        activityFrame[4] = aod;
        activityFrame[5] = performance;
        activityFrame[6] = timeToComplete;
        
        activityData.Add(activityFrame);
    }

    public void LogNodeData(string nodeName, int inOrOut, string task)
    {
        if(loggingKilled) return;
        string[] nodeTriggerFrame = new string[10];
        nodeTriggerFrame[0] = (timePassed).ToString(CultureInfo.CurrentCulture);
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
        keyFrame[0] = (timePassed).ToString(CultureInfo.CurrentCulture);
        keyFrame[1] = keyName;
        keyFrame[2] = ""+value;
        
        keyData.Add(keyFrame);
    }
    
    public void LogInfo(string SavedTime, string ControlDone="false", string TourDone="false", string MainTaskDone="false")
    {
        tw = new StreamWriter(Application.persistentDataPath + pathForDataLogs + currentUser + "/info.txt", true);
        tw.WriteLine(SavedTime+","+ControlDone+","+TourDone+","+MainTaskDone);
        tw.Close();
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
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]+","+frame[3]+","+frame[4]+","+frame[5]+","+frame[6]);
        }
        tw.Close();
        
        tw = new StreamWriter(keyFileName, true);
        foreach (Array array in keyData)
        {
            string[] frame = (string[]) array;
            tw.WriteLine(frame[0]+","+frame[1]+","+frame[2]);
        }
        tw.Close();
        
        (int mainTaskDone, float lastSaveTime)= GetTaskDoneNTimeStamp();
        print("this -> " + mainTaskDone + " " + lastSaveTime + " this");
        DeleteLastDataEntry(lastSaveTime);
        
        tw = new StreamWriter(Application.persistentDataPath + pathForDataLogs + currentUser + "/info.txt", false);
        tw.WriteLine("LastSaveTime, IsControlDone, IsTourDone, MainTaskDone");
        tw.WriteLine(lastSaveTime+","+ false +","+false+","+mainTaskDone);
        tw.Close();
    }
    
  
    private void LogPositionalData()
    {
        float[] singleFrame = new float[7];
        singleFrame[0] = timePassed;
        singleFrame[1] = playerLoc.x;
        singleFrame[2] = playerLoc.y;
        singleFrame[3] = playerLoc.z;
        singleFrame[4] = playerRot.x;
        singleFrame[5] = playerRot.y;
        singleFrame[6] = playerRot.z;
        positionalData.Add(singleFrame);
    }
}
