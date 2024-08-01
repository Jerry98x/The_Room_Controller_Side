using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

public class HandleNetoRayMovement : MonoBehaviour
{

    
    public UnityEvent<SinewaveRay, SinewaveRay, float> onNetoRayDistanceChange;
    public event Action<bool> OnEmergencyStatusChanged;

    

    //private InputData inputData;
    
    
    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;
    
    [SerializeField] [ColorUsage(true)] private Color initialActiveBaseColor;
    [SerializeField] [ColorUsage(true, true)] private Color initialActiveEmissiveColor;
    [SerializeField] [ColorUsage(true)] private Color emergencyBaseColor;
    [SerializeField] [ColorUsage(true, true)] private Color emergencyEmissiveColor;
    
    [SerializeField] private EmergencyAudioEffect emergencyAudioEffect;

    
    private ActionBasedController xrController;
    private XRDirectInteractor interactor;
    private Pointer pointer;
    
    private SinewaveRay inactiveSinewaveRay;
    private SinewaveRay activeSinewaveRay;
    [ColorUsage(true)] private Color initialInactiveBaseColor;
    [ColorUsage(true, true)] private Color initialInactiveEmissiveColor;
    private float netoMovementMultiplier;
    private bool isInControl = false;
    private bool hasEmergency = false;
    
    
    
    private void Start()
    {
        //inputData = GetComponent<InputData>();
        
        inactiveSinewaveRay = transform.parent.GetComponentInChildren<SinewaveRay>();
        initialInactiveBaseColor = inactiveSinewaveRay.GetComponent<Renderer>().material.GetColor(Constants.BASE_COLOR_ID);
        initialInactiveEmissiveColor = inactiveSinewaveRay.GetComponent<Renderer>().material.GetColor(Constants.EMISSIVE_COLOR_ID);
        netoMovementMultiplier = inactiveSinewaveRay.GetEndPointObject().GetEndpointMovementMultiplier();

        
        
        // Listeners
        onNetoRayDistanceChange.AddListener(UpdateLineRenderers);
    }

    private void HandledEvents()
    {
        // If I press the E key, I start the emergency mode
        if (!hasEmergency && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Starting emergency mode");
            StartEmergencyMode();
        }
        if(hasEmergency && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Stopping emergency mode");
            StopEmergencyMode();
        }
    }


    private void Update()
    {
        HandledEvents();
        
        if(hasEmergency)
        {
            return;
        }
        
        if (isInControl)
        {
            
            // Handle ray's movement
            HandleRayEndpointMovement(out float finalNewDistance);
                    
            // Invoke the event that will notify the Neto module of the distance change
            onNetoRayDistanceChange?.Invoke(inactiveSinewaveRay, activeSinewaveRay, finalNewDistance);
                
            // Handle emissive intensity based on trigger press
            HandleEmissiveIntensityBasedOnTrigger();
            
            
            // Handle control interruption
            HandleControlInterruption();
            
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
        pointer = other.gameObject.GetComponent<Pointer>();
        xrController = interactor.GetComponentInParent<ActionBasedController>();
        
        if ((interactor != null || pointer != null) && !hasEmergency)
        {
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            isInControl = true;
        }
        
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() == interactor)
        {
            // Reset the active ray material to its initial color to avoid color artifacts in the
            // specific case where the emergency mode was started with the hand already inside the collider
            // and then ended while the hand was already outside the collider
            /*Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            if (rayRenderer != null && hasEmergency)
            {
                SetMaterialColor(rayRenderer, initialActiveBaseColor, initialActiveEmissiveColor);
            }*/
            
            activeSinewaveRay = null;

            pointer = null;
            interactor = null;
            isInControl = false;
        }
    }
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() == interactor)
        {
            // Check if the trigger is released
            float triggerValue = xrController.activateActionValue.action.ReadValue<float>();
            if (triggerValue <= Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD && !hasEmergency)
            {
                isInControl = true;
            }
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
        float clampedDistance = Mathf.Clamp(newDistance, rayEndPoint.GetComponent<RayEndPoint>().GetMinEndpointDistance(), rayEndPoint.GetComponent<RayEndPoint>().GetMaxEndpointDistance());

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
        
        //bool isGripping = xrController.selectAction.action.ReadValue<float>().Equals(1.0f);
        //Debug.Log("GRIPPING: " + isGripping);
        float gripValue = xrController.selectActionValue.action.ReadValue<float>();
        if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
        {
            Debug.Log("TRIGGER PRESSED! Grip value: " + gripValue);
            
            // Map the grip value to the capped emissive intensity range defined in the Constants class
            float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
            Debug.Log("CAPPED EMISSIVE INTENSITY: " + cappedEmissiveIntensity);
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            if (rayRenderer != null)
            {
                Color currentEmissiveColor = rayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
                //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                Color newEmissiveColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissiveColor, cappedEmissiveIntensity);
                
                SetMaterialColor(rayRenderer, rayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newEmissiveColor);
            }

        }
        else
        {
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            if (rayRenderer != null)
            {
                Color newEmissiveColor = initialActiveEmissiveColor;
                
                SetMaterialColor(rayRenderer, rayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newEmissiveColor);
                
            }
        }
    }


    private void HandleControlInterruption()
    {
        Debug.Log("XR CONTROLLER: " + xrController);

        Debug.Log("TRIGGER VALUE: " + xrController.activateActionValue.action.ReadValue<float>());
        float triggerValue = xrController.activateActionValue.action.ReadValue<float>();
        if (triggerValue > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD && !hasEmergency)
        {
            isInControl = false;
        }

    }

    
    
    private void StartEmergencyMode()
    {
        hasEmergency = true;
        isInControl = false;
        
        OnEmergencyStatusChanged?.Invoke(hasEmergency);

        
        // Change the color of the ray to red
    
        Renderer inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
        if (inactiveRayRenderer != null)
        {
            SetMaterialColor(inactiveRayRenderer, emergencyBaseColor, emergencyEmissiveColor);
        }
        if (activeSinewaveRay != null)
        {
            Renderer activeRayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            SetMaterialColor(activeRayRenderer, emergencyBaseColor, emergencyEmissiveColor);
        }
        
        emergencyAudioEffect.onEmergencyStart.Invoke();

        
    }

    public void StopEmergencyMode()
    {
        hasEmergency = false;
        
        OnEmergencyStatusChanged?.Invoke(hasEmergency);

        
        // Change the color of the ray back to its initial color
        Renderer inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
        if (inactiveRayRenderer != null)
        {
            SetMaterialColor(inactiveRayRenderer, initialInactiveBaseColor, initialInactiveEmissiveColor);
        }
        if (activeSinewaveRay != null)
        {
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            SetMaterialColor(rayRenderer, initialActiveBaseColor, initialActiveEmissiveColor);
        }
        
    }
    
    
    public void SetMaterialColor(Renderer rayRenderer, Color baseColor, Color emissiveColor)
    {
        rayRenderer.material.SetColor(Constants.BASE_COLOR_ID, baseColor);
        rayRenderer.material.SetColor(Constants.EMISSIVE_COLOR_ID, emissiveColor);
    }
    
    
    

    public bool IsInControl()
    {
        return isInControl;
    }
    
    public bool HasEmergency()
    {
        return hasEmergency;
    }
    
    
}
