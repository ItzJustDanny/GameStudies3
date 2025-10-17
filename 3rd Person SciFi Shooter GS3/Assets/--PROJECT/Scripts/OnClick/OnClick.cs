using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClick : MonoBehaviour
{

    public void OnApplicationQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    
        Debug.Log("Quiting Game");
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    public void GameQuit(string mainmenu)
    {
        SceneManager.LoadScene(mainmenu);
    }
    
    
    
}
