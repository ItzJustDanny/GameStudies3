using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// This class manages loading and unloading scenes additively for procedural generation.
public class SceneAdditiveLoader : MonoBehaviour
{
    // A list of strings that holds the names of all potential scene 'chunks' 
    // that can be loaded additively to build the level. This is serialized in the inspector.
    [SerializeField] List<string> listOfPotentialScenes = new List<string>();


    // Start is called before the first frame update.
    private void Start()
    {
        // Call the method to load a random scene additively.
        AddRandomAdditiveScene();
        // Call it again to load a second random scene, starting the level assembly.
        AddRandomAdditiveScene();
    }

    // Public method to load a specific scene by name additively.
    public void AddSceneAdditively(string _scene)
    {
        // Check if the provided scene name is in the list of valid potential scenes.
        if (listOfPotentialScenes.Contains(_scene))
        {
            // Asynchronously load the specified scene in Additive mode, meaning it loads 
            // alongside the current active scene without unloading it.
            SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Additive);
        }
    }

    // Public method to select and load a random scene from the list additively.
    public void AddRandomAdditiveScene()
    {
        // Variable to hold the name of the randomly selected scene.
        string _selectedSceneName = "";
        // Generate a random integer index within the bounds of the list (0 inclusive, Count exclusive).
        int _randomNumber = Random.Range(0, listOfPotentialScenes.Count);
        // Get the scene name at the randomly generated index.
        _selectedSceneName = listOfPotentialScenes[_randomNumber];

        // Asynchronously load the randomly selected scene additively.
        SceneManager.LoadSceneAsync(_selectedSceneName, LoadSceneMode.Additive);
    }

    // Public method to unload a specific scene by name.
    public void RemoveAdditiveScene(string _scene)
    {
        // Check if the provided scene name is in the list of valid potential scenes.
        if (listOfPotentialScenes.Contains(_scene))
        {
            // Asynchronously unload the specified scene.
            SceneManager.UnloadSceneAsync(_scene);
        }
    }
}