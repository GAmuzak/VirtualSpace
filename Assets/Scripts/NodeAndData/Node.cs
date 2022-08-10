using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        NodeManager.Instance.Entered(this);
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        NodeManager.Instance.Exited(this);
    }
}


