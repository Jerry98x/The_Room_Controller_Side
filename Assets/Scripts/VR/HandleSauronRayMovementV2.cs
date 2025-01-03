using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniColliderInterpolator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class that handles the movement of the Sauron ray's endpoint, which is controlled by the user's hand controller,
/// and the control interruption.
/// </summary>
public class HandleSauronRayMovementV2 : MonoBehaviour
{
    
    public UnityEvent<SpiralwaveRay, SpiralwaveRay, float> onSauronRayDistanceChange;
    
    
    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;

    //[SerializeField] private Collider portalMeshCollider;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject portalConeSurface;
    [SerializeField] private GameObject portalConeBase;
    [SerializeField] private GameObject portalSimplified;
    [SerializeField] private Material[] coneSurfaceMaterials;
    
    
    
    // Cone properties
    
    [SerializeField] private Transform coneBaseCenter;
    [SerializeField] private Transform coneTip;
    [SerializeField] private Transform coneBaseExtremity;
    
    private Vector3 coneHorizontalAxis; // If Z is the vertical axis, then the horizontal axis can either be X or Y
    private Vector3 coneVerticalAxis;
    
    private float alphaRotationAngle; // The angle of the rotation around the vertical axis
    private float betaElevationAngle; // The angle of the elevation from the horizontal plane

    // Since beta is not computed thanks to a precise mathematical rule but it's approximated (the angle isn't actually
    // the same as the physical Sauron), use a scale factor that needs to be fine-tuned
    private float scale = 0.35f;
    
    
    private List<ActionBasedController> xrControllers = new List<ActionBasedController>();
    private List<XRDirectInteractor> interactors = new List<XRDirectInteractor>();
    private List<Pointer> pointers = new List<Pointer>();
    
    
    
    // Colliders properties
    
    //private MeshCollider portalMeshCollider;
    private BoxCollider[] portalBoxColliders;
    private MeshCollider[] portalSupportColliders;
    private Collider[] portalColliders;
    
    private SphereCollider sphereCollider;
    private Rigidbody rayEndPointRb;
    
    private Vector3 previousPosition;
    
    private MeshRenderer coneSurfaceRenderer;
    
    
    
    // Movement properties
    
    private Vector3 targetPosition;
    private Vector3 smoothedMovement;
    private float lerpSpeed = 30f;
    
    
    private float smoothTime = 0.01f;
    private Vector3 velocity = Vector3.zero;
    
    
    private bool overlapped = false;
    
    private SpiralwaveRay inactiveSpiralwaveRay;
    private SpiralwaveRay activeSpiralwaveRay;
    private float sauronMaxMovementMultiplier;
    private float sauronMinMovementMultiplier;
    private bool isInControl = false;
    
    
    private bool insideAnyCollider = false;
    
    private float colliderHapticFeedbackAmplitude = 0.25f;
    private float colliderHapticFeedbackDuration = 0.2f;
    
    
    private void Start()
    {
        rayEndPointRb = rayEndPoint.GetComponent<Rigidbody>();
        if (rayEndPointRb == null)
        {
            Debug.LogError("rayEndPoint does not have a Rigidbody component.");
        }
        else
        {
            rayEndPointRb.useGravity = false;
            rayEndPointRb.interpolation = RigidbodyInterpolation.Interpolate; // Enable interpolation
        }
        
        
        portalBoxColliders = portal.GetComponents<BoxCollider>();
        portalSupportColliders = portal.GetComponents<MeshCollider>();
        
        
        sphereCollider = rayEndPoint.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("rayEndPoint does not have a SphereCollider component.");
        }
        
        previousPosition = rayEndPoint.position;
        targetPosition = rayEndPoint.position;
        
        
        coneSurfaceRenderer = portalConeSurface.GetComponent<MeshRenderer>();
        AddMaterialToConeSurfaceRenderer(1);
        
