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

    public void LoadScene (string MainMenu)
    {
        SceneManager.LoadScene(MainMenu);
    }
    
}
