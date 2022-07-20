using System;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private Image image;
    [SerializeField] private GameObject model;
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool playSoundOnStart;
    [SerializeField] private bool requiresConfirmation;

    private void Start()
    {
        if (sound != null && playSoundOnStart)
        {
            SoundManager.Instance.PlaySound(sound);
        }
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
}