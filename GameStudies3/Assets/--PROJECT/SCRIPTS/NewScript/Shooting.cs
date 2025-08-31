using UnityEngine;
using UnityEngine.InputSystem;
public class Shooting : MonoBehaviour
{
    private CharacterInput characterInputMap;
    [SerializeField] private ObjectPoolingExample bulletPool;
    [SerializeField] public Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;

    void Awake()
    {
        characterInputMap = new CharacterInput();
        characterInputMap.Enable();
        characterInputMap.PlayerMap.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        characterInputMap.PlayerMap.Attack.performed -= OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {

    }

    public void Fireing()
    {
        RaycastHit hit;

      //  Physics.Raycast(Fire.postion, transform)
    }



}
