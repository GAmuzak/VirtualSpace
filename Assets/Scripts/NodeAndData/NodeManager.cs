using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;
    public static event Action<string> EnteredNode;


    [SerializeField] private List<Landmark> landmarkEnums;
    [SerializeField] private List<Transform> landmarkLocations;
    [SerializeField] private List<Color> landmarkColors;
    [SerializeField] private List<string> moduleInformation;

    private Dictionary<Landmark, Vector3> landMarkToTransform= new Dictionary<Landmark, Vector3>();
    private Dictionary<Landmark, Color> colorForLandmark = new Dictionary<Landmark, Color>();
    private Dictionary<Landmark, string> infoForLandmark = new Dictionary<Landmark, string>();
    private Node currentPlayerNode;
    private DataLogger dataLogger;
    private string currentTask;

    public int ModuleInfoCount => moduleInformation.Count;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dataLogger = FindObjectOfType<DataLogger>();
        
        for(int i=0; i<landmarkEnums.Count; i++)
        {
            landMarkToTransform.Add(landmarkEnums[i],TransformUtils.ReturnAveragePosition(landmarkLocations[i]));
            colorForLandmark.Add(landmarkEnums[i],landmarkColors[i]);
        }

        for (int i = 0; i < landmarkEnums.Count; i++)
        {
            string col = ColorUtility.ToHtmlStringRGB(ReturnColor(landmarkEnums[i]));
            infoForLandmark.Add(landmarkEnums[i],"The <b><color=#"+col+">"+landmarkEnums[i]+"</color></b> "+moduleInformation[i]);
        }
    }

    public void Entered(Node node)
    {
        currentPlayerNode = node;
        EnteredNode?.Invoke(currentPlayerNode.name);
        // dataLogger.LogNodeData(node.name,1, currentTask);
    }
    
    public void Exited(Node node)
    {
        if (currentPlayerNode == node)
        {
            currentPlayerNode = null;
        }
        // dataLogger.LogNodeData(node.name,0, currentTask);
    }

    public void StartDataLogging()
    {
        
    }

    public String ReturnModuleInfo(Landmark landmark)
    {
        return infoForLandmark[landmark];
    }

    public Color ReturnColor(Landmark landmark)
    {
        return colorForLandmark[landmark];
    }

    public Vector3 ReturnPosition(Landmark landmark)
    {
        return landMarkToTransform[landmark];
    }
}