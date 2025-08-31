using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementManager : MonoBehaviour
{
    // Player Input
    private CharacterInput characterInputMap;

    Rigidbody characterRBG;

    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] float sprintMultiplier = 2f;
    public Vector3 movementVector = new Vector3(0, 0, 0);
    [SerializeField] private float jumpForce = 5f;
    private bool isGrounded;

    //Animator

    Animator PlayerController;
    public bool isSprinting;
    public bool isCrouching;
    public bool isWalking;

    //Shooting (ObjectPool)

    // Raycast 
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
   




    void Awake()
    {
        characterInputMap = new CharacterInput();

        characterInputMap.Enable();

        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Jump.canceled -= OnJump;

        characterInputMap.PlayerMap.Movement.performed += x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled += x => OnPlayerStopMove(x.ReadValue<Vector2>());

        characterInputMap.PlayerMap.Sprint.performed += OnSprintInput;
        characterInputMap.PlayerMap.Sprint.canceled += OnSprintInput;

        characterInputMap.PlayerMap.Crouch.performed += x => isCrouching = true;
        characterInputMap.PlayerMap.Crouch.canceled += x => isCrouching = false;

        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }


    }

    void Start()
    {
        PlayerController = GetComponent<Animator>();
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
    
    // Corrected function call
    //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

    // Raycast to get the hit point and set the debug transform's position
   // if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
   // {
   //     debugTransform.position = raycastHit.point;
   // }

       
    }


    void OnDisable()
    {
        characterInputMap.PlayerMap.Movement.performed -= x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled -= x => OnPlayerStopMove(x.ReadValue<Vector2>());

        characterInputMap.PlayerMap.Sprint.performed -= OnSprintInput;
        characterInputMap.PlayerMap.Sprint.canceled -= OnSprintInput;

    }

    void FixedUpdate()
    {

        float currentSpeed = maxSpeed;

        // Check if the player is sprinting AND moving forward
        // If so, apply the sprintMultiplier to the current speed
        if (isSprinting && movementVector.magnitude > 0.1f)
        {
            currentSpeed *= sprintMultiplier;
        }

        // Use the currentSpeed variable to apply force
        characterRBG.AddForce(movementVector * currentSpeed, ForceMode.Force);
    }


    private void OnPlayerMove(Vector2 incomingVector2)
    {
        movementVector = new Vector3(incomingVector2.x, 0, incomingVector2.y);
        
    }

    private void OnPlayerStopMove(Vector2 incomingVector2)
    {
        movementVector = new Vector3(0, 0, 0);
    }

    private void OnSprintInput(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
      
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            characterRBG.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("We Jumped.");
        }

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
    
    
}