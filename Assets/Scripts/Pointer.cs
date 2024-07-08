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
    
    private bool isFirstFrame = true;
    
    
    
    /// <summary>
    /// Initializes the pointer position
    /// </summary>
    void Start()
    {
        initialPosition = transform.position;
        previousPosition = initialPosition;
    }


    private void LateUpdate()
    {
        // Update the previous position
        // Do it in LateUpdate to ensure that the previous position is updated after the current position
        // (loose the first frame, but that's acceptable)
        if (isFirstFrame)
        {
            // Ensure the previous position is set correctly during the first frame
            previousPosition = transform.position;
            isFirstFrame = false;
        }
        else
        {
            // Update the previous position in LateUpdate
            previousPosition = transform.position;
        }
        
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
