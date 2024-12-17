using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for handling the colliders for Neto and Sauron rays
/// </summary>
public class RayColliderSimpleHelper : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Transform rayEndpoint;
    
    
    private float yOffset = 0f;
    private float zOffset = 1f;
    
    private bool firstExecution = true;
    
    
    private void Start()
    {
        this.transform.position = rayOrigin.position;
        this.transform.LookAt(rayEndpoint.position);
    }
    
    
    private void Update()
    {
        // Calculate the direction of the ray
        Vector3 rayDirection = (rayEndpoint.position - rayOrigin.position).normalized;
        // Calculate the zOffset along the ray direction
        Vector3 zOffsetVector = zOffset * rayDirection;

        
        
        // Orient the ray collider object towards the ray endpoint, so that it is always
        // pointing along the ray direction
        //this.transform.LookAt(rayEndpoint.position);

        if (firstExecution)
        {
            this.transform.position += new Vector3(0, yOffset, 0) + zOffsetVector;
            firstExecution = false;
        }
    }
    
    
    
}
