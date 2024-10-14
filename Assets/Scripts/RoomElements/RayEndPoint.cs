using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents the endpoint of a ray
/// </summary>
/// <remarks>
/// The endpoint can refer to either a Neto ray or a Sauron ray
/// </remarks>
public class RayEndPoint : MonoBehaviour
{
    
    [SerializeField] private float endpointMovementMultiplier;
    [SerializeField] private float minEndpointDistance;
    [SerializeField] private float maxEndpointDistance;
    [SerializeField] private Transform center;

    private Vector3 initialPosition;
    private Vector3 lastPosition;
    
    //[SerializeField] private List<GameObject> vfxPrefab; // Reference to the VFX object
    
    /// <summary>
    /// Initializes the endpoint's initial position
    /// </summary>
    void Start()
    {
        initialPosition = transform.position;
        lastPosition = initialPosition;
        
        Vector3 direction = center.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        
    }
    
    private void Update()
    {
        Vector3 direction = center.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
    }


    /*public void ActivateVFX()
    {
        // Instantiate the VFX object
        GameObject vfxObject = Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        // Activate the VFX object
        vfxObject.SetActive(true);

        // Optionally, destroy the VFX object after some time
        Destroy(vfxObject, 5f); // Destroy after 5 seconds
    }*/
    
    
    
    /// <summary>
    /// Returns the initial position of the endpoint
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
    
    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }
    
    
        
    public float GetEndpointMovementMultiplier()
    {
        return endpointMovementMultiplier;
    }
    
    public float GetMinEndpointDistance()
    {
        return minEndpointDistance;
    }
    
    public float GetMaxEndpointDistance()
    {
        return maxEndpointDistance;
    }


}
