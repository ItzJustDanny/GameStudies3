using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Detection : MonoBehaviour
{

    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject interactMenu;
    [SerializeField] private CharacterController characterController;
    private bool triggerZone = false;

    private InputSystem_Actions characterInput;

    private void Awake()
    {
        // Input System
        characterInput = new InputSystem_Actions();
        characterInput.Enable();

        
    }

    void OnEnable()
    {
        // Interact 
        characterInput.Player.InteractButton.performed += OnInteraction;
        characterInput.Player.InteractButton.canceled += OnInteraction;
    }

    void OnDisable()
    {
        // Interact 
        characterInput.Player.InteractButton.performed -= OnInteraction;
        characterInput.Player.InteractButton.canceled -= OnInteraction;
    }

    private void OnInteraction(InputAction.CallbackContext context)
    {
        Debug.Log("Interact Button Pressed (Input Registered). Trigger Zone State: " + triggerZone);
         
        if(context.performed)
        {
            if(triggerZone)
            {
                shield.SetActive(false);
                Debug.Log("Player pressed E, Deactivate shield");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            triggerZone = true;

            if (interactMenu != null)
            {
                interactMenu.SetActive(true);
                Debug.Log("Player detected, Activate menu");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            triggerZone = false;
            if (interactMenu != null)
            {
                interactMenu.SetActive(false);
                Debug.Log("Player left zone, Deactivate menu");
            }
        }
    }




}
