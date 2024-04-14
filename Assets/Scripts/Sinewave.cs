using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Sinewave : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numPoints;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private bool isHorizontal = false;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private void Start()
    {
        lineRenderer.GetComponent<LineRenderer>();
        DrawLine();
    }

    // Function representing the sine for a full cycle
    private void DrawLine()
    {

        // Set the number of points to draw
        lineRenderer.positionCount = numPoints;
        for (int currentPoint = 0; currentPoint < numPoints; currentPoint++)
        {
            float progress = (float)currentPoint / (numPoints - 1);
            Vector3 position = Vector3.Lerp(startPoint.position, endPoint.position, progress);
            float angle = progress * 2 * Mathf.PI * frequency - movementSpeed * Time.timeSinceLevelLoad;

            if (!isHorizontal)
            {
                float y = amplitude * Mathf.Sin(angle);
                lineRenderer.SetPosition(currentPoint, position + new Vector3(0, y, 0));
            }
            else
            {
                float x = amplitude * Mathf.Sin(angle);
                lineRenderer.SetPosition(currentPoint, position + new Vector3(x, 0, 0));
            }
            
        }

    }


    private void Update()
    {
        DrawLine();
    }
}