        // Listeners
        onSauronRayDistanceChange.AddListener(UpdateLineRenderers);
    }


    /// <summary>
    /// FixedUpdate instead of Update to handle physics-related operations.
    /// </summary>
    private void FixedUpdate()
    {
        if (isInControl)
        {
            
            // Handle ray's movement
            
            HandleRayEndpointMovement(out float finalNewDistance);
            
                    
            // Invoke the event that will notify the Sauron module of the distance change
            onSauronRayDistanceChange?.Invoke(inactiveSpiralwaveRay, activeSpiralwaveRay, finalNewDistance);
            
            
            // Handle control interruption
            HandleControlInterruption();
            
        }
        else
        {
            if (coneSurfaceRenderer.materials.Length > 1)
            {
                AddMaterialToConeSurfaceRenderer(1);
            }
            portalConeBase.SetActive(false);
        }
        
        
    }
    

    /// <summary>
    /// Method that is called when the Sauron ray's collider is entered by a hand controller.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        XRDirectInteractor interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        Pointer pointer = other.gameObject.GetComponent<Pointer>();
        ActionBasedController xrController = interactor?.GetComponentInParent<ActionBasedController>();
        
        if(interactor != null && !interactors.Contains(interactor))
        {
            interactors.Add(interactor);
            xrControllers.Add(xrController);
            pointers.Add(pointer);
            
            
            inactiveSpiralwaveRay = transform.parent.GetComponentInChildren<SpiralwaveRay>();
            activeSpiralwaveRay = inactiveSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            sauronMaxMovementMultiplier = inactiveSpiralwaveRay.GetEndPointObject().GetMaxEndpointMovementMultiplier();
            sauronMinMovementMultiplier = inactiveSpiralwaveRay.GetEndPointObject().GetMinEndpointMovementMultiplier();
            
            isInControl = true;
        }
        
    }

    
    /// <summary>
    /// Method that is called when the Sauron ray's collider is exited by a hand controller.
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
                inactiveSpiralwaveRay = null;
                activeSpiralwaveRay = null;
                isInControl = false;
            }
            
        }
    }
    
    
    /// <summary>
    /// Method that is called when a hand controller stays inside the Sauron ray's collider.
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
            
            if (triggerValue <= Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD)
            {
                isInControl = true;
            }
        }
    }
    
    
    
    /// <summary>
    /// Method that handles the movement of the ray's endpoint based on the movement of the hand controller.
    /// It employs collision detection to prevent the endpoint from moving outside a cage of colliders.
    /// </summary>
    /// <param name="finalNewDistance"> New distance of the ray endpoint (not used) </param>
    private void HandleRayEndpointMovement(out float finalNewDistance)
    {
        int index = 0;
        
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
                
                index = pointers.IndexOf(pointer);
            }
        }
        
        // Return if the distance between positions is under a threshold
        if(distance < Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD)
        {
            finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);
            return;
        }


        float movementMultiplier = ComputeMovementMultiplier(distance);

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = pointerMovement * movementMultiplier;
        
        // Smooth the movement vector
        smoothedMovement = Vector3.Lerp(smoothedMovement, scaledMovement, smoothTime);

        // Calculate new potential position of the ray's endpoint
        Vector3 newEndPointPosition = rayEndPoint.position + smoothedMovement;
        
        
        // Calculate the maximum distance the endpoint can move to stay inside the cage of colliders
        float maxDistance = 0f;
        Vector3 direction = newEndPointPosition - previousPosition;
        RaycastHit hit;
        if (Physics.Raycast(previousPosition, direction, out hit))
        {
            maxDistance = hit.distance;
        }
        
        
        if(direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
        }
        newEndPointPosition = previousPosition + direction;
        
        
        
        // Check for collision
        if (!IsColliding(newEndPointPosition))
        {
            // Stop flashing visual effects
            portalConeBase.SetActive(false);
            if (coneSurfaceRenderer.materials.Length > 1)
            {
                AddMaterialToConeSurfaceRenderer(1);
            }
            
            previousPosition = rayEndPoint.position;
            rayEndPointRb.MovePosition(newEndPointPosition);
        }
        else
        {
            // Start flashing visual effects
            portalConeBase.SetActive(true);
            if (coneSurfaceRenderer.materials.Length < 2)
            {
                AddMaterialToConeSurfaceRenderer(2);
            }
            
            // Get the normal of the surface we're colliding with
            Vector3 collisionNormal = hit.normal;
            
            Vector3 safePosition = pointerMovement * Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD;
            
            
            
            
            // Project the movement vector onto the surface to create a sliding effect
            Vector3 slideDirection = Vector3.ProjectOnPlane(pointerMovement, collisionNormal);
            
            
            // Calculate the angle between the movement direction and the surface normal
            float angle = Vector3.Angle(pointerMovement, collisionNormal);

            // Apply a multiplier based on the angle to enhance sliding on less inclined surfaces
            float slidingSpeedMultiplier = Mathf.Lerp(1f, 3f, Mathf.InverseLerp(0, 45, angle)); // Adjust multiplier range and angle as needed
            Vector3 adjustedSlideDirection = slideDirection * slidingSpeedMultiplier;


            Vector3 movePosition = rayEndPoint.position - safePosition + adjustedSlideDirection;
            if (IsSphereCompletelySurrounded(movePosition))
            {
                // If the slide direction has a valid magnitude, move the object along the surface
                if (adjustedSlideDirection.magnitude > Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD)
                {
                    previousPosition = rayEndPoint.position;
                    rayEndPointRb.MovePosition(movePosition);
                }
                else
                {
                    // Fallback: Move back slightly to avoid getting stuck
                    previousPosition = rayEndPoint.position;
                    rayEndPointRb.MovePosition(rayEndPoint.position - safePosition);
                }
            }
            else
            {
                rayEndPointRb.MovePosition(previousPosition);
            }
            
            
            
            // Send haptic feedback to the hand controller
            ProvideHapticFeedback(index, colliderHapticFeedbackAmplitude, colliderHapticFeedbackDuration);
        }
        
        
        
        // Update alpha and beta angles of the cone, so that the values based on them to be sent to the
        // physical Sauron can be computed
        UpdateConeAngles(newEndPointPosition);
        
        finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);
        
    }
    
    
    private Vector3 CalculateSlidePosition(Vector3 currentPos, Vector3 movement)
    {
        RaycastHit hit;
        if (Physics.SphereCast(currentPos, sphereCollider.radius, movement, out hit))
        {
            // Calculate slide vector
            Vector3 slideVector = Vector3.ProjectOnPlane(movement, hit.normal);
            return currentPos + slideVector;
        }
        return currentPos;
    }
    
    
    
    /// <summary>
    /// Checks if the target position is colliding with any of the portal's colliders.
    /// </summary>
    /// <param name="targetPos"> Position that should be reached </param>
    /// <returns></returns>
    bool IsColliding(Vector3 targetPos)
    {
        
        Collider[] colliders = Physics.OverlapSphere(targetPos, sphereCollider.radius);
        foreach (var coll in colliders)
        {
            if (System.Array.Exists(portalBoxColliders, c => c == coll) || System.Array.Exists(portalSupportColliders, c => c == coll))
            {
                return true;
            }
        }
        return false;
    }
    
    
    /// <summary>
    /// Checks if the sphere is completely surrounded by colliders by casting raycasts in multiple directions.
    /// </summary>
    /// <param name="pos"> Position of the sphere </param>
    bool IsSphereCompletelySurrounded(Vector3 pos)
    {
        
        // Cast raycasts in multiple directions
        Vector3[] directions = {
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right,
            Vector3.forward, Vector3.back
        };

        foreach (Vector3 dir in directions)
        {
            if (!Physics.Raycast(pos, dir))
            {
                // Raycast didn't hit any collider in this direction
                return false;
            }
        }

        // If all raycasts hit colliders, the sphere is completely surrounded
        return true;
    }
    
    bool IsCollidingSimplified(Vector3 targetPos)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPos, sphereCollider.radius);
        foreach (var coll in colliders)
        {
            if (System.Array.Exists(portalColliders, c => c == coll))
            {
                return true;
            }
        }
        return false;

    }
    
    
    /// <summary>
    /// Remaps the distance moved by the hand controller to a movement multiplier that will be applied to the endpoint.
    /// </summary>
    /// <param name="distance"> Distance on which the multiplier definition is based </param>
    private float ComputeMovementMultiplier(float distance)
    {
        float movementMultiplier = 0f;
        
        // The movement multiplier is proportional to the distance moved by the controller and it can range between
        // the minimum and maximum movement multipliers values. The smaller the distance between two consecutive hand controller
        // positions, the smaller the movement multiplier to be applied to the endpoint. This should prevent the endpoint from
        // moving too much when the hand controller is moved slightly.
        movementMultiplier = RangeRemappingHelper.Remap(distance, Constants.XR_CONTROLLER_AVERAGE_MOVEMENT_DISTANCE, Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD, 
            sauronMaxMovementMultiplier, sauronMinMovementMultiplier);

        return movementMultiplier;
    }
    
    
    

    /// <summary>
    /// Updates the radius of the spiralwave rays based on the angle of the elevation.
    /// </summary>
    /// <param name="inactiveSpiralwaveRayToChange"> Inactive ray </param>
    /// <param name="activeSpiralwaveRayToChange"> Active ray </param>
    /// <param name="finalNewDistance"> New distance of the ray endpoint </param>
    private void UpdateLineRenderers(SpiralwaveRay inactiveSpiralwaveRayToChange, SpiralwaveRay activeSpiralwaveRayToChange, float finalNewDistance)
    {
        
        // Update the linerenderers' radius with the beta angle
        float betaOffset = 30f;
        float symmetricBetaElevationAngle = Mathf.Abs(betaElevationAngle - betaOffset);
        float newRadius = RangeRemappingHelper.Remap(symmetricBetaElevationAngle, Constants.SAURON_INCLINATION_SYMMETRIC_SERVO_ANGLE_LOW, 
            Constants.SAURON_INCLINATION_SYMMETRIC_SERVO_ANGLE_HIGH, Constants.SAURON_MAX_RADIUS, Constants.SAURON_MIN_RADIUS);

        inactiveSpiralwaveRayToChange.SetRadius(newRadius);
        activeSpiralwaveRayToChange.SetRadius(newRadius);


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
        
        if (triggerValue > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD)
        {
            isInControl = false;
        }

    }
    
    /// <summary>
    /// Emits haptic feedback to the controller that caused the collision.
    /// </summary>
    /// <param name="index"> Index of the controller </param>
    /// <param name="amplitude"> Intensity of the haptic feedback </param>
    /// <param name="duration"> Duration of the impulse of a single haptic feedback </param>
    public void ProvideHapticFeedback(int index, float amplitude, float duration)
    {
        
        // Trigger the haptic feedback on the controller that caused the collision
        xrControllers[index].SendHapticImpulse(amplitude, duration);
    }


    /// <summary>
    /// Updates the alpha and beta angles of the cone based on the new position of the ray's endpoint.
    /// </summary>
    /// <param name="newEndPointPosition"> New position of the ray endpoint </param>
    private void UpdateConeAngles(Vector3 newEndPointPosition)
    {
        
        Vector3 endPointVector = newEndPointPosition - coreCenter.position;
        
        Vector3 newCoordinateEndPointPosition = coneBaseCenter.InverseTransformPoint(newEndPointPosition);
        
        // Calculate the horizontal and vertical axes of the cone
        coneVerticalAxis = coneTip.position - coneBaseCenter.position;
        coneHorizontalAxis = coneBaseExtremity.position - coneBaseCenter.position; // coneBaseExtremity is positioned in the editor so that the chosen axis is X

        
        alphaRotationAngle = Mathf.Atan2(newCoordinateEndPointPosition.y, newCoordinateEndPointPosition.x) * Mathf.Rad2Deg;


        float radius = Mathf.Sqrt(Mathf.Pow(newCoordinateEndPointPosition.x, 2f) + Mathf.Pow(newCoordinateEndPointPosition.y, 2f));

        float betaOffset = 30f; // To avoid negative values becoming positive in the following checks
        betaElevationAngle = scale * Mathf.Atan2(radius, newCoordinateEndPointPosition.z) * Mathf.Rad2Deg;
        
        if (alphaRotationAngle < 0)
        {
            alphaRotationAngle = -alphaRotationAngle;
            betaElevationAngle = -betaElevationAngle;
        }
        
        // Bring the beta angle to the range [0, 60]
        betaElevationAngle += betaOffset;
        
        /*if (alphaRotationAngle > 180.0f)
        {
            alphaRotationAngle -= 180.0f;
        }*/
        
        
        
        

    }
    
    
    /// <summary>
    /// Assigns a new material to the cone's surface renderer.
    /// </summary>
    /// <param name="numberOfMaterials"> New number of materials of the object </param>
    void AddMaterialToConeSurfaceRenderer(int numberOfMaterials)
    {
        // Create a new array with an additional slot for the new material
        Material[] newMaterials = new Material[numberOfMaterials];

        // Copy the material(s) from the serialized array of materials
        for (int i = 0; i < numberOfMaterials; i++)
        {
            newMaterials[i] = coneSurfaceMaterials[i];
        }
        
        // Assign the new array back to coneSurfaceRenderer.materials
        coneSurfaceRenderer.materials = newMaterials;
    }
    
    
    
    
    public bool IsInControl()
    {
        return isInControl;
    }
    
    
    public float GetAlphaRotationAngle()
    {
        return alphaRotationAngle;
    }
    
    public float GetBetaElevationAngle()
    {
        return betaElevationAngle;
    }
    
    
    public List<ActionBasedController> GetControllers()
    {
        return xrControllers;
    }


}