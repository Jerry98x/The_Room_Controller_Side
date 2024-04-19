using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalCoordinates : MonoBehaviour
{
    
    [SerializeField] private Transform centerOfSphere;

    [Range(0.1f, 10f)]
    [SerializeField] private float radius;
    [Range(0, 180)]
    [SerializeField] private float inclination; // Polar angle (theta)
    [Range(-180, 180)]
    [SerializeField] private float azimuth; // Azimuthal angle (phi)

    void Update()
    {
        // Convert spherical coordinates to Cartesian and move the object
        Vector3 cartesianPosition = SphericalCoordinatesHandler.SphericalToCartesian(radius, inclination, azimuth, centerOfSphere);
        transform.position = cartesianPosition;
    }

    
}
