using UnityEngine;
using UnityEngine.Events;

public class QuestFramework : MonoBehaviour
{
    public UnityAction<QuestState> changeState;
    
    [SerializeField] protected QuestState state;
    [SerializeField] protected GameObject Notification;

    
}