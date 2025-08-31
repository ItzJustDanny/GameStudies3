using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    internal static object main;
    public float sensX;
    public float sensY;
    public Transform orientation;
    float XRotation;
    float YRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        YRotation += mouseX;
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90f, 90);

        //Rotate camera and orientation
        transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
        orientation.rotation = Quaternion.Euler(0, YRotation, 0);
    }

    











    
}
