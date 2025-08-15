using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private CharacterInput characterInputMap;
    private Animator characterAnimator;
    Rigidbody characterRBG;
    [SerializeField] float speedMultiplier;
    Vector3 movementVector = new Vector3(0, 0, 0);



    void Awake()
    {
        characterInputMap = new CharacterInput();

        characterInputMap.Enable();

        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Jump.canceled -= OnJump;

        characterInputMap.PlayerMap.Movement.performed += x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled += x => OnPlayerStopMove(x.ReadValue<Vector2>());

        if (characterRBG == null)
        {
            characterRBG = GetComponent<Rigidbody>();
        }

        
    }

    void OnDisable()
    {
        characterInputMap.PlayerMap.Movement.performed -= x => OnPlayerMove(x.ReadValue<Vector2>());
        characterInputMap.PlayerMap.Movement.canceled -= x => OnPlayerStopMove(x.ReadValue<Vector2>());

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("We Jumped.");
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
}