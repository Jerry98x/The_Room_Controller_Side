using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the VFX object of the subportal of the Sauron (which looks like the portal of the Neto).
/// </summary>
public class SmallSauronSubportalVFX : MonoBehaviour
{

    [SerializeField] private Transform center;

    private Transform sphereAttractor;
    private Vector3 rayDirection;
    
    
    private void Start()
    {
        // First one in the hierarchy!
        sphereAttractor = GetComponentInChildren<Transform>();

    }
    
    
}
