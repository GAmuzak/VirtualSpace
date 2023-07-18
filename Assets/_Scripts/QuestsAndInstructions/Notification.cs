using System;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public static event Action NotificationDismissed;
    public static event Action Pain;
    
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool playSoundOnStart;
    [SerializeField] private GameObject player;
    [SerializeField] private float scaleFactor;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI counterText;
    
    private GameObject panel;
    private GameObject crosshair;
    private Transform rigTransform;
    private Vector3 menuOffset;
    private float baseZValue;
    private bool isCrosshairActive;

  
    private void Start()
    {
        baseZValue = transform.localPosition.z;
        panel = transform.GetChild(0).gameObject;
        crosshair = transform.GetChild(1).gameObject;
        menuOffset = transform.position; //Just about as busted as the oculus implementation
        rigTransform = FindObjectOfType<OVRCameraRig>().gameObject.GetComponent<Transform>();
        if (sound != null && playSoundOnStart)
        {
            SoundManager.Instance.PlaySound(sound);
        }
        panel.SetActive(false);
    }

    private void Update()
    {
        
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            bool isPanelActive = panel.activeSelf;
            if (isPanelActive)
                NotificationDismissed?.Invoke();
            else 
                Pain?.Invoke();
            panel.SetActive(!isPanelActive);
            crosshair.SetActive(false);
        }
        if (isCrosshairActive & !panel.activeSelf)
        {
            crosshair.SetActive(true);
        }
        if (!panel.activeSelf) return;
        
        UpdatePosition();
    }
    public void AddCrosshair()
    {
        isCrosshairActive = true;
    }
    private void UpdatePosition()
    {
        float adjustmentVal = player.transform.rotation.x;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, baseZValue - adjustmentVal/scaleFactor);
    }


    public void UpdateText(string newText, int index, int totalCount)
    {
        panel.SetActive(true);
        crosshair.SetActive(false);
        isCrosshairActive = false;
        // SetLocation();
        PlaySound();
        targetText.SetText(newText);
        counterText.SetText("Message: " + index + "/" + totalCount);
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