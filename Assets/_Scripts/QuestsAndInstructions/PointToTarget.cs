using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

 public class PointToTarget : MonoBehaviour
 {
     [SerializeField] private Transform player;
     [SerializeField] private float turnSpeed;
     [SerializeField] private Transform anchor;
     [SerializeField] private QuestManager questManager;

     private Landmark nextLandmark;
     private Quaternion rotGoal;
     private Vector3 dirn;
     private Vector3 targetPosition;
     private int[] arr = new int[9];
     private bool active;

     private void Start()
     {
         targetPosition = Vector3.zero;
         ToggleVisibility(false, Landmark.Cupola, 0f);
     }

     private void Update()
     {
         if (!active) return;
         Vector3 playerPosition = player.position;
         dirn = (targetPosition - playerPosition).normalized;
         rotGoal = Quaternion.LookRotation(dirn);
         transform.position = anchor.position;
         transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed * Time.deltaTime);
     }

     public void FindNode(string start)
     {
         Landmark startEnum = (Landmark)Enum.Parse(typeof(Landmark), start);
         int startInt = (int)startEnum;
         int end = (int)nextLandmark;
         int[,] connections = new int[9, 9]
         {
             { 0, 0, 0, 0, 0, 0, 0, 1, 0 },
             { 0, 0, 0, 0, 0, 0, 0, 1, 0 }, 
             { 0, 0, 0, 0, 0, 0, 1, 1, 0 }, 
             { 0, 0, 0, 0, 0, 0, 1, 0, 0 }, 
             { 0, 0, 0, 0, 0, 0, 0, 0, 1 },
             { 0, 0, 0, 0, 0, 0, 0, 0, 1 }, 
             { 0, 0, 1, 1, 0, 0, 0, 0, 1 }, 
             { 1, 1, 1, 0, 0, 0, 0, 0, 0 }, 
             { 0, 0, 0, 0, 1, 1, 1, 0, 0 } 
         };
         if (startInt != end)
         {
             int[] endInt = FindValuePath(connections, startInt, end);
             foreach (int var in endInt)
             {
                 print("here" + var + $"{startInt} {end}");
             }

             Landmark landmark = (Landmark)Enum.Parse(typeof(Landmark), Enum.GetName(typeof(Landmark), endInt[1]));
             targetPosition = NodeManager.Instance.ReturnPosition(landmark);
             print("here" + $"{targetPosition}");
         }
     }

     private static int[] FindValuePath(int[,] connections, int start, int end)
     {
         Queue<int[]> queue = new Queue<int[]>();
         queue.Enqueue(new int[]{start});

         while (queue.Count > 0)
         {
             int[] path = queue.Dequeue();
             int lastNode = path[path.Length - 1];

             if (lastNode == end)
             {
                 return path;
             }

             for (int i = 0; i < connections.GetLength(0); i++)
             {
                 if (connections[lastNode,i] == 1 && !path.Contains(i))
                 {
                     int[] newPath = new int[path.Length+1];
                     Array.Copy(path, newPath, path.Length);
                     newPath[path.Length] = i;
                     queue.Enqueue(newPath);
                 }
             }
         }

         return new int[0];
     }
     
     
     public void ToggleVisibility(bool setActive, Landmark nextLandmark, float time=2f, Landmark previousLandmark= Landmark.Cupola)
     {
         active = setActive;
         Vector3 arrowSize = setActive ? Vector3.one : Vector3.zero;
         LeanTweenType arrowAnim = setActive? LeanTweenType.easeInQuad: LeanTweenType.easeOutQuad;
         LeanTween.scale(gameObject, arrowSize, time).setEase(arrowAnim);
        
         this.nextLandmark = nextLandmark;
         if (active)
         {
             print("here running");
             FindNode(NodeManager.Instance.ReturnCurrentPlayerNode());
             NodeManager.CurrentNode += FindNode;
         }
         else
         { 
             NodeManager.CurrentNode -= FindNode;
             targetPosition = transform.position;
         }
     }

 }
