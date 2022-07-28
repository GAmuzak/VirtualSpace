using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    private Node currentPlayerNode;
    private DataLogger dataLogger;

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
}