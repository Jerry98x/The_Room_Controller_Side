using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Class that makes a particle system follow a sinewave path defined by a LineRenderer.
/// </summary>
public class ParticlePathFollower : MonoBehaviour
{
    
    [SerializeField] private SinewaveRay sinewaveRay;
    // Coincide with the Neto endpoint (not to be confused with the LineRenderer's endpoint, which is called "NetoRayEndpoint")
    // and more specifically it's also the same attractor point that attracts the portal's particles
    [SerializeField] private Transform distantPoint;
    private LineRenderer sinewaveLineRenderer;
    private ParticleSystem partSystem;
    private ParticleSystem.Particle[] particles;
    private Vector3[] linePositions;

    private Vector3 particleDirection;
    


    private void Awake()
    {
        partSystem = GetComponent<ParticleSystem>();
        sinewaveLineRenderer = sinewaveRay.GetComponent<LineRenderer>();
    }

    void Start()
    {
        particles = new ParticleSystem.Particle[partSystem.main.maxParticles];
        // Initialize line positions array
        linePositions = new Vector3[sinewaveLineRenderer.positionCount];
        sinewaveLineRenderer.GetPositions(linePositions);

        // Set the particle system's position to the starting position of the sinewave
        transform.position = sinewaveRay.GetStartPoint().position;
        
        SetParticleSystemDirection();
    }

    void LateUpdate()
    {
        RepositionParticles();
        
    }
    
    
    /// <summary>
    /// Changes the position of the particles in the particle system based on the sinewave path.
    /// </summary>
    private void RepositionParticles()
    {
        int linePointCount = sinewaveLineRenderer.positionCount;
        linePositions = new Vector3[linePointCount];
        sinewaveLineRenderer.GetPositions(linePositions);

        particles = new ParticleSystem.Particle[partSystem.main.maxParticles];
        int numParticlesAlive = partSystem.GetParticles(particles);

        Vector3 lineEndPosition = linePositions[linePointCount - 1];
        float speed = partSystem.main.startSpeed.constant;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            float lifetimePercentage = 1 - (particles[i].remainingLifetime / particles[i].startLifetime);

            if (lifetimePercentage < 0.5f)
            {
                // First half of the lifetime: follow the LineRenderer
                float t = lifetimePercentage * 2;
                particles[i].position = InterpolateLinePosition(linePositions, t);
            }
            else
            {
                // Second half of the lifetime: move towards the distant point
                float t = (lifetimePercentage - 0.5f) * 2;
                Vector3 directionToTarget = (distantPoint.position - lineEndPosition).normalized;
                float distanceToTarget = Vector3.Distance(lineEndPosition, distantPoint.position);

                particles[i].position = lineEndPosition + directionToTarget * (t * distanceToTarget);
            }
        }

        partSystem.SetParticles(particles, numParticlesAlive);
    }
    
    
    
    /// <summary>
    /// Interpolates between the positions of the LineRenderer to get the position of the particle.
    /// </summary>
    /// <param name="linePos"> Array containing the positions of linerenderer </param>
    /// <param name="t"> Time parameter </param>
    /// <returns></returns>
    private Vector3 InterpolateLinePosition(Vector3[] linePos, float t)
    {
        
        int pointCount = linePos.Length;
        if (pointCount == 0)
        {
            return Vector3.zero;
        }
        
        // Scale t to the number of line segments
        t = Mathf.Clamp01(t) * (pointCount - 1);
        
        // Find the two points we're interpolating between
        int startIndex = Mathf.FloorToInt(t);
        int endIndex = Mathf.CeilToInt(t);
        
        // Handle corner cases
        if (startIndex >= pointCount - 1)
        {
            startIndex = pointCount - 2;
        }
        if (endIndex >= pointCount)
        {
            endIndex = pointCount - 1;
        }
        
        // Local interpolation factor
        float localT = t - startIndex;
        
        // Interpolate between the two points
        Vector3 finalPos = Vector3.Lerp(linePos[startIndex], linePos[endIndex], localT);
        return finalPos;
        
    }
    
    
    private void SetParticleSystemDirection()
    {
        particleDirection = sinewaveRay.GetEndPoint().position - transform.position;
        partSystem.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
}
