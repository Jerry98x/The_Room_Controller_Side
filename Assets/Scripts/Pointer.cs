using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the pointer object controlled with the VR controller
/// </summary>
public class Pointer : MonoBehaviour
{
    private Vector3 pointerPosition;
    
    /// <summary>
    /// Initializes the pointer position
    /// </summary>
    void Start()
    {
        pointerPosition = transform.position;
    }
    
    
    /// <summary>
    /// Returns the initial position of the pointer
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return pointerPosition;
    }
    
}
