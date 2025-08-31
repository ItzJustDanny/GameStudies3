using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
     private ObjectPoolingExample objectPoolManager;
    [SerializeField] private float deactivateTime = 2f;

    public void SetObjectPoolParent(ObjectPoolingExample parent)
    {
        objectPoolManager = parent;
    }

    // This method is called by the Shooting script to fire the bullet
    public void Shoot(float speed)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // Deactivate the bullet after a set amount of time
        StartCoroutine(DeactivateBullet());
    }

    private IEnumerator DeactivateBullet()
    {
        yield return new WaitForSeconds(deactivateTime);
        gameObject.SetActive(false);
    }

}