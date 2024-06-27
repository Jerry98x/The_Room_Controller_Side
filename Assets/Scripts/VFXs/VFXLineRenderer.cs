using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXLineRenderer : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private LineRenderer lineRenderer;
    
    private ParticleSystem particleSystem;
    

    //private List<Vector3> positions = new List<Vector3>();

    
    private ComputeBuffer computeBuffer;
    private GraphicsBuffer graphicsBuffer;
    private Vector3[] positions;
    private int maxParticles = 100;
    private VFXParameterBinder binder;
    


    void Start()
    {
        // Initialize the compute buffer
        computeBuffer = new ComputeBuffer(maxParticles, sizeof(float) * 3);
        positions = new Vector3[maxParticles];

        // Create a GraphicsBuffer from the ComputeBuffer
        graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, computeBuffer.count, sizeof(float) * 3);
        computeBuffer.GetData(positions);
        graphicsBuffer.SetData(positions);

        // Ensure the visualEffect reference is set
        if (vfx == null)
        {
            vfx = GetComponent<VisualEffect>();
        }

        // Add and configure the VFXParameterBinder
        binder = vfx.gameObject.AddComponent<VFXParameterBinder>();
        binder.BindGraphicsBuffer("PositionBuffer", graphicsBuffer);
    }

    void Update()
    {
        // Retrieve data from the compute buffer
        computeBuffer.GetData(positions);

        // Update the GraphicsBuffer with the new data
        graphicsBuffer.SetData(positions);

        // Update the LineRenderer positions
        int particleCount = Mathf.Min(positions.Length, lineRenderer.positionCount);
        for (int i = 0; i < particleCount; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }
    
    void OnDestroy()
    {
        // Release the compute buffer and graphics buffer
        computeBuffer.Release();
        graphicsBuffer.Release();
    }
    
    
    
}
