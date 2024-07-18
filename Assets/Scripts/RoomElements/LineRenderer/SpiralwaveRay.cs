using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents a Sauron ray as a spiralwave
/// </summary>
/// <remarks>
/// The spiralwave has different modifiable parameters such as number of loops, rotation speed, radius, and direction.
/// Its shape is supposed to suggest the rotatory movement that the Controller should perform with the VR controller
/// to move the Sauron module.
/// </remarks>
public class SpiralwaveRay : BaseRay
{

    
    [SerializeField] private int numberOfLoops = 1;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private bool isInward = false;
    [SerializeField] private bool isMirrored = false;
    
    
    //private Renderer renderer;
    private Color initialColor;
    //private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    
    
    private float minClampingRadius = 0.1f;
    private float maxClampingRadius = 1f;


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the line renderer and the initial color
    /// </summary>
    public void Awake()
    {
        //renderer = GetComponent<Renderer>();
        lineRenderer.GetComponent<LineRenderer>();
        initialColor = GetComponent<LineRenderer>().material.GetColor(Constants. EMISSIVE_COLOR_ID);
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
    /// Concretely draws the spiralwave through a line renderer. 
    /// </summary>
    /// <remarks>
    /// The drawing of the line is done by computing each of its points and handling all its characteristics.
    /// It sets the position of each point based on the spiralwave radius and rotation.
    /// </remarks>
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
            
            
            // Rotate around the "up" axis
            Vector3 offset = rotation * Vector3.up * scaledRadius;

            position += offset;

            lineRenderer.SetPosition(currentPoint, position);
        }
        
    }

    
    /// <summary>
    /// Handles the events related to the sinewave such as changing its radius, number of loops, and color
    /// </summary>
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
            if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
            {
                if(GetComponent<Renderer>().material.GetColor(Constants.EMISSIVE_COLOR_ID) == initialColor)
                {
                    GetComponent<Renderer>().material.SetColor(Constants.EMISSIVE_COLOR_ID, Color.green);
                }
                else
                {
                    GetComponent<Renderer>().material.SetColor(Constants.EMISSIVE_COLOR_ID, initialColor);
                }
            }
        }
    }

    #endregion



    #region Getters and setters

    /// <summary>
    /// Returns the startpoint of the spiralwave
    /// </summary>
    public override Transform GetStartPoint()
    {
        return startPoint;
    }

    /// <summary>
    /// Returns the endpoint of the spiralwave
    /// </summary>
    public override Transform GetEndPoint()
    {
        return endPoint;
    }
    
    /// <summary>
    /// Returns the endpoint object
    /// </summary>
    public override RayEndPoint GetEndPointObject()
    {
        return endPoint.GetComponent<RayEndPoint>();
    }
    
    
    /// <summary>
    /// Returns the radius of the spiralwave
    /// </summary>
    public float GetRadius()
    {
        return radius;
    }

    /// <summary>
    /// Returns the numeber of loops of the spiralwave
    /// </summary>
    public float GetNumberOfLoops()
    {
        return numberOfLoops;
    }
    
    
    /// <summary>
    /// Set the radius after clamping it
    /// </summary>
    /// <param name="newRadius"> The updated radius of the spiralwave </param>
    public void SetRadius(float newRadius)
    {
        radius = Mathf.Clamp(newRadius, minClampingRadius, maxClampingRadius);
    }
    
    /// <summary>
    /// Reset the radius to the minimum value
    /// </summary>
    public void ResetRadius()
    {
        radius = minClampingRadius;
    }    

    #endregion
    
   
}
