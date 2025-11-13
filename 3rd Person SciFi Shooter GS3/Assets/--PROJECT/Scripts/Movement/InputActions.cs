using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.VFX;



public class InputActions : MonoBehaviour
{
    // Input system 
    private InputSystem_Actions characterInput;

    // Character controller
    private CharacterController controller;

    // Canvas
    [SerializeField] private GameObject pauseMenuCanvas;

    //Animator 
    Animator PlayerController;

    // Aimimation Rig
    public float aimRigWeight;

      // VFX graph
    [SerializeField] private VisualEffect muzzleFlash;

    // Cameras
    [SerializeField] public CinemachineCamera aimCamera;
    [SerializeField] public CinemachineCamera thirdPersonCamera;
    [SerializeField] public Transform cam;

    // Movement
    [SerializeField] private float moveSpeed = 5f;
    public float sprintSpeed = 6.5f;
    private float baseSpeed;
    [SerializeField] private float jumpForce = 500f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float rotationSpeed = 500f; 
    private Vector2 moveInput;
    private Vector3 verticalVelocity;

    // States
    public bool isAiming;
    public bool isCrouching;
    public bool isSprinting;
    public bool isShooting;

        
    void Awake()
    {
        // Character controller
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found.");
        }

        // Input System
        characterInput = new InputSystem_Actions();
        characterInput.Enable();

        // Animator 
        PlayerController = GetComponentInChildren<Animator>();
        if (PlayerController == null)
        {
            Debug.LogError("Animator component not found.");
        }

        muzzleFlash = GetComponent<VisualEffect>();
          
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        baseSpeed = moveSpeed;
    }

    void OnEnable()
    {
        // Move Input Action 
        characterInput.Player.Move.performed += x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInput.Player.Move.canceled += x => OnPlayerMove(x.ReadValue<Vector2>());

        // Rotate Input Action
        characterInput.Player.Look.performed += x => OnPlayerLook(x.ReadValue<Vector2>());
        characterInput.Player.Look.canceled += x => OnPlayerLook(x.ReadValue<Vector2>());

        // Sprint Input Action
        characterInput.Player.Sprint.performed += OnSprintInput;
        characterInput.Player.Sprint.canceled += OnSprintInput;

        // Crouch Input Action
        characterInput.Player.Crouch.performed += OnCrouchInput;
        characterInput.Player.Crouch.canceled += OnCrouchInput;

        // Jump Input Action 
        characterInput.Player.Jump.performed += OnJump;

        // Shoot Input Action
        characterInput.Player.Attack.performed += OnShootInput;
        characterInput.Player.Attack.canceled += OnShootInput;

        // UI Toggle Action
        characterInput.UI.Pause.performed += OnTogglePauseMenu;
        
    }

    void OnDisable()
    {
        // Move Input Action 
        characterInput.Player.Move.performed -= x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInput.Player.Move.canceled -= x => OnPlayerMove(x.ReadValue<Vector2>());

        // Rotate Input Action
        characterInput.Player.Look.performed -= x => OnPlayerLook(x.ReadValue<Vector2>());
        characterInput.Player.Look.canceled -= x => OnPlayerLook(x.ReadValue<Vector2>());

        // Sprint Input Action
        characterInput.Player.Sprint.performed -= OnSprintInput;
        characterInput.Player.Sprint.canceled -= OnSprintInput;

        // Crouch Input Action
        characterInput.Player.Crouch.performed -= OnCrouchInput;
        characterInput.Player.Crouch.canceled -= OnCrouchInput;

        // Jump Input Action 
        characterInput.Player.Jump.performed -= OnJump;

        // Shoot Input Action
        characterInput.Player.Attack.performed -= OnShootInput;
        characterInput.Player.Attack.canceled -= OnShootInput;

        // UI Toggle Action
        characterInput.UI.Pause.performed -= OnTogglePauseMenu;
        
    }

    void Update()
    {


        // Handle movement
        if (controller != null)
        {
            // Reset vertical velocity only if the character is grounded AND moving downwards.
            if (controller.isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }

            // Apply gravity to the vertical velocity.
            verticalVelocity.y += gravity * Time.deltaTime;

            // Read the move input
            // Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

            // Update Cam
            // Vector3 camforward = cam.forward;
            // Vector3 camRight = cam.right;

            // camforward.y = 0;
            // camRight.y = 0;
       
            // Vector3 forwardRelative = Input.GetAxis("Vertical") * camforward;
            // Vector3 rightRelative = Input.GetAxis("Horizontal") * camforward;

            // Vector3 moveDir = forwardRelative + rightRelative;

            // Get the Camera's forward and right vectors.
            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;

            // Calculate the final movement direction vector using the input.
            Vector3 movementDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

            // Apply movement to the Character Controller
            controller.Move(movementDirection * moveSpeed * Time.deltaTime + verticalVelocity * Time.deltaTime);

            // Handle Rotation
            // Character should rotate to face the direction of the movement input.
            if (movementDirection.magnitude >= 0.1f)
            {
                HandleRotation(movementDirection);
            }
            
            // Update Animator speed and movement parameters based on input.
            // Using the magnitude gives a smooth value from 0 to 1.
            float currentSpeed = moveInput.magnitude;

            PlayerController.SetFloat("Speed", currentSpeed);
            PlayerController.SetFloat("MoveX", moveInput.x);
            PlayerController.SetFloat("MoveY", moveInput.y);

            // Set the boolean animator parameters
            PlayerController.SetBool("isSprinting", isSprinting);
            PlayerController.SetBool("isCrouching", isCrouching);
            PlayerController.SetBool("isShooting", isShooting);

            // Apply movement and vertical velocity
            // controller.Move(transform.TransformDirection(movement) * moveSpeed * Time.deltaTime + verticalVelocity * Time.deltaTime);


        }
    }
    
      private void HandleRotation(Vector3 movementDirection)
    {
        // Calculate the target rotation based on the current movement direction.
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

        // Smoothly rotate the character to the target rotation.
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnPlayerMove(Vector2 incomingVector2)
    {
        moveInput = incomingVector2;
    }

    private void OnPlayerLook(Vector2 incomingVector2)
    {

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Use the controller's built-in isGrounded property.
        if (controller != null && controller.isGrounded)
        {
            // Set the vertical velocity to the jump force.
            verticalVelocity.y = jumpForce;

            // Correctly call SetTrigger with only one argument.
            if (PlayerController != null)
            {
                PlayerController.SetTrigger("isJump");
            }
        }
    }

    private void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            moveSpeed = sprintSpeed;
            
        }
        else
        {
            isSprinting = false;
            moveSpeed = baseSpeed;
            
        }
        Debug.Log($"Sprint state changed. Current speed: {moveSpeed}");

    }

    private void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            
        }
        else
        {
            isCrouching = false;
            
        }
    }

    public void OnShootInput(InputAction.CallbackContext context)
    {
        isShooting = context.performed;
        
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    private void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        // Checks if the pause menu canvas reference is valid.
        if (pauseMenuCanvas != null)
        {
            // Toggles the active state of the canvas.
            bool isPaused = !pauseMenuCanvas.activeSelf;
            pauseMenuCanvas.SetActive(isPaused);

            if (isPaused)
            {
                // If the game is paused, set the time scale to 0 to stop all game-related time.
                Time.timeScale = 0f;
                // Unlocks and makes the cursor visible for UI interaction.
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                characterInput.Player.Disable();
                
            }
            else
            {
                // If the game is unpaused, set the time scale back to 1.
                Time.timeScale = 1f;
                // Locks and hides the cursor for gameplay.
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                characterInput.Player.Enable();
                
            }
        }
    }
}

   
