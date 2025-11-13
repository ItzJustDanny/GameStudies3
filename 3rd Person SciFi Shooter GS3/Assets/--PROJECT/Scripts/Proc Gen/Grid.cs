using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public GameObject cellPrefab;
    public int x;
    public int y;
    public Vector3 gridOrigin = Vector3.zero;
   
    void Start()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        
    }

   
}
