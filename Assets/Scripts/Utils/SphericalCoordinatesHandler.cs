using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class to convert spherical coordinates to Cartesian coordinates and vice versa
/// </summary>
public static class SphericalCoordinatesHandler
{

    /// <summary>
    /// Convert spherical coordinates to Cartesian coordinates
    /// </summary>
    /// <param name="radius"> Distance from the center of the sphere </param>
    /// <param name="inclination"> Angle between the horizontal plane and the line connecting the central point to the specific point in space. </param>
    /// <param name="azimuth"> Angle between the north direction on the horizontal plane and the projection of the line connecting the central point to the specific point in space onto this plane. </param>
    /// <param name="centerOfSphere"> The core center of the Room </param>
    /// <returns> Cartesian coordinate </returns>
    public static Vector3 SphericalToCartesian(float radius, float inclination, float azimuth, Transform centerOfSphere)
    {
        float radInclination = inclination * Mathf.Deg2Rad; // Convert inclination from degrees to radians
        float radAzimuth = azimuth * Mathf.Deg2Rad; // Convert azimuth from degrees to radians
        
        float x = radius * Mathf.Sin(radInclination) * Mathf.Cos(radAzimuth);
        float y = radius * Mathf.Sin(radInclination) * Mathf.Sin(radAzimuth);
        float z = radius * Mathf.Cos(radInclination);
        return new Vector3(x, y, z) + centerOfSphere.position;
    }

    /// <summary>
    /// Convert Cartesian coordinates to spherical coordinates
    /// </summary>
    /// <param name="cartesianCoords"> Input Cartesian coordinates </param>
    /// <param name="radius"></param>
    /// <param name="inclination"></param>
    /// <param name="azimuth"></param>
    public static void CartesianToSpherical(Vector3 cartesianCoords, out float radius, out float inclination, out float azimuth)
    {
        radius = cartesianCoords.magnitude;
        inclination = Mathf.Acos(cartesianCoords.z / radius); // Polar angle (theta)
        azimuth = Mathf.Atan2(cartesianCoords.y, cartesianCoords.x); // Azimuthal angle (phi)
    }

}