using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a generic ray
/// </summary>
public abstract class BaseRay : MonoBehaviour
{
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected int numPoints;
    
    [SerializeField] protected Transform startPoint;
    [SerializeField] protected Transform endPoint;


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the line renderer and draws the ray at the start of the scene
    /// </summary>
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawLine();
    }

    /// <summary>
    /// Draws the ray at each frame
    /// </summary>
    private void Update()
    {
        DrawLine();
    }

    #endregion
    




    #region Relevant functions

    /// <summary>
    /// Concretely draws the ray through a line renderer
    /// </summary>
    protected abstract void DrawLine();
    
    #endregion
    
    
    
    #region Getters and setters

    /// <summary>
    /// Returns the endpoint of the ray
    /// </summary>
    public abstract Transform GetEndPoint();

    /// <summary>
    /// Returns the endpoint object
    /// </summary>
    public abstract EndPoint GetEndPointObject();

    #endregion
    

}
