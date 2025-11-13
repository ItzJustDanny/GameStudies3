using UnityEngine;

// This class handles the correct placement of a scene chunk object 
// upon its instantiation into the world.
public class PlacementManager : MonoBehaviour
{
    
    // Start is called before the first frame update.
    void Start()
    {
        // Get the first available instantiation position from the GridManager and 
        // set the current object's position to it. This places the new scene chunk 
        // at the correct 'slot' in the level layout.
        transform.position = GridManager.instance.CheckForFirstOpenInstantiationLocation().position;
        // Register the current game object (the scene chunk) with the GridManager's list 
        // of objects/chunks that have been placed.
        GridManager.instance.AddObjectToList(this.gameObject);

    }

}