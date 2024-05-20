using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SinewaveRay : BaseRay
{
    
    // Sinewave representing the ray of a "Neto" module
    
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private bool isHorizontal = false;
    [SerializeField] private bool isInward = false;

    private Renderer renderer;
    private Color initialColor;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    
    private float minClampingAmplitude = 0.05f;
    private float maxClampingAmplitude = 0.6f;
    
    private float minSpeed = 0.1f;
    private float maxSpeed = 0.5f;
    
    private float minFrequency = 1f;
    private float maxFrequency = 2f;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        initialColor = renderer.material.GetColor(EmissionColorId);
    }
    
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
        HandleEvents();
        DrawLine();
    }


    // Testing events for the sinewave by pressing keys
    private void HandleEvents()
    {
        
        // Sinewave amplitude
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            
            if (!this.amplitude.Equals(1f))
            {
                this.amplitude = 1f;
            }
            else
            {
                this.amplitude = 0.5f;
            }
        }
        // Sinewave frequency
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!this.frequency.Equals(4f))
            {
                this.frequency = 4f;
            }
            else
            {
                this.frequency = 2f;
            }
        }
        // Sinewave color
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (renderer != null && renderer.material != null)
            {
                if(renderer.material.GetColor(EmissionColorId) == initialColor)
                {
                    renderer.material.SetColor(EmissionColorId, Color.red);
                }
                else
                {
                    renderer.material.SetColor(EmissionColorId, initialColor);
                }
            }
        }
        
    }
    
    public bool IsHorizontal()
    {
        return isHorizontal;
    }
    
    
    public override Transform GetEndPoint()
    {
        return endPoint;
    }
    
    public override EndPoint GetEndPointObject()
    {
        return endPoint.GetComponent<EndPoint>();
    }
    
    
    public void SetAmplitude(float newAmplitude)
    {
        amplitude = Mathf.Clamp(Mathf.Abs(newAmplitude), minClampingAmplitude, maxClampingAmplitude);
        //amplitude = newAmplitude;;
    }
    
    public void SetSpeed(float newSpeed)
    {
        movementSpeed = Mathf.Clamp(Mathf.Abs(newSpeed), minSpeed, maxSpeed);
    }
    
    public void SetFrequency(float newFrequency)
    {
        frequency = Mathf.Clamp(Mathf.Abs(newFrequency), minFrequency, maxFrequency);
    }
    
}
