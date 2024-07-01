using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class to use in the Unity Editor to get Cartesian coordinates from spherical coordinates
/// </summary>
public class SphericalCoordinates : MonoBehaviour
{
    
    [SerializeField] private Transform centerOfSphere;

    [Range(2f, 10f)]
    [SerializeField] private float radius;
    [Range(-90, 90)]
    [SerializeField] private float inclination; // Polar angle (theta)
    [Range(-180, 180)]
    [SerializeField] private float azimuth; // Azimuthal angle (phi)

    /// <summary>
    /// Converts spherical coordinates to Cartesian coordinates and moves the object accordingly at each frame
    /// </summary>
    void Update()
    {
        // Convert spherical coordinates to Cartesian and move the object
        Vector3 cartesianPosition = SphericalCoordinatesHandler.SphericalToCartesian(radius, inclination, azimuth, centerOfSphere);
        transform.position = cartesianPosition;
        
        // Calculate the direction vector from the endpoint object to the core center
        Vector3 directionToCenter = centerOfSphere.position - transform.position;
        // Update the rotation of the endpoint object to face the core center
        transform.rotation = Quaternion.LookRotation(directionToCenter);
    }

    
}
