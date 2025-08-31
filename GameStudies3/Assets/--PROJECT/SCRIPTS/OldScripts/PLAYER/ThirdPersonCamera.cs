using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{


    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 10f; 
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;
    private CharacterInput controls;
    private CinemachineCamera cam;
    private CinemachineThirdPersonFollow thirdPersonFollow;
    private Vector2 scrollDelta;
    private float targetZoom;
    private float currentZoom;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         controls = new CharacterInput();
    controls.Enable();
    controls.CameraControls.MouseZoom.performed += HandleMouseScroll;

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    // Correctly get components from the same GameObject
    cam = GetComponent<CinemachineCamera>();
    thirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();
    
    // Check for null references
    if (cam == null || thirdPersonFollow == null)
    {
        Debug.LogError("CinemachineCamera or CinemachineThirdPersonFollow component not found on this GameObject.", this);
        return;
    }

    targetZoom = currentZoom = thirdPersonFollow.CameraDistance;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Mouse is Scrolling. Value : {scrollDelta}");

    }

    // Update is called once per frame
    void Update()
    {
        // Combine mouse scroll and gamepad input into a single zoom value
    float combinedZoomInput = scrollDelta.y;

    float bumperDelta = controls.CameraControls.GamePadZoom.ReadValue<float>();
    combinedZoomInput += bumperDelta;

    if (combinedZoomInput != 0)
    {
        // Calculate a new target zoom based on the combined input
        targetZoom = Mathf.Clamp(thirdPersonFollow.CameraDistance - combinedZoomInput * zoomSpeed, minDistance, maxDistance);
    }
    
    // Smoothly move the camera to the new targetZoom
    thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetZoom, Time.deltaTime * zoomLerpSpeed);

    // Reset scrollDelta after using it
    scrollDelta = Vector2.zero;
    }
}
