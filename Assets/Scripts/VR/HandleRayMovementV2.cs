using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// NOT USED

/*public class HandleRayMovementV2 : MonoBehaviour
{
    
    private class RayControlInfo
    {
        public Transform RayEndPoint;
        public float NetoMovementMultiplier;
        public XRController Controller;
        public SinewaveRay ActiveSinewaveRay;
        public SinewaveRay InactiveSinewaveRay;
        public SpiralwaveRay ActiveSpiralwaveRay;
        public SpiralwaveRay InactiveSpiralwaveRay;
    }
    
    
    private Dictionary<RayColliderHelper, RayControlInfo> rayControlInfos = new Dictionary<RayColliderHelper, RayControlInfo>();

    
    
    
    
    
    
    public UnityEvent<SinewaveRay, SinewaveRay, float> onNetoRayDistanceChange;
    
    [SerializeField] private Transform coreCenter;
    
    private Pointer pointer;
    private RayColliderHelper currentRayColliderHelper;
    
    private Dictionary<int, RayColliderHelper> activeRayColliderHelpers = new Dictionary<int, RayColliderHelper>();
    
    
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
        //xrController = GetComponent<XRController>();
        pointer = GetComponentInChildren<Pointer>();
        
    }

    private void Start()
    {
        // Listeners
        onNetoRayDistanceChange.AddListener(UpdateLineRenderers);
        
    }
    
    
    private void Update()
    {
        
        
        foreach (var rayControlInfo in rayControlInfos.Values)
        {
            // Check if the controller associated with this ray is in control
            // and then handle the ray's movement based on the controller's input
            if (IsControllerInControl(rayControlInfo.Controller))
            {
                HandleRayEndpointMovement(rayControlInfo, out float finalNewDistance);
                onNetoRayDistanceChange?.Invoke(rayControlInfo.InactiveSinewaveRay, rayControlInfo.ActiveSinewaveRay, finalNewDistance);
                HandleEmissiveIntensityBasedOnTrigger(rayControlInfo.Controller, rayControlInfo.ActiveSinewaveRay);

            }
        }
        
        
        
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
    
    
    
    
    // Example method to check if a controller is in control
    /*private bool IsControllerInControl(XRController controller)
    {
        // Example logic to determine if the controller is in control
        // This should be replaced with your actual logic to check controller input
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed)
        {
            return true;
        }
        return false;
    }#1#
    
    
    
    // Adjusted method to handle ray endpoint movement based on the controller
    private void HandleRayEndpointMovement(RayControlInfo rayControlInfo, out float finalNewDistance)
    {
        Vector3 direction = (rayControlInfo.RayEndPoint.position - coreCenter.position).normalized;
        Vector3 currentPointerPosition = pointer.transform.position;
        Vector3 previousPointerPosition = pointer.GetPreviousPosition();
        Vector3 pointerMovement = currentPointerPosition - previousPointerPosition;
        Vector3 projectedMovement = Vector3.Project(pointerMovement, direction);
        Vector3 scaledMovement = projectedMovement * rayControlInfo.NetoMovementMultiplier;
        Vector3 newEndPointPosition = rayControlInfo.RayEndPoint.position + scaledMovement;
        float newDistance = Vector3.Distance(coreCenter.position, newEndPointPosition);
        float clampedDistance = Mathf.Clamp(newDistance, rayControlInfo.RayEndPoint.GetComponent<EndPoint>().GetMinEndpointDistance(), rayControlInfo.RayEndPoint.GetComponent<EndPoint>().GetMaxEndpointDistance());
        Vector3 clampedEndPointPosition = coreCenter.position + direction * clampedDistance;
        rayControlInfo.RayEndPoint.position = clampedEndPointPosition;
        finalNewDistance = clampedDistance;
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
    
    
    
    
    
    
    
    
    
    
    
    
    
    

    // You'll need to adjust your OnPointerEnter and OnPointerExit methods to manage the rayControlInfos dictionary
    // This includes adding and removing entries based on the rays being interacted with
    
    
    
    
    
    
    /*public void OnPointerEnter(RayColliderHelper rayColliderHelper)
    {
        var controller = // Determine the controller interacting with the ray
            var rayControlInfo = new RayControlInfo
        {
            Controller = controller,
            RayEndPoint = // Determine the ray's endpoint,
                InactiveSinewaveRay = // Get the InactiveSinewaveRay component,
                    ActiveSinewaveRay = // Get the ActiveSinewaveRay component,
                        InactiveSpiralwaveRay = // Get the InactiveSpiralwaveRay component if applicable,
                            ActiveSpiralwaveRay = // Get the ActiveSpiralwaveRay component if applicable
                                NetoMovementMultiplier = // Set the netoMovementMultiplier
        };

        rayControlInfos[rayColliderHelper] = rayControlInfo;
    
    }#1#
    
    
    public void OnPointerEnter(RayColliderHelper rayColliderHelper)
    {
        Debug.Log("ENTER COLLIDER");

        // Determine the controller interacting with the ray
        // For simplicity, this example assumes there are two controllers, left and right
        // Adjust the logic to fit your actual controller identification method
        XRController controller = GetInteractingController(rayColliderHelper);

        // Get the corresponding ray components
        SinewaveRay inactiveSinewaveRay = rayColliderHelper.transform.parent.GetComponentInChildren<SinewaveRay>();
        SpiralwaveRay inactiveSpiralwaveRay = rayColliderHelper.transform.parent.GetComponentInChildren<SpiralwaveRay>();

        // If the ray components are found, proceed to get their active counterparts
        SinewaveRay activeSinewaveRay = null;
        if (inactiveSinewaveRay != null)
        {
            activeSinewaveRay = inactiveSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
        }

        SpiralwaveRay activeSpiralwaveRay = null;
        if (inactiveSpiralwaveRay != null)
        {
            activeSpiralwaveRay = inactiveSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
        }

        // Determine the ray's endpoint and neto movement multiplier
        Transform rayEndPoint = inactiveSinewaveRay != null ? inactiveSinewaveRay.GetEndPoint() : inactiveSpiralwaveRay.GetEndPoint();
        float netoMovementMultiplier = inactiveSinewaveRay != null ? inactiveSinewaveRay.GetEndPointObject().GetEndpointZMovementMultiplier() : inactiveSpiralwaveRay.GetEndPointObject().GetEndpointZMovementMultiplier();

        // Create and store RayControlInfo
        var rayControlInfo = new RayControlInfo
        {
            Controller = controller,
            RayEndPoint = rayEndPoint,
            InactiveSinewaveRay = inactiveSinewaveRay,
            ActiveSinewaveRay = activeSinewaveRay,
            InactiveSpiralwaveRay = inactiveSpiralwaveRay,
            ActiveSpiralwaveRay = activeSpiralwaveRay,
            NetoMovementMultiplier = netoMovementMultiplier
        };

        // Add or update the rayControlInfo in the dictionary
        rayControlInfos[rayColliderHelper] = rayControlInfo;

        // Additional logic for setting up the interaction
        rayColliderHelper.onPointerEnter.AddListener(OnPointerEnter);
        rayColliderHelper.onPointerExit.AddListener(OnPointerExit);

        Debug.Log("Ray endpoint: " + rayEndPoint);
    }
    
    
    
    
    
    /*public void OnPointerEnter(RayColliderHelper rayColliderHelper)
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

    }#1#

    /*public void OnPointerExit(RayColliderHelper rayColliderHelper)
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
    }#1#
    
    
    public void OnPointerExit(RayColliderHelper rayColliderHelper)
    {
        if (rayControlInfos.ContainsKey(rayColliderHelper))
        {
            rayControlInfos.Remove(rayColliderHelper);
        }
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
    
    
    private void HandleEmissiveIntensityBasedOnTrigger(XRController controller, SinewaveRay activeSinewaveRay)
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, 0f, 1.0f, Constants.CAPPED_MIN_EMISSION_INTENSITY, Constants.CAPPED_MAX_EMISSION_INTENSITY);
            Renderer rayRenderer = activeSinewaveRay.GetComponent<Renderer>();
            if (rayRenderer != null)
            {
                rayRenderer.material.SetFloat(Constants.EMISSION_INTENSITY_ID, cappedEmissiveIntensity);
            }
        }
    }
    
    
    
    
    private XRController GetInteractingController(RayColliderHelper rayColliderHelper)
    {
        // Placeholder method to determine which controller is interacting with the ray
        // You should replace this with your actual logic to identify the correct controller
        // For simplicity, we assume there are two controllers: left and right

        // Example of getting the left and right controllers from the scene
        XRController leftController = GetComponent<XRController>();
        XRController rightController = GetComponent<XRController>();
        
        Debug.Log("Gameobject is: " + gameObject);
        Debug.Log("LEFT CONTROLLER: " + leftController);
        Debug.Log("RIGHT CONTROLLER: " + rightController);
        
        Debug.Log("LEFT CONTROLLER POSITION: " + leftController.transform.position);
        Debug.Log("RIGHT CONTROLLER POSITION: " + rightController.transform.position);
        
        Debug.Log("RAY COLLIDER HELPER: " + rayColliderHelper);
        Debug.Log("RAY COLLIDER HELPER POSITION: " + rayColliderHelper.transform.position);

        // Logic to determine which controller is interacting
        // This is a simplified example, adjust as needed
        if (Vector3.Distance(leftController.transform.position, rayColliderHelper.transform.position) < Vector3.Distance(rightController.transform.position, rayColliderHelper.transform.position))
        {
            return leftController;
        }
        else
        {
            return rightController;
        }
    }
    
    
    
    


    public bool IsInControl()
    {
        return isInControl;
    }
    
    private bool IsControllerInControl(XRController controller)
    {
        return controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed;
    }
    
}*/
