using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Detection : MonoBehaviour
{

    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject interactMenu;
    [SerializeField] private CharacterController characterController;
    public bool triggerZone = false;

    public InputSystem_Actions characterInput;

    public void Awake()
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

    public void OnInteraction(InputAction.CallbackContext context)
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


    public void OnTriggerEnter(Collider other)
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

    public void OnTriggerExit(Collider other)
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
