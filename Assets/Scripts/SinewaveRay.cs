using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SinewaveRay : BaseRay
{
    
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private bool isHorizontal = false;
    [SerializeField] private bool isInward = false;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private void Start()
    {
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
    }
}
