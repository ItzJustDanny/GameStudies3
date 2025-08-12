using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private CharacterInput characterInputMap;

    Rigidbody characterRBG;
    
    [SerializeField] float speedMultipler;

    Vector3 movementVector = new Vector3(0, 0, 0);
    void Awake()
    {
        characterInputMap = new CharacterInput();
        
        characterInputMap.Enable();
        //Jump ----
        characterInputMap.PlayerMap.Jump.performed += OnJump;
        characterInputMap.PlayerMap.Jump.canceled -= OnJump;
        
        //Pause ----
        characterInputMap.PlayerMap.Pause.performed += OnPause;
        characterInputMap.PlayerMap.Pause.canceled -= OnPause;
        
        //Movement ----
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

    private void OnPlayerMove(Vector2 incomingVector2)
    {
        Debug.Log($"We are moving in dircetion {incomingVector2}");
        Vector3 _movementVector = new Vector3(incomingVector2.x, 0, incomingVector2.y);
        characterRBG.AddForce(_movementVector * speedMultipler, ForceMode.Force);
    }

    private void OnPlayerStopMove(Vector2 incomingVector2)
    {
        movementVector = new Vector3(0, 0, 0);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("We Perform a Jump");
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("We Pause");
    }
    
}