using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;
public class ThirdPersonShooterController : MonoBehaviour
{
    private CharacterInput characterInput;
    private CharacterController characterController;

    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private ObjectPoolingExample objectPoolManager; 
    [SerializeField] private float shootForce = 100f;
    [SerializeField] Animator PlayerController;
    [SerializeField] private Rig aimRig;
    private float aimRigWeight;
    private bool isAiming;


    private void Awake()
    {
        characterInput = new CharacterInput();
        characterController = GetComponent<CharacterController>();
        PlayerController = GetComponent<Animator>();

        if (aimRig == null)
        {
            aimRig = GetComponentInChildren<Rig>(); 
        }


        characterInput.PlayerMap.Aim.performed += OnAimPerformed;
        characterInput.PlayerMap.Aim.canceled += OnAimCanceled;

         if (objectPoolManager == null)
        {
            Debug.LogError("ObjectPoolingExample script reference not set on " + gameObject.name);
        }


    }

    void Update()
    {

        // Raycast and update debugTransform every frame, regardless of aiming state
        UnityEngine.Camera activeCamera = cinemachineBrain.OutputCamera;

        if (activeCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = Vector3.zero;
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
            PlayerController.SetLayerWeight(1, Mathf.Lerp(PlayerController.GetLayerWeight(1), 1f,
             Time.deltaTime * 10f));
        }
        else
        {
            // Smoothly decrease layer 1's weight to 0 (no weight)
            PlayerController.SetLayerWeight(1, Mathf.Lerp(PlayerController.GetLayerWeight(1), 0f,
             Time.deltaTime * 10f));
        }

        // Only turn the character if the player is aiming
        if (isAiming)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            if (characterController != null && characterController.shoot)
            {
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
                characterController.shoot = false;
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
        characterInput.PlayerMap.Aim.performed -= OnAimPerformed;
        characterInput.PlayerMap.Aim.canceled -= OnAimCanceled;


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
    
      

   
}