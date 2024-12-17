using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the startpoint of a ray
/// </summary>
/// <remarks>
/// The startpoint can refer to either a Neto ray or a Sauron ray
/// </remarks>
public class RayStartPoint : MonoBehaviour
{

    [SerializeField] private Transform startPointPosition;
    



    private void Start()
    {
        transform.position = startPointPosition.position;
    }

    
    
}
