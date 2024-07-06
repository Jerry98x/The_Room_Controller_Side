using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleRayMovementV2 : MonoBehaviour
{
    
    public UnityEvent<SinewaveRay, SinewaveRay, float> onNetoRayDistanceChange;
    
    [SerializeField] private Transform coreCenter;
    
    private Pointer pointer;
    private RayColliderHelper currentRayColliderHelper;

    
    private SinewaveRay inactiveSinewaveRay;
    private SinewaveRay activeSinewaveRay;
    private SpiralwaveRay inactiveSpiralwaveRay;
    private SpiralwaveRay activeSpiralwaveRay;
    private Transform rayEndPoint;
    //private Transform activeRayEndPoint;
    
    
    private float netoMovementMultiplier;
    
    private bool isInControl = false;
    
    
    private XRController xrController;


    private void Awake()
    {
        xrController = GetComponent<XRController>();
        pointer = GetComponentInChildren<Pointer>();
        
    }

    private void Start()
    {
        // Listeners
        onNetoRayDistanceChange.AddListener(UpdateLineRenderers);
        
    }
    
    
    private void Update()
    {
        if (currentRayColliderHelper != null)
        {
            // The pointer is currently inside the collider at the base of the ray
            // I can decide to take control of the ray's movement

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
            
    }
    
    
    
    public void OnPointerEnter(RayColliderHelper rayColliderHelper)
    {
        Debug.Log("ENTER COLLIDER");
        
        
        // TODO: Understand if there is a better way to handle events. currentRayColliderHelper can
        // TODO: dynamically change!
        
        // The pointer has entered a ray's collider
        currentRayColliderHelper = rayColliderHelper;
        currentRayColliderHelper.onPointerEnter.AddListener(OnPointerEnter);
        currentRayColliderHelper.onPointerExit.AddListener(OnPointerExit);
        //currentRayColliderHelper.onPointerStay.AddListener(OnPointerStay);
        
        currentRayColliderHelper.onPointerEnterLineRendererActivation.AddListener(OnPointerEnterLineRendererActivation);
        currentRayColliderHelper.onPointerExitLineRendererDeactivation.AddListener(OnPointerExitLineRendererDeactivation);
        
        
        // Get corresponding ray linerenderer and ray endpoint
        if (currentRayColliderHelper.transform.parent.GetComponentInChildren<SinewaveRay>())
        {
            inactiveSinewaveRay = currentRayColliderHelper.transform.parent.GetComponentInChildren<SinewaveRay>();
            rayEndPoint = inactiveSinewaveRay.GetEndPoint();
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            //activeRayEndPoint = activeSinewaveRay.GetEndPoint();
            netoMovementMultiplier = inactiveSinewaveRay.GetEndPointObject().GetEndpointZMovementMultiplier();
        }
        if (currentRayColliderHelper.transform.parent.GetComponentInChildren<SpiralwaveRay>())
        {
            inactiveSpiralwaveRay = currentRayColliderHelper.transform.parent.GetComponentInChildren<SpiralwaveRay>();
            rayEndPoint = inactiveSpiralwaveRay.GetEndPoint();
            activeSpiralwaveRay = inactiveSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            //activeRayEndPoint = activeSpiralwaveRay.GetEndPoint();
            
        }
        
        Debug.Log("Ray endpoint: " + rayEndPoint);

        isInControl = true;

    }

    public void OnPointerExit(RayColliderHelper rayColliderHelper)
    {
        Debug.Log("EXIT COLLIDER");
        
        // The pointer has exited a ray's collider
        if (currentRayColliderHelper == rayColliderHelper)
        {
            currentRayColliderHelper.onPointerEnter.RemoveListener(OnPointerEnter);
            currentRayColliderHelper.onPointerExit.RemoveListener(OnPointerExit);
            currentRayColliderHelper = null;
        }
        
        // Free ray linerenderer and ray endpoint
        inactiveSinewaveRay = null;
        activeSinewaveRay = null;
        inactiveSpiralwaveRay = null;
        activeSpiralwaveRay = null;
        rayEndPoint = null;
        //activeRayEndPoint = null;


        isInControl = false;
    }


    public void HandleRayEndpointMovement(out float finalNewDistance)
    {

        Debug.Log("IN CONTROL");
        // Calculate the direction from coreCenter to the ray's endpoint
        Vector3 direction = (rayEndPoint.position - coreCenter.position).normalized;

        // Get the current pointer position and the previous frame pointer position
        Vector3 currentPointerPosition = pointer.transform.position;
        Vector3 previousPointerPosition = pointer.GetPreviousPosition();
        Debug.Log("Current pointer position: " + currentPointerPosition);
        Debug.Log("Previous pointer position: " + previousPointerPosition);

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


    public void OnPointerEnterLineRendererActivation(LineRenderer parentLineRenderer, LineRenderer childLineRenderer)
    {
        childLineRenderer.gameObject.SetActive(true);
        parentLineRenderer.widthMultiplier = 0f;
    }

    
    public void OnPointerExitLineRendererDeactivation(LineRenderer parentLineRenderer, LineRenderer childLineRenderer)
    {
        parentLineRenderer.widthMultiplier = Constants.PARENT_LINERENDERER_WIDTH_MULTIPLIER;
        childLineRenderer.gameObject.SetActive(false);
    }
    
    
    
    private void UpdateLineRenderers(SinewaveRay inactiveSinewaveRayToChange, SinewaveRay activeSinewaveRayToChange, float finalNewDistance)
    {
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
        Debug.Log("XR CONTROLLER INPUT DEVICE: " + xrController.inputDevice);;
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
        }
    }


    public bool IsInControl()
    {
        return isInControl;
    }
    
}
