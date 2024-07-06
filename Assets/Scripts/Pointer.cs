using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the pointer object controlled with the VR controller
/// </summary>
public class Pointer : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 previousPosition;
    
    /// <summary>
    /// Initializes the pointer position
    /// </summary>
    void Start()
    {
        initialPosition = transform.position;
        previousPosition = initialPosition;
    }


    private void Update()
    {
        // Update the previous position
        previousPosition = transform.position;
        
    }


    /// <summary>
    /// Returns the initial position of the pointer
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
    
    
    public Vector3 GetPreviousPosition()
    {
        return previousPosition;
        
    }
    
}
