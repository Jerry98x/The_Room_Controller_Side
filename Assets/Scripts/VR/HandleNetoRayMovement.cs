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

/// <summary>
/// Class that handles the movement of the Neto ray, the emissive intensity of the ray and the control interruption.
/// </summary>
public class HandleNetoRayMovement : MonoBehaviour
{
    
    public UnityEvent<SinewaveRay, SinewaveRay, float> onNetoRayDistanceChange;
    public event Action<bool> OnEmergencyStatusChanged;
    
    
    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;
    
    [SerializeField] [ColorUsage(true)] private Color initialActiveBaseColor;
    [SerializeField] [ColorUsage(true, true)] private Color initialActiveEmissiveColor;
    [SerializeField] [ColorUsage(true)] private Color emergencyBaseColor;
    [SerializeField] [ColorUsage(true, true)] private Color emergencyEmissiveColor;
    
    [SerializeField] private EmergencyAudioEffect emergencyAudioEffect;

    
    private List<ActionBasedController> xrControllers = new List<ActionBasedController>();
    private List<XRDirectInteractor> interactors = new List<XRDirectInteractor>();
    private List<Pointer> pointers = new List<Pointer>();
    
    private SinewaveRay inactiveSinewaveRay;
    private SinewaveRay activeSinewaveRay;
    [ColorUsage(true, true)] private Color initialInactiveBaseColor;
    [ColorUsage(true, true)] private Color initialInactiveEmissiveColor;
    private float netoMovementMultiplier;
    private bool isInControl = false;
    private bool isSupposedToResetEmissiveIntensity = false;
    private bool hasEmergency = false;
    
    
    
    private void Start()
    {
        //inputData = GetComponent<InputData>();
        
        inactiveSinewaveRay = transform.parent.GetComponentInChildren<SinewaveRay>();
        initialInactiveBaseColor = inactiveSinewaveRay.GetComponent<Renderer>().material.GetColor(Constants.BASE_COLOR_ID);
        initialInactiveEmissiveColor = inactiveSinewaveRay.GetComponent<Renderer>().material.GetColor(Constants.EMISSIVE_COLOR_ID);
        netoMovementMultiplier = inactiveSinewaveRay.GetEndPointObject().GetMaxEndpointMovementMultiplier();

        
        
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


    /// <summary>
    /// Method that is called when the Neto ray's collider is entered by a hand controller.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {

        XRDirectInteractor interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        Pointer pointer = other.gameObject.GetComponent<Pointer>();
        ActionBasedController xrController = interactor?.GetComponentInParent<ActionBasedController>();

        if (interactor != null && !interactors.Contains(interactor) && !hasEmergency)
        {
            interactors.Add(interactor);
            pointers.Add(pointer);
            xrControllers.Add(xrController);
            
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            isInControl = true;
            isSupposedToResetEmissiveIntensity = false;
        }
        
        
    }

    /// <summary>
    /// Method that is called when the Neto ray's collider is exited by a hand controller.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponent<XRDirectInteractor>();
        
