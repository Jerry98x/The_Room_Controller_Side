using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

// NOT USED

/// <summary>
/// Class that handles the raycast interaction with the modules of the Room and updates the rays' characteristics
/// </summary>
/// <remarks>
/// Used for the second version of the Room, where action/movement is on the ray itself
/// (the movement is reflected in the change in the characteristics of the ray) and perception of the Visitor
/// is on the endpoint (visual effects are applied near the endpoint of the ray)
/// </remarks>
/*public class HandleRayMovement : MonoBehaviour
{

    [SerializeField] private Transform coreCenter;
    
    //public XRController handController;
    private XRRayInteractor rayInteractor;
    private Pointer pointer;
    //private Vector3 hitPoint;
    
    SinewaveRay sinewaveRay;
    SpiralwaveRay spiralwaveRay;
    
    private Vector3 previousPointerPosition;
    private float previousTime;
    
    
    /*private Queue<float> recentSpeeds = new Queue<float>();
    private const int maxRecentSpeedsCount = 100;#1#

    
    // Variables for simple version with amplitude based on angles
    // minAmplitude can match the minClampingAmplitude in SinewaveRay, maxAmplitude needs to be fine-tuned
    private float minAmplitude = 0.05f;
    private float maxAmplitude = 10f;
    /*private float minSpeed = 0.1f;
    private float maxSpeed = 0.5f;
    private float minFrequency = 1f;
    private float maxFrequency = 4f;#1#
    
    // Variables for simple version with radius based on angles
    // minRadius can match the minClampingRadius in SpiralwaveRay, maxRadius needs to be fine-tuned
    private float minRadius = 0.1f;
    private float maxRadius = 5f;
    
    
    private bool isTracking = false;
    //private bool isInitial = true;


    // Variables for simple version with amplitude based on mere positions
    
    /*private float initialHandYPosition;
    private float initialHandXPosition;
    private Vector3 previousHandPosition = Vector3.zero;
    private float previousTime;#1#
    
    
    
    
    // Variables for circular buffer version
    
    /*private const int bufferSize = 100; // Size of the circular buffer
    private Vector3[] handPositionBuffer = new Vector3[bufferSize]; // Circular buffer to store hand positions
    private int bufferIndex = 0; // Index to keep track of the current position in the buffer#1#




    // Variables for custom function version first attempt
    
    /*private Vector3 savedMaxPosition = Vector3.zero;
    private bool isInitial = true;
    private bool isFirstDirection = false;
    private bool isFirstHalf = false;
    private bool isPositiveDirection = false;
    private Vector3 initialDirection = Vector3.zero;#1#



    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the ray interactor and the pointer
    /// </summary>
    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
        
        pointer = rayInteractor.GetComponentInChildren<Pointer>();
    }
    
    
    /// <summary>
    /// Updates the rays' characteristics at each frame
    /// </summary>
    private void Update()
    {
        if (isTracking)
        {
            if(sinewaveRay != null)
            {
                UpdateSinewaveRayByAngle();
            }
            if(spiralwaveRay != null)
            {
                UpdateSpiralwaveRayByAngle();
            }
        }
        else
        {
            //isInitial = true;
        }
    }

    #endregion



    #region Relevant functions

    /// <summary>
    /// Handles the event when the hand enters the ray and find if the ray is a SinewaveRay or a SpiralwaveRay
    /// </summary>
    /// <param name="args"> Event data </param>
    public void HandleHoverEntered(HoverEnterEventArgs args)
    {
        // Set the amplitude of the ray
        SinewaveRay sine = args.interactable.GetComponentInChildren<SinewaveRay>();
        //hitPoint = rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit) ? hit.point : Vector3.zero;
        if (sine != null)
        {
            Debug.Log("SinewaveRay");
            sinewaveRay = sine;
        }
        SpiralwaveRay spiral = args.interactable.GetComponentInChildren<SpiralwaveRay>();
        if (spiral != null)
        {
            spiralwaveRay = spiral;
        }
        
        
        /*initialHandYPosition = pointer.transform.position.y;
        initialHandXPosition = pointer.transform.position.x;#1#
        isTracking = true;
    }
    
    
    /// <summary>
    /// Handles the event when the hand exits the ray
    /// </summary>
    /// <param name="args"> Event data </param>
    public void HandleHoverExited(HoverExitEventArgs args)
    {
        // Reset the rays
        sinewaveRay = null;
        spiralwaveRay = null;
        
        isTracking = false;

    }
    
    
    
    
    
    
    
    
    /// <summary>
    /// Takes the angle between the direction of the ray and the direction of the hand
    /// and maps it to the sinewave ray's amplitude, updating it accordingly
    /// </summary>
    /// <remarks>
    /// Simple version with amplitude based on relative angles
    /// </remarks>
    private void UpdateSinewaveRayByAngle()
    {
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;

        // Calculate the vectors from the coreCenter to the endPoint and the pointer
        Vector3 vectorToEndpoint = endPointPosition - coreCenter.position;
        Vector3 vectorToPointer = pointer.transform.position - coreCenter.position;
        
        // Calculate the angle between the two vectors
        float angleDifference = Mathf.Abs(Vector3.Angle(vectorToEndpoint, vectorToPointer));

        // Map the angle difference to the amplitude range
        float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, angleDifference / 180f); // Dividing by 180 because the maximum difference between two angles is 180 degrees

        // Update the sinewave ray's amplitude based on the calculated amplitude
        sinewaveRay.SetAmplitude(amplitude);
    }
    
    
    

    
    /// <summary>
    /// Takes the distance between the hand and the endPoint of the ray (which is radially centered)
    /// and maps it to the sinewave ray's amplitude, updating it accordingly
    /// </summary>
    /// <remarks>
    /// Simple version with amplitude based on mere positions
    /// </remarks>
    private void UpdateSinewaveRayByPosition()
    {

        float handXPosition;
        float handYPosition;

        float relativeXPosition;
        float relativeYPosition;
        float relativePosition;

        float amplitude;
        
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        /*sinewaveRay.GetEndPoint().GetLocalPositionAndRotation(out Vector3 endPointPos, out Quaternion endPointRot);
        Vector3 endPointPosition = endPointPos;#1#
        
        // Get the current position of the hand
        handXPosition = pointer.transform.position.x;
        handYPosition = pointer.transform.position.y;
        // Calculate the relative position of the hand to the endPoint
        relativeXPosition = handXPosition - endPointPosition.x;
        relativeYPosition = handYPosition - endPointPosition.y;
        relativePosition = Mathf.Sqrt(Mathf.Pow(relativeXPosition, 2) + Mathf.Pow(relativeYPosition, 2));
        
        amplitude = Mathf.Abs(relativePosition);
        
        /*if(sinewaveRay.IsHorizontal())
        {
            // Get the current position of the hand
            handXPosition = pointer.transform.position.x;
            // Calculate the relative position of the hand to the endPoint
            relativeXPosition = handXPosition - endPointPosition.x;
            Debug.Log("Horizontal");
            Debug.Log("Hand X Position: " + handXPosition);
            Debug.Log("End Point X Position: " + endPointPosition.x);
            Debug.Log("Relative X Position: " + relativeXPosition);
            amplitude = Mathf.Abs(relativeXPosition);
            //amplitude = Mathf.InverseLerp(endPointPosition.x - maxElongation, endPointPosition.x + maxElongation, Mathf.Abs(relativeXPosition));
            Debug.Log("Amplitude determined by X Position: " + amplitude);
        }
        else
        {
            handYPosition = pointer.transform.position.y;
            relativeYPosition = handYPosition - endPointPosition.y;
            Debug.Log("Vertical");
            Debug.Log("Hand Y Position: " + handYPosition);
            Debug.Log("End Point Y Position: " + endPointPosition.y);
            Debug.Log("Relative Y Position: " + relativeYPosition);
            amplitude = Mathf.Abs(relativeYPosition);
            //amplitude = Mathf.Lerp(0.1f, 0.8f, Mathf.InverseLerp(endPointPosition.y - maxElongation, endPointPosition.y + maxElongation, Mathf.Abs(relativeYPosition))) ;
            Debug.Log("Amplitude determined by Y Position: " + amplitude);
        }#1#
        
        // Consider endpointposition + maxelongation and endpointposition - maxelongation as a lerp
        
        

        // Clamp the amplitude to a range you decide
        //amplitude = Mathf.Clamp(amplitude, minAmplitude, maxAmplitude);

        // Update the sinewave ray's amplitude based on the hand amplitude
        sinewaveRay.SetAmplitude(amplitude);

    }
    
    
    
    
    
    
    
    //TODO: see if there is a way to make the version with the circular buffer storing a history of hand position work
    //TODO: and if there is a way to make the custom version with all the if-else statements work;
    //TODO: otherwise remove them and keep the simple version with amplitude based on angles
    
    
    
    // Circular buffer version
    
    /*private void UpdateSinewaveRay()
    {
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        
        // Store the current hand position in the buffer
        Vector3 currentHandPosition = pointer.transform.position;
        //handPositionBuffer[bufferIndex] = currentHandPosition - endPointPosition;
        handPositionBuffer[bufferIndex] = currentHandPosition;
        bufferIndex = (bufferIndex + 1) % bufferSize;

        
        for(int i = 0; i < bufferSize; i++)
        {
            Debug.Log("Hand Position #" + i + ": " + handPositionBuffer[i]);
        }
        
        Debug.Log("Current Hand Position: " + currentHandPosition);

        // Calculate the amplitude based on the buffered hand positions
        float handAmplitude = CalculateHandAmplitude();
        
        // Remap the hand amplitude to the desired range [0.1f, 0.5f]
        const float minHandAmplitude = 0.1f; // Minimum hand amplitude to map to 0.1f
        const float maxHandAmplitude = 5.0f; // Maximum hand amplitude to map to 0.5f
        Debug.Log("Hand Amplitude is " + handAmplitude);
        Debug.Log("Value of Hand Amplutude between " + minHandAmplitude + " and " + maxHandAmplitude + " is " + Mathf.InverseLerp(minHandAmplitude, maxHandAmplitude, handAmplitude));
        float remappedAmplitude = Mathf.Lerp(0.1f, 0.5f, Mathf.InverseLerp(minHandAmplitude, maxHandAmplitude, handAmplitude));
        Debug.Log("Remapped Amplitude (between 0.1f and 0.5f) is " + remappedAmplitude);
        

        // Update the sinewave ray's amplitude based on the hand amplitude
        sinewaveRay.SetAmplitude(remappedAmplitude);

        // Update previous hand position and time for speed calculation
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);

        previousHandPosition = currentHandPosition;
        previousTime = currentTime;
    }
    
    private float CalculateHandAmplitude()
    {
        // Find the maximum and minimum positions in the buffer
        Vector3 maxPosition = Vector3.negativeInfinity;
        Vector3 minPosition = Vector3.positiveInfinity;
        foreach (Vector3 position in handPositionBuffer)
        {
            maxPosition = Vector3.Max(maxPosition, position);
            minPosition = Vector3.Min(minPosition, position);
        }

        // Calculate the amplitude of the hand movement
        float handAmplitude;
        if (sinewaveRay.IsHorizontal())
        {
            handAmplitude = Mathf.Abs(maxPosition.x - minPosition.x);
        }
        else
        {
            handAmplitude = Mathf.Abs(maxPosition.y - minPosition.y);
        }

        return handAmplitude;
    }#1#










    // Custom version first attempt
    
    /*private void UpdateSinewaveRay()
    {
        
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        Vector3 pointerPosition = pointer.transform.position;
        Vector3 currentHandPosition = pointerPosition - endPointPosition;
        
        Debug.Log("End Point Position: " + endPointPosition);
        Debug.Log("Pointer Position: " + pointerPosition);
        Debug.Log("Current Hand Position: " + currentHandPosition);
        Debug.Log("Previous Hand Position: " + previousHandPosition);
        
        
        float handYPosition;
        float handXPosition;
        float amplitude;
        if (sinewaveRay.IsHorizontal())
        { 
            handXPosition = pointerPosition.x;
            amplitude = Mathf.Abs(handXPosition - initialHandXPosition);
                            
        }
        else
        {
            
            handYPosition = pointerPosition.y;
            amplitude = Mathf.Abs(handYPosition - initialHandYPosition);

        }
        //sinewaveRay.SetAmplitude(amplitude);


        if (isInitial)
        {
            // Just hit the interactable; this is the first time we're tracking the hand position
            sinewaveRay.SetAmplitude(amplitude);

            /*if (currentHandPosition.y > 0)
            {
                isPositiveDirection = true;
            }
            else
            {
                isPositiveDirection = false;
            }#2#
            
            //if(Mathf.Abs(currentHandPosition.y) > Mathf.Abs(previousHandPosition.y))
            if(currentHandPosition.y > endPointPosition.y) // Maybe RHS 0
            {
                initialDirection = new Vector3(0,1,0);
                isFirstDirection = true;
                isFirstHalf = true;
                isInitial = false;
            }
            //else if(Mathf.Abs(currentHandPosition.y) == Mathf.Abs(previousHandPosition.y))
            else if(currentHandPosition.y == endPointPosition.y) // Maybe RHS 0
            {
                // Consider the case where the first vertical position is exactly zero (relative to the endPoint)
                isInitial = true;
            }
            else
            {
                initialDirection = new Vector3(0,-1,0);
                isFirstDirection = true;
                isFirstHalf = true;
                isInitial = false;
            }
            
        }
        else
        {
            // We're tracking the hand position

            if (isFirstDirection && isFirstHalf)
            {
                if(Mathf.Abs(currentHandPosition.y) >= Mathf.Abs(previousHandPosition.y))
                {
                    sinewaveRay.SetAmplitude(amplitude);
                    //Valutare se clampare già qua
                }
                else if(Mathf.Abs(currentHandPosition.y) < Mathf.Abs(previousHandPosition.y))
                {
                    savedMaxPosition = Vector3.Max(currentHandPosition, savedMaxPosition);
                    isFirstDirection = false;
                }
                
                previousHandPosition = currentHandPosition;
            }
            else if(!isFirstDirection && isFirstHalf)
            {
                
                
                if(Mathf.Abs(currentHandPosition.y) > Mathf.Abs(previousHandPosition.y))
                {
                    isFirstDirection = true;
                }
                else if(Mathf.Abs(currentHandPosition.y) <= Mathf.Abs(previousHandPosition.y))
                {
                    if (Mathf.Sign(currentHandPosition.y) == Mathf.Sign(previousHandPosition.y))
                    {
                        isFirstHalf = true;
                    }
                    else
                    {
                        isFirstHalf = false;
                    }
                }
                
                previousHandPosition = currentHandPosition;
            }
            
            else if(!isFirstDirection && !isFirstHalf)
            {
                if (Mathf.Abs(currentHandPosition.y) > Mathf.Abs(previousHandPosition.y))
                {
                    
                }
                else if (Mathf.Abs(currentHandPosition.y) <= Mathf.Abs(previousHandPosition.y))
                {
                    isFirstDirection = true;
                    savedMaxPosition = Vector3.Max(currentHandPosition, savedMaxPosition);
                    sinewaveRay.SetAmplitude(amplitude);
                }
                
                previousHandPosition = currentHandPosition;
            }
            else if(isFirstDirection && !isFirstHalf)
            {
                if(Mathf.Abs(currentHandPosition.y) >= Mathf.Abs(previousHandPosition.y))
                {
                    isFirstDirection = false;
                }
                else if(Mathf.Abs(currentHandPosition.y) < Mathf.Abs(previousHandPosition.y))
                {
                    
                    if (Mathf.Sign(currentHandPosition.y) == Mathf.Sign(previousHandPosition.y))
                    {
                        isFirstHalf = false;
                    }
                    else
                    {
                        isFirstHalf = true;
                    }
                    
                    previousHandPosition = currentHandPosition;
                }
            }
            
            
        }
        
        
        
        
        
        
        
        
        
        
        
        
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);
        //float frequency = speed / 2;
        //sinewaveRay.SetSpeed(speed);

        
        previousTime = currentTime;
        
        
        
    }#1#
    
    /*private float HandlePositionChange()
    {
        
    }#1#
    
    
    
    /// <summary>
    /// Takes the angle between the direction of the ray and the direction of the hand
    /// and maps it to the spiralwave ray's radius, updating it accordingly
    /// </summary>
    /// <remarks>
    /// Simple version with amplitude based on relative angles
    /// </remarks>
    private void UpdateSpiralwaveRayByAngle()
    {
        //TODO: Now the endpoint gets directed towards the core at the start. Check if fixes the small issue of missing perfect symmetry
        
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = spiralwaveRay.GetEndPoint().position;

        // Calculate the vectors from the coreCenter to the endPoint and the pointer
        Vector3 vectorToEndpoint = endPointPosition - coreCenter.position;
        Vector3 vectorToPointer = pointer.transform.position - coreCenter.position;

        // Calculate the angle between the two vectors        
        float angleDifference = Mathf.Abs(Vector3.Angle(vectorToEndpoint, vectorToPointer));

        // Map the angle difference to the radius range
        float radius = Mathf.Lerp(minRadius, maxRadius, angleDifference / 180f); // Dividing by 180 because the maximum difference between two angles is 180 degrees
        
        
        spiralwaveRay.SetRadius(radius);
        
        /*
        // Check if the hand is moving in a counterclockwise direction
        bool isMoving = Vector3.Distance(pointer.transform.position, previousPointerPosition) > 0;

        // Update the spiralwave ray's radius based on the calculated radius and whether the hand is moving
        if (isMoving && isTracking)
        {
            spiralwaveRay.SetRadius(radius);
        }
        else
        {
            spiralwaveRay.ResetRadius();
        }

        // Update the previous pointer position for the next frame
        previousPointerPosition = pointer.transform.position;
        #1#

    }

    #endregion
    
    
    
    
}*/
