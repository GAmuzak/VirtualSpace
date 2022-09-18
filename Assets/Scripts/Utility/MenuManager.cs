using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        QuestManager.EndGame += ReturnToMenu;
    }

    private void OnDisable()
    {
        QuestManager.EndGame -= ReturnToMenu;
    }

    private void Update()
    {
        ReturnToMenu();
    }

    private static void ReturnToMenu()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
