using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SphericalCoordinatesHandler
{

    // Convert spherical coordinates to Cartesian coordinates
    public static Vector3 SphericalToCartesian(float radius, float inclination, float azimuth, Transform centerOfSphere)
    {
        float radInclination = inclination * Mathf.Deg2Rad; // Convert inclination from degrees to radians
        float radAzimuth = azimuth * Mathf.Deg2Rad; // Convert azimuth from degrees to radians
        
        float x = radius * Mathf.Sin(radInclination) * Mathf.Cos(radAzimuth);
        float y = radius * Mathf.Sin(radInclination) * Mathf.Sin(radAzimuth);
        float z = radius * Mathf.Cos(radInclination);
        return new Vector3(x, y, z) + centerOfSphere.position;
    }

    // Convert Cartesian coordinates to spherical coordinates
    public static void CartesianToSpherical(Vector3 cartesianCoords, out float radius, out float inclination, out float azimuth)
    {
        radius = cartesianCoords.magnitude;
        inclination = Mathf.Acos(cartesianCoords.z / radius); // Polar angle (theta)
        azimuth = Mathf.Atan2(cartesianCoords.y, cartesianCoords.x); // Azimuthal angle (phi)
    }

}