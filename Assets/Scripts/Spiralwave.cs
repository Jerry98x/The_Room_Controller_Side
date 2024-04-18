using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiralwave : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numPoints;
    [SerializeField] private int numberOfLoops = 1;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private bool isClockwise = false;
    
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private void Start()
    {
        lineRenderer.GetComponent<LineRenderer>();
        DrawLine();
    }

    // Function representing the spiral for a full cycle
    private void DrawLine()
    {
        
        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        
        
        float totalDistance = Vector3.Distance(startPoint.position, endPoint.position);
        float deltaTheta = 360f * numberOfLoops / numPoints;
        float deltaDistance = totalDistance / numPoints;
        

        Quaternion rotation;
        
        // Set the number of points to draw
        lineRenderer.positionCount = numPoints;
        
        
        for (int currentPoint = 0; currentPoint < numPoints; currentPoint++)
        {
            // Calculate position along the direction vector
            float distanceAlongDirection = currentPoint * deltaDistance;
            Vector3 position = startPoint.position + direction * distanceAlongDirection;

            // Calculate the current angle around the direction vector
            float currentTheta = currentPoint * deltaTheta;
            
            // Calculate the distance from the midpoint
            float distanceFromMidpoint = Mathf.Abs((float)currentPoint / numPoints - 0.5f) * 2; // Scaled to [0, 1]
            // Scale radius based on distance from midpoint
            float scaledRadius = radius * Mathf.Cos(distanceFromMidpoint * Mathf.PI / 2); // Cosine function for smooth scaling

            
            // Apply a rotation around the direction vector
            if (!isClockwise)
            {
                rotation = Quaternion.AngleAxis(rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }
            else
            {
                rotation = Quaternion.AngleAxis(-rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }

            // Calculate the offset from the calculated position along the direction vecto
            Vector3 offset = rotation * Vector3.up * scaledRadius; // Rotate around the "up" axis
            
            position += offset;

            lineRenderer.SetPosition(currentPoint, position);
        }
        

    }


    private void Update()
    {
        DrawLine();
    }
    
}
