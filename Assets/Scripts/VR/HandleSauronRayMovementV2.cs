using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniColliderInterpolator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleSauronRayMovementV2 : MonoBehaviour
{
    
    
    public UnityEvent<SpiralwaveRay, SpiralwaveRay, float> onSauronRayDistanceChange;
    
    

    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;

    //[SerializeField] private Collider portalMeshCollider;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject portalSimplified;
    
    
    
    
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
    private BoxCollider portalBoxCollider;
    private MeshCollider[] portalSupportColliders;
    private Collider[] portalColliders;
    
    private SphereCollider sphereCollider;
    private Rigidbody rayEndPointRb;
    
    private Vector3 previousPosition;
    
    
    
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
    
    private float colliderHapticFeedbackAmplitude = 0.5f;
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
        
        //portalMeshCollider = portal.GetComponent<MeshCollider>();
        
        portalBoxCollider = portal.GetComponent<BoxCollider>();
        portalSupportColliders = portal.GetComponents<MeshCollider>();
        
        portalColliders = portalSimplified.GetComponentsInChildren<Collider>();
        
        
        sphereCollider = rayEndPoint.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("rayEndPoint does not have a SphereCollider component.");
        }
        
        previousPosition = rayEndPoint.position;
        targetPosition = rayEndPoint.position;
        
        
        // Listeners
        onSauronRayDistanceChange.AddListener(UpdateLineRenderers);
    }
    
    
    private void FixedUpdate()
    {
        if (isInControl)
        {
            
            // Handle ray's movement
            //HandleRayEndpointMovement();
            
            HandleRayEndpointMovement(out float finalNewDistance);
            
            
            
            /*// Lerp to the target position
            rayEndPoint.position = Vector3.Lerp(rayEndPoint.position, targetPosition, lerpSpeed * Time.deltaTime);

            // Calculate final new distance
            float finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);*/

            
                    
            // Invoke the event that will notify the Sauron module of the distance change
            onSauronRayDistanceChange?.Invoke(inactiveSpiralwaveRay, activeSpiralwaveRay, finalNewDistance);
            
            
            // Handle control interruption
            HandleControlInterruption();
            
        }
        
        
    }
    

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

    
    void OnTriggerExit(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponent<XRDirectInteractor>();
        
        if (interactor != null && interactors.Contains(interactor))
        {

            int index = interactors.IndexOf(interactor);
            Debug.Log("Removing interactor at index: " + index);
            
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
   
    
    
    
    public void HandleRayEndpointMovement()
    {
        
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
        Debug.Log($"DIO: Pointer Movement Vector: {pointerMovement}");

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = pointerMovement * sauronMaxMovementMultiplier;
        Debug.Log($"DIO: Scaled Movement Vector: {scaledMovement}");
        
        // Smooth the movement vector
        smoothedMovement = Vector3.Lerp(smoothedMovement, scaledMovement, smoothTime);
        Debug.Log($"DIO: Smoothed Movement Vector: {smoothedMovement}");

        // Calculate new potential position of the ray's endpoint
        Vector3 newEndPointPosition = rayEndPoint.position + smoothedMovement;
        Debug.Log($"DIO: Target Position: {newEndPointPosition}");
        
        
        
        
        // Check for collision
        if (!IsColliding(newEndPointPosition))
        {
            previousPosition = rayEndPoint.position;
            rayEndPointRb.MovePosition(newEndPointPosition);
            Debug.Log($"DIO: Sphere moved to: {newEndPointPosition}");
        }
        else
        {
            // Collision detected, revert to previous position
            rayEndPointRb.MovePosition(previousPosition);
            Debug.Log("DIO: Collision detected, movement blocked.");
            
        }

        
        


        /*// Check if the new position is inside any of the portal support colliders
        bool isInside = false;
        foreach (MeshCollider coll in portalSupportColliders)
        {
            if (coll.bounds.Contains(newEndPointPosition))
            {
                isInside = true;
                break;
            }
        }
        
        if (isInside)
        {
            targetPosition = newEndPointPosition;
        }
        else
        {
            // Find the nearest point inside the colliders and adjust for sphere radius and potential gap
            Vector3 closestPoint = newEndPointPosition;
            float minDistance = float.MaxValue;

            foreach (MeshCollider boundary in portalSupportColliders)
            {
                Vector3 point = boundary.ClosestPoint(newEndPointPosition);
                float distance = Vector3.Distance(newEndPointPosition, point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            Vector3 directionToClosestPoint = (closestPoint - newEndPointPosition).normalized;
            float penetrationDepth = sphereCollider.radius - minDistance;

            // Ensure the sphere does not get stuck in the gap
            if (penetrationDepth > 0)
            {
                targetPosition = newEndPointPosition + directionToClosestPoint * penetrationDepth;
            }
            else
            {
                // Move the sphere closer to the boundary to avoid the gap
                targetPosition = closestPoint;
            }
        }*/
        
        
        
    }
    
    
    
    
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
        
        
        // Update alpha and beta angles of the cone, so that the values based on them to be sent to the
        // physical Sauron can be computed
        UpdateConeAngles(newEndPointPosition);
        
        
        // Check for collision
        if (!IsColliding(newEndPointPosition))
        {
            previousPosition = rayEndPoint.position;
            rayEndPointRb.MovePosition(newEndPointPosition);
            Debug.Log($"DIO: Sphere moved to: {newEndPointPosition}");
        }
        else
        {
            previousPosition = rayEndPoint.position;
            // Move back of the minimum distance to make it easier to avoid getting stuck
            rayEndPointRb.MovePosition(rayEndPoint.position - pointerMovement * Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD);
            // rayEndPointRb.MovePosition(rayEndPoint.position + hit.normal * Constants.XR_CONTROLLER_MOVEMENT_THRESHOLD);
            // Send haptic feedback to the hand controller
            ProvideHapticFeedback(index, colliderHapticFeedbackAmplitude, colliderHapticFeedbackDuration);
        }
        
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
    
    
    
    
    
    
    bool IsColliding(Vector3 targetPos)
    {
        
        Collider[] colliders = Physics.OverlapSphere(targetPos, sphereCollider.radius);
        foreach (var coll in colliders)
        {
            if (coll == portalBoxCollider || System.Array.Exists(portalSupportColliders, c => c == coll))
            {
                return true;
            }
        }
        return false;
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
    
    
    
    /*bool IsPositionInsideColliders(Vector3 position)
    {
        foreach (var coll in portalSupportColliders)
        {
            if (coll.bounds.Contains(position))
            {
                return true;
            }
        }

        if (portalBoxCollider.bounds.Contains(position))
        {
            return true;
        }

        return false;
    }*/
    
    
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
    
    public void ProvideHapticFeedback(int index, float amplitude, float duration)
    {
        
        // Trigger the haptic feedback on the controller that caused the collision
        xrControllers[index].SendHapticImpulse(amplitude, duration);
    }




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
    
    
    
    
    /*private void UpdateConeAngles(Vector3 newEndPointPosition)
    {
        
        Vector3 endPointVector = newEndPointPosition - coreCenter.position;
        
        // Calculate the horizontal and vertical axes of the cone
        coneVerticalAxis = coneTip.position - coneBaseCenter.position;
        coneHorizontalAxis = coneBaseExtremity.position - coneBaseCenter.position; // coneBaseExtremity is positioned in the editor so that the chosen axis is X
        
        
        // Alpha rotation angle is computed by finding the angle between coneHorizontalAxis and the projection of
        // endPointVector on the horizontal plane
        Vector3 endPointVectorProjection = Vector3.ProjectOnPlane(endPointVector, coneVerticalAxis);
        alphaRotationAngle = Vector3.Angle(coneHorizontalAxis, endPointVectorProjection);
        
        
        // Beta elevation angle is computed by finding the angle between the projection of endPointVector on
        // the horizontal plane and endPointVector itself, and then subtracting it from 90 degrees or 180 degrees.
        // Since on the physical Sauron the angle has a range of [0, 60] with 30° being the central value
        // (when the endPointVector is vertical), the previously computed Alpha rotation angle is used to check
        // whether the endPointVector was on the left side or right side. If it was on the left side, the angle is
        // subtracted from 90 degrees. If it was on the right side, the angle is subtracted from 180 degrees and an
        // 60 degrees angle is subtracted, since the physical Sauron counts the angel starting from the lowest inclination
        // on the left.
        // Finally, the angle is capped between 0 and 60 degrees, if necessary.
        float betaRotationInnerAngle;
        float clampedAngle = Mathf.Clamp(Vector3.Angle(endPointVectorProjection, endPointVector), Constants.SAURON_OFFSET_INCLINATION_SERVO_ANGLE, 90.0f); 
        if(alphaRotationAngle > 90.0f)
        {
            betaRotationInnerAngle = 180.0f - clampedAngle - Constants.SAURON_OFFSET_INCLINATION_SERVO_ANGLE;
        }
        else
        {
            betaRotationInnerAngle = clampedAngle - Constants.SAURON_OFFSET_INCLINATION_SERVO_ANGLE;
        }

        betaElevationAngle = betaRotationInnerAngle;
        //betaElevationAngle = Mathf.Clamp(betaRotationInnerAngle, 0, 60);



        // Beta elevation angle is computed by finding the angle between the projection of endPointVector on
        // the horizontal plane and conVerticalAxis, capping the value between 0 and 60 degrees;
        // It may be necessary to add an offset to the angle
        /*Vector3 endPointVectorProjectionHorizontal = Vector3.ProjectOnPlane(endPointVector, coneHorizontalAxis);
        betaElevationAngle = Vector3.SignedAngle(coneVerticalAxis, endPointVectorProjectionHorizontal, coneHorizontalAxis);
        betaElevationAngle = Mathf.Clamp(betaElevationAngle, 0, 60);#1#








        // Calculate the angles
        /*alphaRotationAngle = Vector3.SignedAngle(coneHorizontalAxis, endPointVector, coneVerticalAxis);
        betaElevationAngle = Vector3.SignedAngle(coneVerticalAxis, endPointVector, coneHorizontalAxis);#1#

    }*/
    
    
    
    
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