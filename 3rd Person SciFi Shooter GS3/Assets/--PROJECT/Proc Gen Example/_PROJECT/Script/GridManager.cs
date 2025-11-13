using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// This class manages the layout (grid) of the procedurally generated level. 
// It tracks placed objects and provides the locations for new objects.
public class GridManager : MonoBehaviour
{
    // Static reference to the GridManager instance, implementing the Singleton pattern 
    // for easy global access (e.g., GridManager.instance.Method()).
    public static GridManager instance;
    // Reference to the SceneAdditiveLoader script, likely on the same Game Object.
    private SceneAdditiveLoader additiveloader;
    // A list of Game Objects representing the scene chunks currently placed in the grid.
    [SerializeField] List<GameObject> listOfGridObjects;
    // A list of Transforms (positions and rotations) that define the exact spots 
    // where new scene chunks should be instantiated/placed.
    [SerializeField] List<Transform> listOfInstantiationLocations;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Set the static instance reference to this object.
        instance = this;
        // Get the SceneAdditiveLoader component from the same Game Object.
        additiveloader = GetComponent<SceneAdditiveLoader>();
    }

    // Checks if a specific Game Object is already in the list of placed grid objects.
    public bool CheckIfObjectIsInList(GameObject obj)
    {
        // Returns true if the list contains the provided object, false otherwise.
        return listOfGridObjects.Contains(obj);
    }

    // Adds a Game Object (a newly placed scene chunk) to the list of placed objects.
    public void AddObjectToList(GameObject obj)
    {
        // Appends the object to the list.
        listOfGridObjects.Add(obj);
    }

    // Calculates and returns the Transform for the next open location 
    // where a new scene chunk should be placed.
    public Transform CheckForFirstOpenInstantiationLocation()
    {
        // The index of the next location is equal to the current number of 
        // objects already placed in the grid list (since list indices are 0-based).
        int _index = listOfGridObjects.Count;
        
        // Returns the Transform (position, rotation) from the list of locations 
        // that corresponds to the number of objects currently placed.
        return listOfInstantiationLocations[_index];
    }
}