using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the VFX object of the portal of the Neto.
/// </summary>
public class NetoPortalVFX : MonoBehaviour
{

    [SerializeField] private Transform center;

    private Transform sphereAttractor;
    private Vector3 rayDirection;


    private void Start()
    {
        // First one in the hierarchy!
        sphereAttractor = GetComponentInChildren<Transform>();
        
        Vector3 yOffset = new Vector3(0, 0, 0);
        rayDirection = center.position - transform.position - yOffset;
        sphereAttractor.rotation = Quaternion.LookRotation(rayDirection);
    }
}
