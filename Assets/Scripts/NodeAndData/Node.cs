using UnityEngine;

public class Node : MonoBehaviour
{
    public bool HasPlayer { get; private set; } = false;
    
    [SerializeField] private string nodeName;
    
    public void OnTriggerEnter(Collider other)
    {   
        if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log("Player entered node " + name);
        HasPlayer = true;
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log("Player exited node " + name);
        HasPlayer = false;
    }

    public void LogMovement()
    {
        
    }
}


