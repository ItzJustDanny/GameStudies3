using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    // Third Person Camera
    [SerializeField] public Transform cameraTransform;
    public Rigidbody characterRBG;

    // Aim Camera
    [SerializeField] public CinemachineCamera thirdPersonCamera;
    [SerializeField] public CinemachineCamera aimCamera;

    //Animator
    Animator PlayerAnimation;
     

    // Player Input
    private CharacterInput characterInputMap;
    [SerializeField] const float speedMultiplier = 10f;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private float jumpForce = 10f;
    Vector2 moveInput;
    private bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }

        PlayerAnimation = GetComponent<Animator>();
        Debug.Log("Animator");
    }

    void Awake()
    {
        characterInputMap = new CharacterInput();
        characterInputMap.Enable();

        characterInputMap.PlayerMap.Aim.performed += OnAimToggle;
        characterInputMap.PlayerMap.Aim.canceled += OnAimToggle; 
        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Movement.performed += x => moveInput = x.ReadValue<Vector2>();
        characterInputMap.PlayerMap.Movement.canceled += x => moveInput = Vector2.zero;
        characterInputMap.PlayerMap.Pause.performed += OnTogglePauseMenu;
    }

    void OnDisable()
    {
        characterInputMap.PlayerMap.Jump.performed -= OnJump;
        characterInputMap.PlayerMap.Movement.performed -= x => moveInput = x.ReadValue<Vector2>();
        characterInputMap.PlayerMap.Movement.canceled -= x => moveInput = Vector2.zero;
        characterInputMap.PlayerMap.Pause.performed -= OnTogglePauseMenu;
        characterInputMap.PlayerMap.Aim.canceled -= OnAimToggle;
        
    }

    void Update()
    {

        //  align the player's rotation with the camera's Y-axis rotation.
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        Debug.Log("Move Input: " + moveInput);
        
        PlayerAnimation.SetFloat("MoveX", moveInput.x);
        PlayerAnimation.SetFloat("MoveY", moveInput.y);

        
    }

    void FixedUpdate()
    {
        // Handle movement using Rigidbody
        // Move based on the player's own forward and right directions, not the camera's.
        Vector3 movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        characterRBG.AddForce(movement.normalized * speedMultiplier, ForceMode.Force);
    }

    private void OnAimToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
    {
        // The button was pressed (performed)
        aimCamera.gameObject.SetActive(true);
        thirdPersonCamera.gameObject.SetActive(false);
    }
    else
    {
        // The button was released (canceled)
        aimCamera.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);
    }
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (characterRBG != null)
        {
            if (isGrounded)
            {
                characterRBG.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                // Set the Animator trigger to play the jump animation
                PlayerAnimation.SetTrigger("IsJump");
                Debug.Log("Set IsJump Trigger in Animator.");
            }
            
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            PlayerAnimation.SetBool("isGrounded", true);
            Debug.Log("Is Grounded: True");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            PlayerAnimation.SetBool("isGrounded", false);
            Debug.Log("Is Grounded: False");
        }
        
    }

    private void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (pauseMenuCanvas != null)
        {
            bool isPaused = !pauseMenuCanvas.activeSelf;
            pauseMenuCanvas.SetActive(isPaused);

            if (isPaused)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}