using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleRayMovement : MonoBehaviour
{
    public XRController handController;
    private XRRayInteractor rayInteractor;
    
    SinewaveRay sinewaveRay;
    SpiralwaveRay spiralwaveRay;
    
    
    private bool isTracking = false;
    private float initialHandYPosition;
    private Vector3 previousHandPosition;
    private float previousTime;


    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
    }
    
    
    private void HandleHoverEntered(HoverEnterEventArgs args)
    {
        
        // Set the amplitude of the ray
        SinewaveRay sine = args.interactable.GetComponent<SinewaveRay>();
        if (sine != null)
        {
            sinewaveRay = sine;
        }
        SpiralwaveRay spiral = args.interactable.GetComponent<SpiralwaveRay>();
        if (spiral != null)
        {
            spiralwaveRay = spiral;
        }
        
        
        initialHandYPosition = handController.transform.position.y;
        isTracking = true;
    }
    
    private void HandleHoverExited(HoverExitEventArgs args)
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


    private void UpdateSinewaveRay()
    {
        float handYPosition;
        float amplitude;
        if (sinewaveRay.IsHorizontal())
        { 
            handYPosition = handController.transform.position.y;
            amplitude = handYPosition - initialHandYPosition;
                            
        }
        else
        {
            handYPosition = handController.transform.position.y;
            amplitude = handYPosition - initialHandYPosition;

        }
        sinewaveRay.SetAmplitude(amplitude);
        
        
        Vector3 currentHandPosition = handController.transform.position;
        float currentTime = Time.time;
        float speed = (currentHandPosition - previousHandPosition).magnitude / (currentTime - previousTime);
        //float frequency = speed / 2;
        sinewaveRay.SetSpeed(speed);

        previousHandPosition = currentHandPosition;
        previousTime = currentTime;
    }
    
    
    private void UpdateSpiralwaveRay()
    {
       
    }
    
}