        if (interactor != null && interactors.Contains(interactor))
        {
            
            int index = interactors.IndexOf(interactor);
            
            // Remove this interactor and corresponding elements from the lists
            interactors.RemoveAt(index);
            pointers.RemoveAt(index);
            xrControllers.RemoveAt(index);
            
            // If no controllers remain in the list, disable control
            if (interactors.Count == 0)
            {
                isSupposedToResetEmissiveIntensity = false;
                activeSinewaveRay = null;
                isInControl = false;
            }
            
        }
    }
    
    
    /// <summary>
    /// Method that is called when a hand controller stays inside the Neto ray's collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponent<XRDirectInteractor>();
        
        if (interactor != null && interactors.Contains(interactor))
        {
            // Check if the trigger is released
            
            float triggerValue = 0;
            // Loop through each controller and get the maximum trigger value
            foreach (var xrController in xrControllers)
            {
                float currentTriggerValue = xrController.activateActionValue.action.ReadValue<float>();
                triggerValue = Mathf.Max(triggerValue, currentTriggerValue);
            }
            
            if (triggerValue <= Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD && !hasEmergency)
            {
                isInControl = true;
            }
        }
    }



    /// <summary>
    /// Method that handles the movement of the ray's endpoint based on the movement of the hand controller.
    /// </summary>
    /// <param name="finalNewDistance"> New distance of the ray endpoint (not used) </param>
    private void HandleRayEndpointMovement(out float finalNewDistance)
    {
        
        // Calculate the direction from coreCenter to the ray's endpoint
        Vector3 direction = (rayEndPoint.position - coreCenter.position).normalized;

        
        Vector3 pointerMovement = Vector3.zero;
        float distance = 0;
        // Loop through each pointer and calculate the movement for the one that is moving the most
        foreach (Pointer pointer in pointers)
        {
            // Get the current pointer position and the previous frame pointer position
            Vector3 currentPointerPosition = pointer.transform.position;
            Vector3 previousPointerPosition = pointer.GetPreviousPosition();
            
            float newPointerDistance = Vector3.Distance(currentPointerPosition, previousPointerPosition);
            if(newPointerDistance > distance)
            {
                distance = newPointerDistance;
                
                // Calculate the pointer's movement vector
                pointerMovement = currentPointerPosition - previousPointerPosition;
            }
        }
        

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

        
        
        // Return if the distance between positions is under a threshold
        if(distance < Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD)
        {
            finalNewDistance = clampedDistance;
            return;
        }
        
        
        // Calculate the new clamped position of the endpoint
        Vector3 clampedEndPointPosition = coreCenter.position + direction * clampedDistance;
        
        // Update the ray's endpoint position
        rayEndPoint.position = clampedEndPointPosition;

        finalNewDistance = clampedDistance;
        
    }

    
    /// <summary>
    /// Updates the amplitude and frequency of the sinewave rays based on the new distance of the ray endpoint.
    /// </summary>
    /// <param name="inactiveSinewaveRayToChange"> Inactive ray </param>
    /// <param name="activeSinewaveRayToChange"> Active ray </param>
    /// <param name="finalNewDistance"> New distance of the ray endpoint </param>
    private void UpdateLineRenderers(SinewaveRay inactiveSinewaveRayToChange, SinewaveRay activeSinewaveRayToChange, float finalNewDistance)
    {
        
        // Update the linerenderers' amplitude and frequency, so that when the ray is shorter
        // the sinewave has a higher frequency and amplitude (like it's more compressed),
        // and when the ray is longer the sinewave has a lower frequency and amplitude (like it's more stretched)
        
        // Calculate the new amplitude and frequency and assign them to both sinewave rays
        float newAmplitude  = Constants.NETO_AMPLITUDE_DISTANCE_RATE / finalNewDistance;
        float newFrequency = Constants.NETO_FREQUENCY_DISTANCE_RATE / finalNewDistance;
        
        inactiveSinewaveRayToChange.SetAmplitude(newAmplitude);
        inactiveSinewaveRayToChange.SetFrequency(newFrequency);
        activeSinewaveRayToChange.SetAmplitude(newAmplitude);
        activeSinewaveRayToChange.SetFrequency(newFrequency);

    }
    
    
    /// <summary>
    /// Changes the emissive intensity of the ray based on the grip value of the controller. Handles corner cases.
    /// </summary>
    private void HandleEmissiveIntensityBasedOnTrigger()
    {

        if (isSupposedToResetEmissiveIntensity)
        {
            // Case in which I end up as soon as I press the lateral trigger for the first time after gaining
            // control of the Neto. All the following times I press the lateral trigger, I will stay here.
            // The boolean gets reset to false when I loose control of the Neto.
            
            float gripValue = 0;
            // Loop through each controller and get the maximum grip value
            foreach (var xrController in xrControllers)
            {
                float currentGripValue = xrController.selectActionValue.action.ReadValue<float>();
                gripValue = Mathf.Max(gripValue, currentGripValue);
            }
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                
                Renderer activeRayRenderer = activeSinewaveRay.GetComponent<Renderer>();
                if (activeRayRenderer != null)
                {
                    Color currentEmissiveColor = activeRayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissiveColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissiveColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(activeRayRenderer, activeRayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newEmissiveColor);
                }
                
                Renderer inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
                if (inactiveRayRenderer != null)
                {
                    Color currentInactiveEmissiveColor = inactiveRayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newInactiveEmissiveColor = GetHDRIntensity.AdjustEmissiveIntensity(currentInactiveEmissiveColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveRayRenderer, inactiveRayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newInactiveEmissiveColor);
                }

            }
            else
            {
                // Branch needed to reset the emissive intensity when I let the lateral trigger go (since it
                // could not detect the grip values smoothly)
                
                Renderer activeRayRenderer = activeSinewaveRay.GetComponent<Renderer>();
                if (activeRayRenderer != null)
                {
                    Color newEmissiveColor = initialActiveEmissiveColor;
                
                    SetMaterialColor(activeRayRenderer, activeRayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newEmissiveColor);
                
                }
                
                Renderer inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
                if (inactiveRayRenderer != null)
                {
                    Color newInactiveEmissiveColor = initialInactiveEmissiveColor;
                
                    SetMaterialColor(inactiveRayRenderer, inactiveRayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newInactiveEmissiveColor);
                
                }
                
                
            }
        }
        else
        {
            // Case in which I end up as soon as I gain control of the Neto. I will stay here until I press the
            // lateral trigger. I want a possible emissivity different from the initial one to be maintained when I lose
            // control and then I regain it, because the physical Neto need to illuminate the Room.
            // The ELSE branch of the inner IF statement placed in the IF branch of the outer if statement prevents
            // this from happening, but is needed to properly reset the emissive intensity when I let the lateral
            // trigger go (since it could not detect the grip values smoothly).
            
            float gripValue = 0;
            // Loop through each controller and get the maximum grip value
            foreach (var xrController in xrControllers)
            {
                float currentGripValue = xrController.selectActionValue.action.ReadValue<float>();
                gripValue = Mathf.Max(gripValue, currentGripValue);
            }
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                
                Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
                if (rayRenderer != null)
                {
                    Color currentEmissiveColor = rayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissiveColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissiveColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(rayRenderer, rayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newEmissiveColor);
                }
                
                
                Renderer inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
                if (inactiveRayRenderer != null)
                {
                    Color currentInactiveEmissiveColor = inactiveRayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newInactiveEmissiveColor = GetHDRIntensity.AdjustEmissiveIntensity(currentInactiveEmissiveColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveRayRenderer, inactiveRayRenderer.material.GetColor(Constants.BASE_COLOR_ID), newInactiveEmissiveColor);
                }

                // Set the boolean to true so that as soon as I press the lateral trigger, I end up in the other
                // branch of the if statement
                isSupposedToResetEmissiveIntensity = true;
            }
            
        }
        
        
    }


    /// <summary>
    /// Handles the interruption of the control of the Neto ray, done by pressing the trigger of the controller when in control.
    /// </summary>
    private void HandleControlInterruption()
    {
        float triggerValue = 0;
        // Loop through each controller and get the maximum grip value
        foreach (var xrController in xrControllers)
        {
            float currentTriggerValue = xrController.activateActionValue.action.ReadValue<float>();
            triggerValue = Mathf.Max(triggerValue, currentTriggerValue);
        }
        
        if (triggerValue > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD && !hasEmergency)
        {
            isInControl = false;
        }

    }

    
    /// <summary>
    /// Starts the emergency mode, in which the ray's color changes to red and the emergency audio is played.
    /// </summary>
    /// <remarks>
    /// NOT USED
    /// </remarks>
    public void StartEmergencyMode()
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

    /// <summary>
    /// Stops the emergency mode, in which the ray's color changes back to its initial color.
    /// </summary>
    /// <remarks>
    /// NOT USED
    /// </remarks>
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
        else
        {
            // Handle the specific case in which the emergency mode was started when in control of the ray and ended
            // when the hand controller had already exited the interactable
            inactiveSinewaveRay = transform.parent.GetComponentInChildren<SinewaveRay>();
            inactiveRayRenderer = inactiveSinewaveRay.GetComponent<Renderer>();
            SetMaterialColor(inactiveRayRenderer, initialInactiveBaseColor, initialInactiveEmissiveColor);
            inactiveSinewaveRay = null;
            
        }
        
        
        if (activeSinewaveRay != null)
        {
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            SetMaterialColor(rayRenderer, initialActiveBaseColor, initialActiveEmissiveColor);
        }
        else
        {
            // Handle the specific case in which the emergency mode was started when in control of the ray and ended
            // when the hand controller had already exited the interactable
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            SetMaterialColor(rayRenderer, initialActiveBaseColor, initialActiveEmissiveColor);
            activeSinewaveRay = null;
        }
        
    }
    
    
    private void SetMaterialColor(Renderer rayRenderer, Color baseColor, Color emissiveColor)
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
    
    public List<ActionBasedController> GetControllers()
    {
        return xrControllers;
    }
    
    
}
