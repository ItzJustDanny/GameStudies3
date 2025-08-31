using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Third Person Camera
    [SerializeField] public Transform cameraTransform;
    
    // Aim Camera
    [SerializeField] public CinemachineCamera thirdPersonCamera;
    [SerializeField] public CinemachineCamera aimCamera;

    //Animator
    Animator PlayerAnimation;

    // Raycast
    [SerializeField] LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] LayerMask playerLayerMask;

    // Player Input
    public Rigidbody characterRBG;
    private CharacterInput characterInputMap;
    [SerializeField] float speedMultiplier;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private float jumpForce = 5f;
    Vector2 moveInput;
    private bool isGrounded;

     private void Start()
    {
        // Locks the cursor to the center of the game window.
        Cursor.lockState = CursorLockMode.Locked;
        // Hides the cursor.
        Cursor.visible = false;
        // If the Rigidbody isn't assigned in the Editor, try to get it from the same GameObject.
        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }

        // Gets the Animator component from the same GameObject and stores it.
        PlayerAnimation = GetComponent<Animator>();
        // Logs a message to the Unity console for debugging.
        Debug.Log("Animator");
    }

    // Called when the script instance is being loaded.
    void Awake()
    {
        // Creates a new instance of the input action asset.
        characterInputMap = new CharacterInput();
        // Enables all action maps within the input action asset.
        characterInputMap.Enable();

        // Subscribes the OnAimToggle method to the 'performed' and 'canceled' events of the 'Aim' action.
        characterInputMap.PlayerMap.Aim.performed += OnAimToggle;
        characterInputMap.PlayerMap.Aim.canceled += OnAimToggle;
        // Subscribes the OnJump method to the 'performed' event of the 'Jump' action.
        characterInputMap.PlayerMap.Jump.performed += OnJump;
        // Subscribes a lambda function to the 'performed' event of the 'Movement' action to read the input value.
        characterInputMap.PlayerMap.Movement.performed += x => moveInput = x.ReadValue<Vector2>();
        // Subscribes a lambda function to the 'canceled' event of the 'Movement' action to reset the input value.
        characterInputMap.PlayerMap.Movement.canceled += x => moveInput = Vector2.zero;
        // Subscribes the OnTogglePauseMenu method to the 'performed' event of the 'Pause' action.
        characterInputMap.PlayerMap.Pause.performed += OnTogglePauseMenu;
    }

    // Called when the behaviour becomes disabled or inactive.
    void OnDisable()
    {
        // Unsubscribes methods from input action events to prevent memory leaks or errors.
        characterInputMap.PlayerMap.Jump.performed -= OnJump;
        characterInputMap.PlayerMap.Movement.performed -= x => moveInput = x.ReadValue<Vector2>();
        characterInputMap.PlayerMap.Movement.canceled -= x => moveInput = Vector2.zero;
        characterInputMap.PlayerMap.Pause.performed -= OnTogglePauseMenu;
        characterInputMap.PlayerMap.Aim.canceled -= OnAimToggle;
    }

    // Called once per frame. Use for game logic that doesn't rely on physics.
    void Update()
    {
        // Align the player's rotation with the camera's Y-axis rotation.
        Vector3 camForward = cameraTransform.forward;
        // Sets the Y component of the camera's forward vector to zero,
        // so the player only rotates on the horizontal plane.
        camForward.y = 0;
        // Creates a new rotation that looks in the direction of the flattened camera forward vector.
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        // Smoothly rotates the player towards the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Sets the "MoveX" and "MoveY" float parameters in the Animator to the movement input values.
        // This is used to control animation blending (e.g., walking forward/backward, strafing).
        PlayerAnimation.SetFloat("MoveX", moveInput.x);
        PlayerAnimation.SetFloat("MoveY", moveInput.y);

        // Calculate the final layer mask by excluding the player's layer from the aimColliderMask.
        LayerMask finalAimMask = aimColliderMask & ~playerLayerMask;
        

        Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f);
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, finalAimMask))
        {
            debugTransform.position = raycastHit.point;
        }
    }

    // Called at a fixed rate, independent of frame rate. Use for physics-based updates.
    void FixedUpdate()
    {
        // Handle movement using Rigidbody.
        // Calculates the movement direction based on the player's own forward and right directions,
        // using the stored movement input.
        Vector3 movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        // Applies a force to the Rigidbody in the calculated movement direction,
        // scaled by 'speedMultiplier'. ForceMode.Force applies a continuous force.
        characterRBG.AddForce(movement.normalized * speedMultiplier, ForceMode.Force);
    }

    // Toggles between the 'AimCamera' and 'ThirdPersonCamera' (ADS).
    private void OnAimToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // The button was pressed (performed), so enable the aim camera and disable the third-person camera.
            aimCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
            // Sets the "IsAim" trigger in the Animator to play the aiming animation.
            PlayerAnimation.SetTrigger("IsAim");
            // Logs a debug message.
            Debug.Log("Is aiming in");
        }
        else
        {
            // The button was released (canceled), so disable the aim camera and enable the third-person camera.
            aimCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
            // Sets the "IsAimOut" trigger in the Animator to play the un-aiming animation.
            PlayerAnimation.SetTrigger("IsAimOut");
            // Logs a debug message.
            Debug.Log("Is aming out");
        }
    }

    // Applies a jump force to the player.
    private void OnJump(InputAction.CallbackContext context)
    {
        // Checks if the Rigidbody reference is valid.
        if (characterRBG != null)
        {
            // Checks if the player is on the ground before allowing a jump.
            if (isGrounded)
            {
                // Applies an instantaneous force (Impulse) upwards to make the character jump.
                characterRBG.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                // Sets the "IsJump" trigger in the Animator to play the jump animation.
                PlayerAnimation.SetTrigger("IsJump");
                // Logs a debug message.
                Debug.Log("Set IsJump Trigger in Animator.");
            }
        }
    }

    // Checks if the player is on the ground using the "Ground" tag.
    private void OnCollisionStay(Collision collision)
    {
        // Checks if the object the player is colliding with has the "Ground" tag.
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If so, set the 'isGrounded' flag to true.
            isGrounded = true;
            // Sets the "isGrounded" boolean parameter in the Animator to true.
            PlayerAnimation.SetBool("isGrounded", true);
        }
    }

    // Called when a collider stops touching another collider.
    private void OnCollisionExit(Collision collision)
    {
        // Checks if the object the player stopped colliding with has the "Ground" tag.
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If so, set the 'isGrounded' flag to false.
            isGrounded = false;
            // Sets the "isGrounded" boolean parameter in the Animator to false.
            PlayerAnimation.SetBool("isGrounded", false);
        }
    }

    // Toggles the pause menu on or off.
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
            }
            else
            {
                // If the game is unpaused, set the time scale back to 1.
                Time.timeScale = 1f;
                // Locks and hides the cursor for gameplay.
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}