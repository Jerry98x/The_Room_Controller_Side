using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        Debug.Log("Particle system start position: " + transform.position);
        
        SetParticleSystemDirection();
    }

    void LateUpdate()
    {
        RepositionParticles();
        
        //UpdateParticles();
    }
    
    
    
    /*private void RepositionParticles()
    {
        // Take the set of positions of the sinewave linerenderer's points
        // These positions should be already updated at each frame
        int linePointCount = sinewaveLineRenderer.positionCount;
        linePositions = new Vector3[linePointCount];
        sinewaveLineRenderer.GetPositions(linePositions);

        
        // Get the current particles from the particle system
        particles = new ParticleSystem.Particle[partSystem.main.maxParticles];
        int numParticlesAlive = partSystem.GetParticles(particles);
        
        
        // Get the last position of the LineRenderer
        Vector3 lineEndPosition = linePositions[linePointCount - 1];
        
        
        // Update each particle's position based on the sinewave's line renderer
        for (int i = 0; i < numParticlesAlive; i++)
        {
     
            // Calculate the particle's lifetime percentage
            float lifetimePercentage = 1 - (particles[i].remainingLifetime / particles[i].startLifetime);

            
            // Determine the corresponding position on the sine wave
            Vector3 newPosition = InterpolateLinePosition(linePositions, lifetimePercentage);
            
            
            // If the particle has reached the end of the LineRenderer, move towards the target point
            if (lifetimePercentage >= 1)
            {
                
                float speed = partSystem.main.startSpeed.constant;
                newPosition = lineEndPosition + speed * (1 - particles[i].remainingLifetime / particles[i].startLifetime) * particleDirection;
            }
            
            
            particles[i].position = newPosition;
            
            
        }


        
        partSystem.SetParticles(particles, numParticlesAlive);
    }*/
    
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
        Debug.Log("Start index: " + startIndex);
        Debug.Log("End index: " + endIndex);
        
        Debug.Log("Position at startIndex: " + linePos[startIndex]);
        Debug.Log("Position at endIndex: " + linePos[endIndex]);
        
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
        Debug.Log("LOCAL T: " + localT);
        
        // Interpolate between the two points
        Vector3 finalPos = Vector3.Lerp(linePos[startIndex], linePos[endIndex], localT);
        Debug.Log("Final position: " + finalPos);
        return finalPos;
        
    }
    
    
    
    


    
    
    
    
    // TODO: remove this function after obtaining the desired behaviour with a new one
    private void UpdateParticles()
    {
        
        // Update the line positions array size if needed
        if (linePositions.Length != sinewaveLineRenderer.positionCount)
        {
            linePositions = new Vector3[sinewaveLineRenderer.positionCount];
        }
        
        
        // Update the line positions from the LineRenderer
        sinewaveLineRenderer.GetPositions(linePositions);

        // Get the current particles from the particle system
        int numParticlesAlive = partSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Calculate the distance the particle has traveled based on its lifetime
            float normalizedLifetime = 1 - (particles[i].remainingLifetime / particles[i].startLifetime);
            float distanceTraveled = normalizedLifetime * sinewaveLineRenderer.positionCount;

            // Find the two points in the line positions array the particle is between
            int startIdx = Mathf.FloorToInt(distanceTraveled);
            int endIdx = Mathf.CeilToInt(distanceTraveled);

            if (startIdx >= linePositions.Length)
            {
                startIdx = linePositions.Length - 1;
            }

            if (endIdx >= linePositions.Length)
            {
                endIdx = linePositions.Length - 1;
            }

            // Calculate the interpolation factor
            float t = distanceTraveled - startIdx;

            // Interpolate between the two points to get the particle's new position
            particles[i].position = Vector3.Lerp(linePositions[startIdx], linePositions[endIdx], t);

        }

        // Apply the particle positions to the particle system
        partSystem.SetParticles(particles, numParticlesAlive);
    }
    
    
    
    
    private void SetParticleSystemDirection()
    {
        Debug.Log("End position: " + sinewaveRay.GetEndPoint().position);
        particleDirection = sinewaveRay.GetEndPoint().position - transform.position;
        partSystem.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
}
