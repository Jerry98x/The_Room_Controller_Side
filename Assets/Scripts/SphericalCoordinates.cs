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

    // Convert spherical coordinates to Cartesian coordinates
    public Vector3 SphericalToCartesian(float radius, float inclination, float azimuth, Transform centerOfSphere)
    {
        float radInclination = inclination * Mathf.Deg2Rad; // Convert inclination from degrees to radians
        float radAzimuth = azimuth * Mathf.Deg2Rad; // Convert azimuth from degrees to radians
        
        float x = radius * Mathf.Sin(radInclination) * Mathf.Cos(radAzimuth);
        float y = radius * Mathf.Sin(radInclination) * Mathf.Sin(radAzimuth);
        float z = radius * Mathf.Cos(radInclination);
        return new Vector3(x, y, z) + centerOfSphere.position;
    }

    // Convert Cartesian coordinates to spherical coordinates
    public void CartesianToSpherical(Vector3 cartesianCoords, out float radius, out float inclination, out float azimuth)
    {
        radius = cartesianCoords.magnitude;
        inclination = Mathf.Acos(cartesianCoords.z / radius); // Polar angle (theta)
        azimuth = Mathf.Atan2(cartesianCoords.y, cartesianCoords.x); // Azimuthal angle (phi)
    }

    void Update()
    {
        // Convert spherical coordinates to Cartesian and move the object
        Vector3 cartesianPosition = SphericalToCartesian(radius, inclination, azimuth, centerOfSphere);
        transform.position = cartesianPosition;
    }
}