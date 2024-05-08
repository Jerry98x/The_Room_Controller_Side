using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRay : MonoBehaviour
{
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected int numPoints;
    
    [SerializeField] protected Transform startPoint;
    [SerializeField] protected Transform endPoint;
    
    protected abstract void DrawLine();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawLine();
    }

    private void Update()
    {
        DrawLine();
    }

    public abstract Transform GetEndPoint();

    public abstract EndPoint GetEndPointObject();
}
