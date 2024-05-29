using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a generic lightning ray
/// </summary>
/// <remarks>
/// The lightning ray here is a general concept applied to the parent object of the LineRenderer.
/// </remarks>
public class LightningRay : MonoBehaviour
{
    private Vector3 initialPosition;
    
    /// <summary>
    /// Initializes the initial position of the lightning ray
    /// </summary>
    void Start()
    {
        initialPosition = transform.position;
    }
    
    /// <summary>
    /// Returns the initial position of the lightning ray
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
}
