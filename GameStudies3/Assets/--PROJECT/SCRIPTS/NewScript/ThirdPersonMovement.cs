using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private CharacterInput characterInputMap;

    Rigidbody characterRBG;

    [SerializeField] float speedMultiplier;

    Vector3 movementVector = new Vector3(0, 0, 0);
    private bool isGrounded;
    private float jumpForce = 5f;
    public bool shoot;
    public bool isAiming;
    public bool isCrouching;
    public bool isSprinting;
    Animator PlayerController;
    


    void Awake()
    {
        characterInputMap = new CharacterInput();
        
        characterInputMap.Enable();

        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Jump.canceled -= OnJump;

        characterInputMap.PlayerMap.Movement.performed += x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled += x => OnPlayerStopMove(x.ReadValue<Vector2>());

        characterInputMap.PlayerMap.Attack.performed += OnShootPerform;
        characterInputMap.PlayerMap.Attack.canceled += OnShootCanceled;


        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        PlayerController = GetComponent<Animator>();
    }

    void OnDisable()
    {
        characterInputMap.PlayerMap.Movement.performed -= x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled -= x => OnPlayerStopMove(x.ReadValue<Vector2>());

        characterInputMap.PlayerMap.Attack.performed -= OnShootPerform;
        characterInputMap.PlayerMap.Attack.canceled -= OnShootCanceled;

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            characterRBG.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("We Jumped.");
        }

    }

    void Update()
    {
        // Calculate overall speed and set the animator float
        float currentSpeed = movementVector.magnitude;
        PlayerController.SetFloat("Speed", currentSpeed);

        // Set the boolean animator parameters
        PlayerController.SetBool("isSprinting", isSprinting);
        PlayerController.SetBool("isCrouching", isCrouching);

        // Set the blend tree values
        PlayerController.SetFloat("MoveX", movementVector.x);
        PlayerController.SetFloat("MoveY", movementVector.z);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void FixedUpdate()
    {
        characterRBG.AddForce(movementVector * speedMultiplier, ForceMode.Force);
    }

    private void OnPlayerMove(Vector2 incomingVector2)
    {
        Debug.Log($"We are moving in direction {incomingVector2}");
        movementVector = new Vector3(incomingVector2.x, 0, incomingVector2.y);
    }

    private void OnPlayerStopMove(Vector2 incomingVector2)
    {
        movementVector = new Vector3(0, 0, 0);
    }

    private void OnCollisionStay(Collision collision)
    {
        // Checks if the object the player is colliding with has the "Ground" tag.
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If so, set the 'isGrounded' flag to true.
            isGrounded = true;


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


        }
    }

    public void OnShootPerform(InputAction.CallbackContext context)
    {
        shoot = true;
        Debug.Log("Shoot performed");
    }

    public void OnShootCanceled(InputAction.CallbackContext context)
    {
        shoot = false;
        Debug.Log("Shoot canceled");
    }
}