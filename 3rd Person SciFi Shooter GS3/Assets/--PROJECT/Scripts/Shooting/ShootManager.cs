using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.VFX;
public class ShootManager : MonoBehaviour
{
    // Input Actions system
    private InputSystem_Actions characterInput;

    // Imput Actions script
    private InputActions characterController;

    // Object Pool
     [SerializeField] private ObjectPoolingExample objectPoolManager;

    // Particle system
    [SerializeField] private ParticleSystem muzzleSpark;

    // Raycast
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform aimRigTarget;

    // Cameras 
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] public GameObject thirdPersonCam;
    [SerializeField] public GameObject aimCam;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    // layer Mask
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    // Bullet 
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private float shootForce;

    // Animators
    [SerializeField] Animator PlayerController;
    [SerializeField] private Rig aimRig;
    private float aimRigWeight;

    // States
    private bool isAiming;


    private void Awake()
    {
        characterInput = new InputSystem_Actions();
        characterController = GetComponent<InputActions>();
        PlayerController = GetComponentInChildren<Animator>();
        muzzleSpark = GetComponent<ParticleSystem>();

        if (aimRig == null)
        {
            aimRig = GetComponentInChildren<Rig>();
        }


        characterInput.Player.Aim.performed += OnAimPerformed;
        characterInput.Player.Aim.canceled += OnAimCanceled;

    }

    void Start()
    {

        thirdPersonCam.SetActive(true);
        aimCam.SetActive(false);
        // Locks the cursor to the center of the game window.
        Cursor.lockState = CursorLockMode.Locked;
        // Hides the cursor.
        Cursor.visible = false;
    }

    void Update()
    {
        if (isAiming)
        {
            aimCam.SetActive(true);
            thirdPersonCam.SetActive(false);
        }
        else
        {
            aimCam.SetActive(false);
            thirdPersonCam.SetActive(true);
        }

        
        // Raycast and update debugTransform every frame, regardless of aiming state
        UnityEngine.Camera activeCamera = cinemachineBrain.OutputCamera;

        if (activeCamera == null)
        {
            return;
        }


        /***************************************************************************************
*    Title: Awesome Third Person Shooter Controller! (Unity Tutorial)
*    Author: Code Monkey
*    Date: 2021
*    Code version: 2021.3 LTS
*    Availability: https://www.youtube.com/watch?v=FbM4CkqtOuA&t=1828s
*
***************************************************************************************/

        Vector3 mouseWorldPosition = transform.position;
        Vector2 screenCenterPoint = new Vector2(activeCamera.pixelWidth / 2f, activeCamera.pixelHeight / 2f);
        Ray ray = activeCamera.ScreenPointToRay(screenCenterPoint);

        
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (isAiming)
        {
            // Smoothly increase layer 1's weight to 1 (full weight)
            PlayerController.SetLayerWeight(1, Mathf.Lerp(PlayerController.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        }
        else
        {
            // Smoothly decrease layer 1's weight to 0 (no weight)
            PlayerController.SetLayerWeight(1, Mathf.Lerp(PlayerController.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }

        // Only turn the character if the player is aiming
        if (isAiming)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            if (characterController != null && characterController.isShooting)
            {

                TriggerMuzzleFlash();
                
                // Get the direction to shoot
                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;

                // Get a bullet from the object pool
                GameObject pooledBullet = objectPoolManager.EnableObject();

                if (pooledBullet != null)
                {
                    // Position and orient the bullet at the spawn point
                    pooledBullet.transform.position = spawnBulletPosition.position;
                    pooledBullet.transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);

                    // Get the PooledObject component and call its Shoot method
                    PooledObject pooledObjectScript = pooledBullet.GetComponent<PooledObject>();
                    if (pooledObjectScript != null)
                    {
                        pooledObjectScript.Shoot(shootForce);
                    }
                }

                // Reset the shoot state
                characterController.isShooting = false;
                
            }
        }

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f); 
        
        

    }

    private void OnEnable()
    {
        characterInput.Enable();
    }

    private void OnDisable()
    {
        characterInput.Disable();

        // Unsubscribe from events to prevent memory leaks
        characterInput.Player.Aim.performed -= OnAimPerformed;
        characterInput.Player.Aim.canceled -= OnAimCanceled;

    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        // When the aim button is held down, switch to the aim camera
        isAiming = true;
        aimCamera.Priority = 11;
        thirdPersonCamera.Priority = 10;
        aimRigWeight = 1f;
        
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        // When the aim button is released, switch back to the main camera
        isAiming = false;
        thirdPersonCamera.Priority = 11;
        aimCamera.Priority = 10;
        aimRigWeight = 0f;

    }
    

    private void TriggerMuzzleFlash()
    {
              
        if(muzzleSpark != null)
        {
            muzzleSpark.Play();
        }
    }
    
      

   
}