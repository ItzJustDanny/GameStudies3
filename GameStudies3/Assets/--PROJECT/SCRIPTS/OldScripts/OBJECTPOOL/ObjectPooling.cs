using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingExample : MonoBehaviour
{

    List<GameObject> ObjectPool = new List<GameObject>();

    [SerializeField] GameObject pooledObjectPrefab;

    [SerializeField] Transform parentTransform;

    [SerializeField] float objectPoolSize;

    void Start()
    {
        for (int i = 0; i < objectPoolSize; i++)
        {
            GameObject _temporaryGameObject = Instantiate(pooledObjectPrefab, parentTransform);

            if (_temporaryGameObject.TryGetComponent<PooledObject>(out PooledObject _foundObject))
            {
                _foundObject.SetObjectPoolParent(this);
            }

            _temporaryGameObject.SetActive(false);
            ObjectPool.Add(_temporaryGameObject);
        }
    }

    public GameObject EnableObject() // Change return type to GameObject
{
    GameObject _tempObject = GetPooledObject();
    if (_tempObject != null)
    {
        _tempObject.SetActive(true);
        return _tempObject; // Return the activated object
    }
    return null; // Return null if no object is available
}

    public GameObject GetPooledObject()
    {
        foreach (GameObject _pooledItem in ObjectPool)
        {
            if (!_pooledItem.activeInHierarchy)
            {
                return _pooledItem;
            }
        }
        return null;
    }
}
