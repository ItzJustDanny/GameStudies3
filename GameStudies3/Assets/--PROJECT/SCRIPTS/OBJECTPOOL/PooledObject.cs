using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPoolingExample objectPoolManager;

    public void SetObjectPoolParent(ObjectPoolingExample parent)
    {
        objectPoolManager = parent;
    }

   
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}