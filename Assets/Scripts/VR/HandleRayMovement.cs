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
    
    SinewaveRay sinewaveRay;
    SpiralwaveRay spiralwaveRay;
    
    
    private bool isTracking = false;
    private float initialHandYPosition;
    private float initialHandXPosition;
    private Vector3 previousHandPosition;
    private float previousTime;
    
    
    
    
    
    private const int bufferSize = 100; // Size of the circular buffer
    private Vector3[] handPositionBuffer = new Vector3[bufferSize]; // Circular buffer to store hand positions
    private int bufferIndex = 0; // Index to keep track of the current position in the buffer

    
    
    
    
    
    


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
    }


    /*private void UpdateSinewaveRay()
    {
        
        float handYPosition = pointer.transform.position.y;
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
        previousTime = currentTime;#1#
    }*/
    
    
    private void UpdateSinewaveRay()
    {
        // Get the endPoint position of the hovered ray
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        
        // Store the current hand position in the buffer
        Vector3 currentHandPosition = pointer.transform.position;
        handPositionBuffer[bufferIndex] = currentHandPosition - endPointPosition;
        bufferIndex = (bufferIndex + 1) % bufferSize;
        
        Debug.Log("Current Hand Position: " + currentHandPosition);

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
        
        // Remap the hand amplitude to the desired range [0.1f, 0.5f]
        const float minHandAmplitude = 0.1f; // Minimum hand amplitude to map to 0.1f
        const float maxHandAmplitude = 5.0f; // Maximum hand amplitude to map to 0.5f
        float remappedAmplitude = Mathf.Lerp(0.1f, 0.5f, Mathf.InverseLerp(minHandAmplitude, maxHandAmplitude, handAmplitude));


        // Update the sinewave ray's amplitude based on the hand amplitude
        sinewaveRay.SetAmplitude(remappedAmplitude);

        // Update previous hand position and time for speed calculation
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);

        previousHandPosition = currentHandPosition;
        previousTime = currentTime;
    }
    
    
    private void UpdateSpiralwaveRay()
    {
       
    }
    
}
