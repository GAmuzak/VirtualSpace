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

    private void Start()
    {
        
        for(int i=0; i<landmarkEnums.Count; i++)
        {
            landMarkToTransform.Add(landmarkEnums[i],Utilities.ReturnAveragePosition(landmarkLocations[i]));
        }
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        dataLogger = FindObjectOfType<DataLogger>();
    }

    public void Entered(Node node)
    {
        currentPlayerNode = node;
        dataLogger.LogNodeData(node.name,1);
    }
    
    public void Exited(Node node)
    {
        if (currentPlayerNode == node)
        {
            currentPlayerNode = null;
        }
        dataLogger.LogNodeData(node.name,0);
    }

    public Vector3 ReturnPosition(Landmark landmark)
    {
        return landMarkToTransform[landmark];
    }
}