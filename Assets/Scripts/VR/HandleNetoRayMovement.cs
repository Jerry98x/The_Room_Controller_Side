using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

public class HandleNetoRayMovement : MonoBehaviour
{

    
    public UnityEvent<SinewaveRay, SinewaveRay, float> onNetoRayDistanceChange;
    
    public InputAction gripEmissionAction;
    
    
    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;
    
    
    private ActionBasedController xrController;
    private XRDirectInteractor interactor;
    private Pointer pointer;
    
    private SinewaveRay inactiveSinewaveRay;
    private SinewaveRay activeSinewaveRay;
    private float netoMovementMultiplier;
    private bool isInControl = false;
    
    
    
    private void Start()
    {
        // Listeners
        onNetoRayDistanceChange.AddListener(UpdateLineRenderers);
    }


    private void Update()
    {
        if (isInControl)
        {
            
            // Handle ray's movement
            HandleRayEndpointMovement(out float finalNewDistance);
                    
            // Invoke the event that will notify the Neto module of the distance change
            onNetoRayDistanceChange?.Invoke(inactiveSinewaveRay, activeSinewaveRay, finalNewDistance);
                
            // Handle emissive intensity based on trigger press
            HandleEmissiveIntensityBasedOnTrigger();
        }
    }


    void OnTriggerEnter(Collider other)
    {

        if(interactor != null || pointer != null)
        {
            Debug.Log("INTERACTOR NOT NULL: " + interactor);
            return;
        }

        interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        Debug.Log("INTERACTOR: " + interactor);
        pointer = other.gameObject.GetComponent<Pointer>();
        Debug.Log("POINTER: " + pointer);
        xrController = interactor.GetComponentInParent<ActionBasedController>();
        Debug.Log("XR CONTROLLER: " + xrController);
        Debug.Log("XR CONTROLLER NAME: " + xrController.gameObject.name);
        
        if (interactor != null || pointer != null)
        {
            
            inactiveSinewaveRay = transform.parent.GetComponentInChildren<SinewaveRay>();
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            netoMovementMultiplier = inactiveSinewaveRay.GetEndPointObject().GetEndpointZMovementMultiplier();
            
            isInControl = true;
        }
        
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() == interactor)
        {
            
            inactiveSinewaveRay = null;
            activeSinewaveRay = null;

            pointer = null;
            interactor = null;
            isInControl = false;
        }
    }



    public void HandleRayEndpointMovement(out float finalNewDistance)
    {
        
        // Calculate the direction from coreCenter to the ray's endpoint
        Vector3 direction = (rayEndPoint.position - coreCenter.position).normalized;

        // Get the current pointer position and the previous frame pointer position
        Vector3 currentPointerPosition = pointer.transform.position;
        Vector3 previousPointerPosition = pointer.GetPreviousPosition();

        // Calculate the pointer's movement vector
        Vector3 pointerMovement = currentPointerPosition - previousPointerPosition;

        // Project the pointer's movement onto the direction vector
        Vector3 projectedMovement = Vector3.Project(pointerMovement, direction);

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = projectedMovement * netoMovementMultiplier;

        // Calculate new potential position of the ray's endpoint
        Vector3 newEndPointPosition = rayEndPoint.position + scaledMovement;
        
        // Calculate the distance from coreCenter to the new endpoint position
        float newDistance = Vector3.Distance(coreCenter.position, newEndPointPosition);

        // Clamp the new distance within the allowed range
        float clampedDistance = Mathf.Clamp(newDistance, rayEndPoint.GetComponent<EndPoint>().GetMinEndpointDistance(), rayEndPoint.GetComponent<EndPoint>().GetMaxEndpointDistance());

        // Calculate the new clamped position of the endpoint
        Vector3 clampedEndPointPosition = coreCenter.position + direction * clampedDistance;
        
        // Update the ray's endpoint position
        rayEndPoint.position = clampedEndPointPosition;

        finalNewDistance = clampedDistance;
        
    }

    
    
    private void UpdateLineRenderers(SinewaveRay inactiveSinewaveRayToChange, SinewaveRay activeSinewaveRayToChange, float finalNewDistance)
    {
        
        Debug.Log("UPDATING LINE RENDERERS");
        Debug.Log("Inactive sinewave ray: " + inactiveSinewaveRayToChange);
        Debug.Log("Active sinewave ray: " + activeSinewaveRayToChange);
        Debug.Log("Final new distance: " + finalNewDistance);
        
        // Update the linerenderers amplitude and frequency, so that when the ray is shorter
        // the sinewave has a higher frequency and amplite (like it's more compressed),
        // and when the ray is longer the sinewave has a lower frequency and amplitude (like it's more stretched)
        
        // Calculate the new amplitude and frequency and assign them to both sinewave rays
        float newAmplitude  = Constants.NETO_AMPLITUDE_DISTANCE_RATE / finalNewDistance;
        float newFrequency = Constants.NETO_FREQUENCY_DISTANCE_RATE / finalNewDistance;
        
        inactiveSinewaveRayToChange.SetAmplitude(newAmplitude);
        inactiveSinewaveRayToChange.SetFrequency(newFrequency);
        activeSinewaveRayToChange.SetAmplitude(newAmplitude);
        activeSinewaveRayToChange.SetFrequency(newFrequency);

    }
    
    
    
    private void HandleEmissiveIntensityBasedOnTrigger()
    {
        Debug.Log("XR CONTROLLER: " + xrController);
        
        /*InputDevice device = xrController.inputDevice;
        
        if (xrController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            
            // Map the grip value to the capped emissive intensity range defined in the Constants class
            float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, 0f, 1.0f,
                Constants.CAPPED_MIN_EMISSION_INTENSITY, Constants.CAPPED_MAX_EMISSION_INTENSITY);
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            if (rayRenderer != null)
            {
                rayRenderer.material.SetFloat(Constants.EMISSION_INTENSITY_ID, cappedEmissiveIntensity);
            }
            
            
            // Adjust emissive intensity based on the grip value (0.0 to 1.0)
            //float emissiveIntensity = Mathf.Lerp(0f, 1.0f, gripValue);
            
            // Assuming the rayMaterial has an emissive color property
            //rayMaterial.SetColor("_EmissiveColor", rayMaterial.GetColor("_EmissiveColor") * emissiveIntensity);
        }*/
    }


    public bool IsInControl()
    {
        return isInControl;
    }
    
    
    
}
