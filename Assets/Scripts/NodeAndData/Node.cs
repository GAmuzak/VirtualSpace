using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool HasPlayer { get; private set; } = false;

    private void Awake()
    {
        NodeManager nodeManager = NodeManager.instance;
    }

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
}


