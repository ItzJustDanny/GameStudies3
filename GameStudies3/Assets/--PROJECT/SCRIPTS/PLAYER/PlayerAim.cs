using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget;
    [SerializeField] private InputActionReference lookInput;
    [SerializeField] private InputActionReference switchSholderinput;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float gamepadSensitivity = 0.5f;
    [SerializeField] private float sensitivity = .5f;
    [SerializeField] private float pitchMix = -40f;
    [SerializeField] private float pitchMax = 80f;
    [SerializeField] private CinemachineThirdPersonFollow aimCam;
    [SerializeField] private float shoulderSwitchSpeed = 5f;
    private float yaw;
    private float pitch;
    private float targetCameraSide;

    private void Awake()
    {
        aimCam = GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimCam.CameraSide;

    }

    void Start()
    {
        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        lookInput.asset.Enable();


    }

    private void OnEnable()
    {
        switchSholderinput.action.Enable();
        switchSholderinput.action.performed += OnswitchShoulder;
    }

    private void OnDisable()
    {
        switchSholderinput.action.Disable();
        switchSholderinput.action.performed -= OnswitchShoulder;
    }

    private void OnswitchShoulder(InputAction.CallbackContext context)
    {
        targetCameraSide = aimCam.CameraSide < 0.5f ? 1f : 0f;
    }

    void Update()
    {
        Vector2 look = lookInput.action.ReadValue<Vector2>();

        if (Mouse.current != null && Mouse.current.delta.IsActuated())
        {
            look *= mouseSensitivity;
        }

        else if (Gamepad.current != null && Gamepad.current.rightStick.IsActuated())
        {
            look *= gamepadSensitivity;
        }

        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);
    }

















}
