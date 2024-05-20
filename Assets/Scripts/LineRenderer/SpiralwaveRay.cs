using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SpiralwaveRay : BaseRay
{
    
    // Spiralwave representing the ray of a "Sauron" module
    
    [SerializeField] private int numberOfLoops = 1;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private bool isInward = false;
    [SerializeField] private bool isMirrored = false;
    
    
    private Renderer renderer;
    private Color initialColor;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    
    
    private float minClampingRadius = 0.1f;
    private float maxClampingRadius = 1f;

    public void Awake()
    {
        renderer = GetComponent<Renderer>();
        initialColor = renderer.material.GetColor(EmissionColorId);
    }
    

    private void Start()
    {
        lineRenderer.GetComponent<LineRenderer>();
        DrawLine();
    }

    // Function representing the spiral for a full cycle
    protected override void DrawLine()
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
            if (isInward && isMirrored)
            {
                rotation = Quaternion.AngleAxis(-rotationSpeed * Time.timeSinceLevelLoad - currentTheta, direction);
            }
            else if (isInward && !isMirrored)
            {
                rotation = Quaternion.AngleAxis(rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }
            else if (!isInward && isMirrored)
            {
                rotation = Quaternion.AngleAxis(rotationSpeed * Time.timeSinceLevelLoad - currentTheta, direction);
            }
            else
            {
                rotation = Quaternion.AngleAxis(-rotationSpeed * Time.timeSinceLevelLoad + currentTheta, direction);
            }
            
            
            Vector3 offset = rotation * Vector3.up * scaledRadius; // Rotate around the "up" axis

            position += offset;

            lineRenderer.SetPosition(currentPoint, position);
        }
        

    }


    private void Update()
    {
        HandleEvents();
        DrawLine();
    }

    
    // Testing events for the spiralwave by pressing keys
    private void HandleEvents()
    {
        // Spiralwave radius
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            
            if (!this.radius.Equals(0.2f))
            {
                this.radius = 0.2f;
            }
            else
            {
                this.radius = 0.05f;
            }
        }
        // Spiralwave number of loops
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (!this.numberOfLoops.Equals(8))
            {
                this.numberOfLoops = 8;
            }
            else
            {
                this.numberOfLoops = 5;
            }
        }
        // Spiralwave color
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (renderer != null && renderer.material != null)
            {
                if(renderer.material.GetColor(EmissionColorId) == initialColor)
                {
                    renderer.material.SetColor(EmissionColorId, Color.green);
                }
                else
                {
                    renderer.material.SetColor(EmissionColorId, initialColor);
                }
            }
        }
    }
    
    
    public override Transform GetEndPoint()
    {
        return endPoint;
    }
    
    public override EndPoint GetEndPointObject()
    {
        return endPoint.GetComponent<EndPoint>();
    }
    
    
    public void SetRadius(float newRadius)
    {
        radius = Mathf.Clamp(newRadius, minClampingRadius, maxClampingRadius);
    }
    
    public void ResetRadius()
    {
        radius = minClampingRadius;
    }
    
    
    
}
