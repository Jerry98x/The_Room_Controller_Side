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
            // Calcoliamo la posizione lungo il vettore direzione
            float distanceAlongDirection = currentPoint * deltaDistance;
            Vector3 position = startPoint.position + direction * distanceAlongDirection;

            // Calcoliamo l'angolo attuale intorno al vettore direzione
            float currentTheta = currentPoint * deltaTheta;

            // Applichiamo una rotazione attorno al vettore direzione
            if (!isClockwise)
            {
                rotation = Quaternion.AngleAxis(rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }
            else
            {
                rotation = Quaternion.AngleAxis(-rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }

            // Calcoliamo l'offset dalla posizione calcolata lungo il vettore direzione
            Vector3 offset = rotation * Vector3.up * radius; // Ruotiamo attorno all'asse "up"

            // Aggiorniamo la posizione con l'offset
            position += offset;

            lineRenderer.SetPosition(currentPoint, position);
        }
        

    }


    private void Update()
    {
        DrawLine();
    }
    
}
