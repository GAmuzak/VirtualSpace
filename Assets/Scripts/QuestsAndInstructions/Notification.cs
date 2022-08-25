using System;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool playSoundOnStart;
    [SerializeField] private bool disableControlsTillConfirmed;

    private OVRCameraRig rig;
    private Vector3 menuOffset;

    private void Start()
    {
        menuOffset = transform.position; //Just about as busted as the oculus implementation
        rig = FindObjectOfType<OVRCameraRig>();
        if (sound != null && playSoundOnStart)
        {
            SoundManager.Instance.PlaySound(sound);
        }
        gameObject.SetActive(false);
    }
    
    public void PlaySound()
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

    public void SetLocation()
    {
        transform.position = rig.transform.TransformPoint(menuOffset);
        Vector3 newEulerRot = rig.transform.rotation.eulerAngles;
        transform.eulerAngles = newEulerRot;
    }
}