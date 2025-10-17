using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    /*Awesome Third Person Shooter Controller! (Unity Tutorial)(2021).Unity 2021.3.45f1.Code Monkey.
    https://www.youtube.com/watch?v=FbM4CkqtOuA&t=1176s*/


    private Rigidbody bulletRigidbody;


    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        float speed = 15f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("HIT");
        }
        
    }


    void Update()
    {
        
    }
}
