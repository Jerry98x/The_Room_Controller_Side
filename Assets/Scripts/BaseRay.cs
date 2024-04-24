using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRay : MonoBehaviour
{
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected int numPoints;

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
}
