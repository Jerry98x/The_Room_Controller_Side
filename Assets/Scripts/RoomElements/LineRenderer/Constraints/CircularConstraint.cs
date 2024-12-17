using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT USED

/// <summary>
/// Constraint class used to define the limits of the area where Sauron a ray (endpoint) can move
/// </summary>
/// <remarks>
/// Used for the first version of the Room, where action/movement is on the endpoint (the whole ray moves in the space)
/// and perception of the Visitor is on the ray itself (reflected in the change in the characteristics of the ray)
/// </remarks>
public class CircularConstraint : MonoBehaviour
{
    
    [SerializeField] private float radius = 1f;

    /// <summary>
    /// Get the radius of the area where a ray (endpoint) can move
    /// </summary>
    /// <returns></returns>
    public float GetRadius()
    {
        return radius;
    }
    
}
