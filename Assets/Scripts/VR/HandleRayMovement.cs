using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleRayMovement : MonoBehaviour
{
    //public XRController handController;
    private XRRayInteractor rayInteractor;
    private Pointer pointer;
    //private Vector3 hitPoint;
    
    SinewaveRay sinewaveRay;
    SpiralwaveRay spiralwaveRay;

    private float maxElongation = 2f;
    private float minAmplitude = 0.1f;
    private float maxAmplitude = 2.5f;
    
    
    private bool isTracking = false;
    private float initialHandYPosition;
    private float initialHandXPosition;
    private Vector3 previousHandPosition = Vector3.zero;
    private float previousTime;
    
    
    
    
    
    private const int bufferSize = 100; // Size of the circular buffer
    private Vector3[] handPositionBuffer = new Vector3[bufferSize]; // Circular buffer to store hand positions
    private int bufferIndex = 0; // Index to keep track of the current position in the buffer




    // Variables for custom function attempt

    private Vector3 savedMaxPosition = Vector3.zero;
    private bool isInitial = true;
    private bool isFirstDirection = false;
    private bool isFirstHalf = false;
    private bool isPositiveDirection = false;
    private Vector3 initialDirection = Vector3.zero;
    


    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
        
        pointer = rayInteractor.GetComponentInChildren<Pointer>();
    }
    
    
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
        
        
        initialHandYPosition = pointer.transform.position.y;
        initialHandXPosition = pointer.transform.position.x;
        isTracking = true;
    }
    
    public void HandleHoverExited(HoverExitEventArgs args)
    {
        isTracking = false;
        
        // Reset the amplitude of the ray
        SinewaveRay sine = args.interactable.GetComponent<SinewaveRay>();
        if (sine != null)
        {
            sinewaveRay = null;
        }
        SpiralwaveRay spiral = args.interactable.GetComponent<SpiralwaveRay>();
        if (spiral != null)
        {
            spiralwaveRay = null;
        }
    }
    
    
    
    
    
    
    private void Update()
    {
        if (isTracking)
        {
            if(sinewaveRay != null)
            {
                UpdateSinewaveRay();
            }
            if(spiralwaveRay != null)
            {
                UpdateSpiralwaveRay();
            }
        }
        else
        {
            isInitial = true;
        }
    }


    
    // Simple version
    
    private void UpdateSinewaveRay()
    {

        float handXPosition;
        float handYPosition;

        float relativeXPosition;
        float relativeYPosition;

        float amplitude;
        
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        
        if(sinewaveRay.IsHorizontal())
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
        }
        
        // Consider endpointposition + maxelongation and endpointposition - maxelongation as a lerp
        
        

        // Clamp the amplitude to a range you decide
        //amplitude = Mathf.Clamp(amplitude, minAmplitude, maxAmplitude);

        // Update the sinewave ray's amplitude based on the hand amplitude
        sinewaveRay.SetAmplitude(amplitude);
        
        
        
        /*float handYPosition = pointer.transform.position.y;
        float handXPosition = pointer.transform.position.x;

        // Calculate the distance between current hand position and initial position
        float distanceFromInitial = Vector2.Distance(new Vector2(handXPosition, handYPosition), new Vector2(initialHandXPosition, initialHandYPosition));

        // Map the distance to the amplitude using a periodic function
        float amplitude = Mathf.Sin(distanceFromInitial * Mathf.PI) * 0.5f; //0.5f is the maximum amplitude

        sinewaveRay.SetAmplitude(amplitude);

        // Update previous hand position and time for speed calculation
        Vector3 currentHandPosition = pointer.transform.position;
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);

        previousHandPosition = currentHandPosition;
        previousTime = currentTime;
        */
        
        
        
        
        
        /*float handYPosition;
        float handXPosition;
        float amplitude;
        if (sinewaveRay.IsHorizontal())
        { 
            handXPosition = pointer.transform.position.x;
            amplitude = Mathf.Abs(handXPosition - initialHandYPosition);
                            
        }
        else
        {
            
            handYPosition = pointer.transform.position.y;
            Debug.Log("Hand Y Position: " + handYPosition);
            amplitude = Mathf.Abs(handYPosition - initialHandYPosition);
            Debug.Log("Amplitude: " + amplitude);

        }
        sinewaveRay.SetAmplitude(amplitude);
        
        
        Vector3 currentHandPosition = pointer.transform.position;
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);
        //float frequency = speed / 2;
        //sinewaveRay.SetSpeed(speed);

        previousHandPosition = currentHandPosition;
        previousTime = currentTime;*/
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
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
    }*/










    // Custom version alla cazzo di cane
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
            }#1#
            
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
        
        
        
    }*/
    
    /*private float HandlePositionChange()
    {
        
    }*/
    
    
    
    
    private void UpdateSpiralwaveRay()
    {
       
    }
    
}
