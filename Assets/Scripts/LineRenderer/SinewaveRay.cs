using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents a Neto ray as a sinewave
/// </summary>
/// <remarks>
/// The sinewave has different modifiable parameters such as amplitude, frequency, speed, and inclination.
/// Its shape is supposed to suggest the linear movement that the Controller should perform with the VR controller
/// to move the Neto module.
/// </remarks>
public class SinewaveRay : BaseRay
{
    
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    //[SerializeField] private bool isHorizontal = false;
    [Range(0, 180)]
    [SerializeField] private float inclination = 0f;
    [SerializeField] private bool isInward = false;

    //private Renderer renderer;
    private Color initialColor;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    
    private float minClampingAmplitude = 0.05f;
    private float maxClampingAmplitude = 0.6f;
    
    private float minSpeed = 0.1f;
    private float maxSpeed = 0.5f;
    
    private float minFrequency = 1f;
    private float maxFrequency = 2f;
    

    
    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the line renderer and the initial color
    /// </summary>
    private void Awake()
    {
        /*renderer = GetComponent<Renderer>();*/
        lineRenderer.GetComponent<LineRenderer>();
        initialColor = GetComponent<LineRenderer>().material.GetColor(EmissionColorId);
    }
    
    /// <summary>
    /// Draws the sinewave at the start of the scene
    /// </summary>
    private void Start()
    {
        DrawLine();
    }
    
    
    /// <summary>
    /// Handles the events and draws the line at each frame
    /// </summary>
    private void Update()
    {
        HandleEvents();
        DrawLine();
    }

    #endregion



    #region Relevant functions

    /// <summary>
    /// Concretely draws the sinewave through a line renderer. 
    /// </summary>
    /// <remarks>
    /// The drawing of the line is done by computing each of its points and handling all its characteristics.
    /// It sets the x and y coordinates of each point based on the sinewave function.
    /// </remarks>
    protected override void DrawLine()
    {

        // Set the number of points to draw
        lineRenderer.positionCount = numPoints;
        
        // Convert inclination from degrees to radians
        float inclinationRad = inclination * Mathf.Deg2Rad;


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

            
            // Calculate the x and y coordinates of the sinusoid along the inclined line
            float x = scaledAmplitude * Mathf.Sin(angle) * Mathf.Cos(inclinationRad);
            float y = scaledAmplitude * Mathf.Sin(angle) * Mathf.Sin(inclinationRad);
            
            // Set the position of the line renderer using the new coordinates
            lineRenderer.SetPosition(currentPoint, position + new Vector3(x, y, 0));
            
            
        }

    }
    


    /// <summary>
    /// Handles the events related to the sinewave such as changing its amplitude, frequency, and color
    /// </summary>
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
            if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
            {
                if(GetComponent<Renderer>().material.GetColor(EmissionColorId) == initialColor)
                {
                    GetComponent<Renderer>().material.SetColor(EmissionColorId, Color.red);
                }
                else
                {
                    GetComponent<Renderer>().material.SetColor(EmissionColorId, initialColor);
                }
            }
        }
        
    }

    #endregion
    
    


    #region Getters and setters

    /// <summary>
    /// Returns the endpoint of the sinewave
    /// </summary>
    public override Transform GetEndPoint()
    {
        return endPoint;
    }
    
    /// <summary>
    /// Returns the endpoint object
    /// </summary>
    public override EndPoint GetEndPointObject()
    {
        return endPoint.GetComponent<EndPoint>();
    }
    
    /// <summary>
    /// Returns the amplitude of the sinewave
    /// </summary>
    public float GetAmplitude()
    {
        return amplitude;
    }
    
    /// <summary>
    /// Returns the speed of the sinewave
    /// </summary>
    public float GetSpeed()
    {
        return movementSpeed;
    }
    
    /// <summary>
    /// Returns the frequency of the sinewave
    /// </summary>
    public float GetFrequency()
    {
        return frequency;
    }
    
    /// <summary>
    /// Returns the inclination of the sinewave
    /// </summary>
    public float GetInclination()
    {
        return inclination;
    }
    
    
    /// <summary>
    /// Sets the amplitude of the sinewave after clamping it
    /// </summary>
    /// /// <param name="newAmplitude"> The updated amplitude for the sinewave </param>
    public void SetAmplitude(float newAmplitude)
    {
        amplitude = Mathf.Clamp(Mathf.Abs(newAmplitude), minClampingAmplitude, maxClampingAmplitude);
        //amplitude = newAmplitude;;
    }
    
    /// <summary>
    /// Sets the speed of the sinewave after clamping it
    /// </summary>
    /// <param name="newSpeed"> The updated speed for the sinewave </param>
    public void SetSpeed(float newSpeed)
    {
        movementSpeed = Mathf.Clamp(Mathf.Abs(newSpeed), minSpeed, maxSpeed);
    }
    
    /// <summary>
    /// Sets the inclination of the sinewave after clamping it
    /// </summary>
    /// <param name="newFrequency"> The updated frequency for the sinewave </param>
    public void SetFrequency(float newFrequency)
    {
        frequency = Mathf.Clamp(Mathf.Abs(newFrequency), minFrequency, maxFrequency);
    }

    #endregion
    

}
