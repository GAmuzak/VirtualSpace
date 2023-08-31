using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userText;
    
    [SerializeField] private TextMeshProUGUI DebugText;
    private int currentUser;
    
    void Start() 
    
    {
        // to look for the latest user
        currentUser = FindLatestUser();
        PlayerPrefs.SetInt("currentUser", currentUser);
        SetUserText(currentUser);
    }

    public void ChangeUser(int value)
    {
        currentUser += value;
        if (currentUser < 1)
        {
            currentUser = 1;
        }
        PlayerPrefs.SetInt("currentUser", currentUser);
        SetUserText(currentUser);
    }

    private void SetUserText(int user)
    {
        userText.text = ""+user;
    }
    private int FindLatestUser()
    {
        int user = 1;
        // to look for the latest user
        if (Directory.Exists(Application.persistentDataPath))
        {
            while (true)
            {   
                
                if (!Directory.Exists(Application.persistentDataPath +"/Logs/user"+ user+"/"))
                {
                    break;
                }
                user++;
            }
        }

        return user;
    }
    
}
