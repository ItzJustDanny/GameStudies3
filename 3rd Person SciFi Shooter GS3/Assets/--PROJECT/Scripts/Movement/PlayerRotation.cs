using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerRotation : MonoBehaviour
{
    // Player 
    [Header("References")]
    public Transform orientation;
    public Transform player; 
    public Transform playerobj; 
    public CharacterController characterController;
    public float rotationSpeed; 

    // Input System
    private InputSystem_Actions characterInput;
    
    private Vector2 moveInput;

    void Awake()
    {
       
       characterInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Enable the input action map
        characterInput.Enable();

     
    }

    private void OnDisable()
    {
        
        characterInput.Disable();
    }

    private void Update()
    {
        // Inpust system
        moveInput = characterInput.Player.Move.ReadValue<Vector2>();

        // Camera Orientation Logic 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // Rotation Logic 
        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;
        
        // Calculate the direction vector relative to the orientation
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Rotate player object (visual model)
        if (inputDir != Vector3.zero)
        {
            // Use Slerp for smooth rotation of the visual model towards the movement direction
            playerobj.forward = Vector3.Slerp(playerobj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);

            
        }

        
    }
}