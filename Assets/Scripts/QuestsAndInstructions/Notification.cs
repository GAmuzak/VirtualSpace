using System;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public static event Action NotificationDismissed;
    
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool playSoundOnStart;
    // [SerializeField] private bool disableControlsTillConfirmed;
    
    private GameObject panel;
    private Transform rigTransform;
    private Vector3 menuOffset;
    private TextMeshProUGUI targetText;

    private void Start()
    {
        panel = transform.GetChild(0).gameObject;
        menuOffset = transform.position; //Just about as busted as the oculus implementation
        rigTransform = FindObjectOfType<OVRCameraRig>().gameObject.GetComponent<Transform>();
        if (sound != null && playSoundOnStart)
        {
            SoundManager.Instance.PlaySound(sound);
        }
        panel.SetActive(false);
        targetText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>(); //I'm lazy, don't mess with the prefab order
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            bool isPanelActive = panel.activeSelf;
            if(isPanelActive) 
                NotificationDismissed?.Invoke();
            else 
                SetLocation();
            panel.SetActive(!isPanelActive);
        }
    }


    public void UpdateText(string newText)
    {
        panel.SetActive(true);
        SetLocation();
        PlaySound();
        targetText.SetText(newText);
    }

    private void SetLocation()
    {
        if (!gameObject.activeSelf) return;
        transform.position = rigTransform.TransformPoint(menuOffset);
        Vector3 newEulerRot = rigTransform.rotation.eulerAngles;
        transform.eulerAngles = newEulerRot;
    }

    private void PlaySound()
    {
        if (sound != null)
        {
            SoundManager.Instance.PlaySound(sound);
        }
        else
        {
            throw new Exception("Sound is null");
        }
    }
}