using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : BaseRay
{
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private bool isHorizontal = false;
    [SerializeField] private bool isInward = false;
    [SerializeField] private float gravity = 0.1f;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.25f;
    private int segmentLength = 35;
    private float stiffness = 0.01f;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = startPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector3 forceGravity = new Vector3(0f, -gravity, 0f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;

            // Add sinewave effect
            float angle = (float)i / this.segmentLength * 2 * Mathf.PI * frequency - movementSpeed * Time.timeSinceLevelLoad;
            float sinValue = amplitude * Mathf.Sin(angle);

            if (isHorizontal)
                firstSegment.posNow += new Vector3(sinValue, 0, 0);
            else
                firstSegment.posNow += new Vector3(0, sinValue, 0);

            this.ropeSegments[i] = firstSegment;
        }

        // CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        // First segment
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = startPoint.position;
        this.ropeSegments[0] = firstSegment;

        // Last segment
        RopeSegment endSegment = ropeSegments[ropeSegments.Count - 1];
        endSegment.posNow = endPoint.position;
        ropeSegments[ropeSegments.Count - 1] = endSegment;

        // Two points and the rope will always keep a certain distance apart
        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
            
            // Calculate the difference from the resting length
            float diff = dist - ropeSegLen;
            // Apply stiffness to the movement
            Vector3 force = stiffness * diff * changeDir;
            
            // Apply forces to the segments
            firstSeg.posNow += force;
            secondSeg.posNow -= force;
        }
    }

    protected override void DrawLine()
    {
        // Set width of the line
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Set the number of points to draw
        lineRenderer.positionCount = segmentLength;

        for (int currentPoint = 0; currentPoint < segmentLength; currentPoint++)
        {
            // Get the current position of the rope segment
            Vector3 position = this.ropeSegments[currentPoint].posNow;

            // Update the position of the line renderer
            lineRenderer.SetPosition(currentPoint, position);
        }
    }

    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
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
}