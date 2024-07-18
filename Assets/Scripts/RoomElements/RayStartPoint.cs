using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayStartPoint : MonoBehaviour
{

    [SerializeField] private Transform startPointPosition;
    



    private void Start()
    {
        transform.position = startPointPosition.position;
    }

    
    
}
