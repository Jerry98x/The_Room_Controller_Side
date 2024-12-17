using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the VFX object of the portal of the Deathtrap.
/// </summary>
public class DeathtrapPortalVFX : MonoBehaviour
{

    [SerializeField] private Transform center;
    
    private Vector3 direction;


    private void Start()
    {
        Vector3 yOffset = new Vector3(0, 0, 0);
        direction = center.position - transform.position - yOffset;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    
}
