using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    
    //singleton pattern
    public static NodeManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private List<Node> nodes = new List<Node>();

    private void Start()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Node"))
        {
            try{
                nodes.Add(g.GetComponent<Node>());
            }
            catch(Exception e){
                Debug.Log(e);
            }
        }
    }

    public Node GetNodeWithPlayer()
    {
        foreach (Node node in nodes)
        {
            if(node.HasPlayer)
            {
                Node targetNode = node;
                return targetNode;
            }
        }
        return null;
    }
}