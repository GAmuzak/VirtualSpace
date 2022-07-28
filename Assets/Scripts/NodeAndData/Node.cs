using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool HasPlayer { get; private set; } = false;
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        NodeManager.Instance.Entered(this);
        HasPlayer = true;
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        NodeManager.Instance.Exited(this);
        HasPlayer = false;
    }
}


