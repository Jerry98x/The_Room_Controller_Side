using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General constraint class used to define the limits of the area where a ray (endpoint) can move
/// </summary>
/// <remarks>
/// Used for the first version of the Room, where action/movement is on the endpoint (the whole ray moves in the space)
/// and perception of the Visitor is on the ray itself (reflected in the change in the characteristics of the ray)
/// </remarks>
public class Constraints : MonoBehaviour
{
    
    
    [SerializeField] private float min = -1f; // The minimum coordinate
    [SerializeField] private float max = 1f; // The maximum coordinate
    

    /// <summary>
    /// Get the limits of the area where a ray (endpoint) can move
    /// </summary>
    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(min);
        limits.Add(max);
        return limits;
    }
}