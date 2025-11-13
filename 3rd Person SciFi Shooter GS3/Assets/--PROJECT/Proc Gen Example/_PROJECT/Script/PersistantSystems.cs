using System;
using UnityEngine;

// This class ensures that the Game Object it is attached to is not destroyed 
// when new scenes are loaded (persistent object).
public class PersistantSystems : MonoBehaviour
{
    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Instructs Unity to prevent the current Game Object from being destroyed 
        // when a new Scene is loaded, making it persistent.
        DontDestroyOnLoad(this.gameObject);
    }
}