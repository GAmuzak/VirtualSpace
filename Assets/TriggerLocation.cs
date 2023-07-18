using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLocation : MonoBehaviour
{
    [SerializeField] private Node node;
 
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        NodeManager.Instance.EnteredInnerNode(node);
    }
}
