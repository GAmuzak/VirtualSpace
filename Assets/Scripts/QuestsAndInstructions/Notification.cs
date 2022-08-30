using System;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool playSoundOnStart;
    // [SerializeField] private bool disableControlsTillConfirmed;

    private Transform rigTransform;
    private Vector3 menuOffset;
    private TextMeshProUGUI targetText;

    private void Start()
    {
        menuOffset = transform.position; //Just about as busted as the oculus implementation
        rigTransform = FindObjectOfType<OVRCameraRig>().gameObject.GetComponent<Transform>();
        if (sound != null && playSoundOnStart)
        {
            SoundManager.Instance.PlaySound(sound);
        }
        gameObject.SetActive(false);
        targetText = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>(); //I'm lazy, don't mess with the prefab order
    }

    public void SetLocation()
    {
        if (!gameObject.activeSelf) return;
        transform.position = rigTransform.TransformPoint(menuOffset);
        Vector3 newEulerRot = rigTransform.rotation.eulerAngles;
        transform.eulerAngles = newEulerRot;
    }

    public void UpdateText(string newText)
    {
        gameObject.SetActive(true);
        SetLocation();
        PlaySound();
        targetText.SetText(newText);
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