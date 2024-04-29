using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Obi;

public class SinewaveRay : BaseRay
{
    
    /*private ObiRope rope;
    private ObiSolver solver;*/
    
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private bool isHorizontal = false;
    [SerializeField] private bool isInward = false;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    /*private void Awake()
    {
        solver = gameObject.GetComponentInParent<ObiSolver>();
        rope = gameObject.GetComponent<ObiRope>();
        
    }*/
    private void Start()
    {
            
        /*Debug.Log("Solver: " + solver);
        Debug.Log("Solver: " + solver.positions);*/
        
        lineRenderer.GetComponent<LineRenderer>();
        DrawLine();
    }

    // Function representing the sine for a full cycle
    protected override void DrawLine()
    {

        // Set the number of points to draw
        lineRenderer.positionCount = numPoints;

        for (int currentPoint = 0; currentPoint < numPoints; currentPoint++)
        {
            float progress = (float)currentPoint / (numPoints - 1);
            Vector3 position = Vector3.Lerp(startPoint.position, endPoint.position, progress);

            // Handle sinewave moving outward or inward
            float angle;
            if (!isInward)
            {
                angle = progress * 2 * Mathf.PI * frequency - movementSpeed * Time.timeSinceLevelLoad;
            }
            else
            {
                angle = progress * 2 * Mathf.PI * frequency + movementSpeed * Time.timeSinceLevelLoad;
            }
            

            // Calculate the distance from the midpoint
            float distanceFromMidpoint = Mathf.Abs(progress - 0.5f) * 2; // Scaled to [0, 1]
            // Scale amplitude based on distance from midpoint
            float scaledAmplitude = amplitude * Mathf.Cos(distanceFromMidpoint * Mathf.PI / 2); // Cosine function for smooth scaling

            // Handle sinewave parallel to YZ plane or XZ plane
            if (!isHorizontal)
            {
                float y = scaledAmplitude * Mathf.Sin(angle);
                lineRenderer.SetPosition(currentPoint, position + new Vector3(0, y, 0));
            }
            else
            {
                float x = scaledAmplitude * Mathf.Sin(angle);
                lineRenderer.SetPosition(currentPoint, position + new Vector3(x, 0, 0));
            }
            
        }

    }
    

    private void Update()
    {
        DrawLine();
        
        // Get all particle positions
        // Update Obi Rope positions from LineRenderer
        /*for (int i = 0; i < rope.particleCount; i++)
        {
            if (i < lineRenderer.positionCount)
            {
                solver.positions[i] = lineRenderer.GetPosition(i);
                rope.SetParticlePosition(i, lineRenderer.GetPosition(i));
            }
            else
            {
                // If the number of LineRenderer points is less than the number of particles,
                // set the remaining particles to the last LineRenderer position
                rope.SetParticlePosition(i, lineRenderer.GetPosition(lineRenderer.positionCount - 1));
                Oni.SetParticlePositions(this.solver.Oni);
            }
        }*/
        
        
        
        
        
        
        // Get all particle positions from the LineRenderer
        /*Vector3[] linePositions = new Vector3[numPoints]; 
        lineRenderer.GetPositions(linePositions);
        
        // Update Obi Rope positions from LineRenderer
        for (int i = 0; i < linePositions.Length; i++)
        {
            // Ensure the particle index is within bounds
            if (i < rope.particleCount)
            {
                // Convert world position to local space of Obi Rope
                Vector3 localPosition = rope.transform.InverseTransformPoint(linePositions[i]);
                rope.solver.positions[i] = localPosition;
            }
        }*/
        
        //Debug.Log(rope.particleCount);
        
    }
}
