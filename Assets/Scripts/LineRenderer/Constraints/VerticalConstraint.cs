using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constraint class used to define the vertical limits of the area where a Neto ray (endpoint) can move
/// </summary>
/// <remarks>
/// Used for the first version of the Room, where action/movement is on the endpoint (the whole ray moves in the space)
/// and perception of the Visitor is on the ray itself (reflected in the change in the characteristics of the ray)
/// </remarks>
public class VerticalConstraint : MonoBehaviour
{

    
    [SerializeField] private float minY; // The minimum y-coordinate
    [SerializeField] private float maxY; // The maximum y-coordinate
    

    /// <summary>
    /// Get the vertical limits of the area where a ray (endpoint) can move
    /// </summary>
    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(minY);
        limits.Add(maxY);
        return limits;
    }
}
