using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    [SerializeField] private List<Landmark> landmarkEnums;
    [SerializeField] private List<Transform> landmarkLocations;

    private Dictionary<Landmark, Vector3> landMarkToTransform=new();
    private Node currentPlayerNode;
    private DataLogger dataLogger;
    private string currentTask;

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
            landMarkToTransform.Add(landmarkEnums[i],Utilities.ReturnAveragePosition(landmarkLocations[i]));
        }
    }

    public void Entered(Node node)
    {
        currentPlayerNode = node;
        dataLogger.LogNodeData(node.name,1, currentTask);
    }
    
    public void Exited(Node node)
    {
        if (currentPlayerNode == node)
        {
            currentPlayerNode = null;
        }
        dataLogger.LogNodeData(node.name,0, currentTask);
    }

    public void StartDataLogging()
    {
        
    }

    public Vector3 ReturnPosition(Landmark landmark)
    {
        return landMarkToTransform[landmark];
    }
}