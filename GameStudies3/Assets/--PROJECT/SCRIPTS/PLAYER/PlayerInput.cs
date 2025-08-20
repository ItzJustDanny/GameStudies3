using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Third Person Camera
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObject;
    public Rigidbody characterRBG;
    public float rotationSpeed;

    // Player Input
    private CharacterInput characterInputMap;
    private Animator characterAnimator;
    [SerializeField] float speedMultiplier;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private float jumpForce = 10f;
    Vector3 movementVector = new Vector3(0, 0, 0);


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Awake()
    {
        characterInputMap = new CharacterInput();

        characterInputMap.Enable();
        characterInputMap.CameraControls.Enable();



        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Jump.canceled -= OnJump;

        characterInputMap.PlayerMap.Movement.performed += x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled += x => OnPlayerStopMove(x.ReadValue<Vector2>());

        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }

        characterInputMap.PlayerMap.Pause.performed += OnTogglePauseMenu;
    }

    void OnDisable()
    {
        characterInputMap.PlayerMap.Movement.performed -= x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled -= x => OnPlayerStopMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Pause.performed -= OnTogglePauseMenu;
    }


    private void OnJump(InputAction.CallbackContext context)
    {

         // Apply an upward force to the Rigidbody
    characterRBG.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("We Jumped.");
    }

    void Update()
    {
        Vector2 lookInput = characterInputMap.CameraControls.Look.ReadValue<Vector2>();

        orientation.Rotate(Vector3.up, lookInput.x * rotationSpeed * Time.deltaTime);

                 
        playerObject.forward = Vector3.Slerp(playerObject.forward, orientation.forward, Time.deltaTime * rotationSpeed);
    }

    void FixedUpdate()
    {
        characterRBG.AddForce(movementVector * speedMultiplier, ForceMode.Force);
    }

    private void OnPlayerMove(Vector2 incomingVector2)
    {
        Debug.Log($"We are moving in direction {incomingVector2}");
        Vector3 inputDir = orientation.forward * incomingVector2.y + orientation.right * incomingVector2.x;
        movementVector = inputDir.normalized;
    }

    private void OnPlayerStopMove(Vector2 incomingVector2)
    {
        movementVector = new Vector3(0, 0, 0);
    }

    private void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (pauseMenuCanvas != null)
        {
            bool isPaused = !pauseMenuCanvas.activeSelf;
            pauseMenuCanvas.SetActive(isPaused);

            if (isPaused)
            {
                // Pause the game and show the cursor
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Resume the game and hide the cursor
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Debug.Log($"Pause Menu Toggled: {isPaused}");
        }
    }

    }
    
