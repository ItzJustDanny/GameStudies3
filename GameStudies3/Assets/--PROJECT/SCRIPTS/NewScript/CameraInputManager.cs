using UnityEngine;
using UnityEngine.InputSystem; 
using Unity.Cinemachine;

public class CameraInputManager : MonoBehaviour
{
    // Assign your CharacterInput asset in the Inspector
    [SerializeField]
    private InputActionAsset playerInputAsset;
    
    // This is the variable that Cinemachine reads
    public static Vector2 lookInput;

    private void Awake()
    {
        // Find the "Look" action in your input asset
        InputAction lookAction = playerInputAsset.FindAction("PlayerMap/Look");

        if (lookAction != null)
        {
            // Subscribe to the "Look" action's performed event
            lookAction.performed += OnLookPerformed;
            lookAction.canceled += OnLookCanceled;
        }
        else
        {
            Debug.LogError("Look action not found in the input asset. Check your action map.");
        }
    }

    private void OnEnable()
    {
        playerInputAsset.Enable();
    }

    private void OnDisable()
    {
        playerInputAsset.Disable();
    }

    // When the look action is performed, store its value
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        Debug.Log("Look input received: " + lookInput); // Add this line
        
    }

    // When the look action is canceled, reset the value to zero
    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }
    
    void Update()
    {
        // Pass the input to Cinemachine. This is the new, non-deprecated way.
        //CinemachineInput.Vector2Input = lookInput;
    }
}