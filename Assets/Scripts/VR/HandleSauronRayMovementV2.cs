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
    
    
    
    
    
    // Colliders properties
    
    //private MeshCollider portalMeshCollider;
    private BoxCollider portalBoxCollider;
    private MeshCollider[] portalSupportColliders;
    private Collider[] portalColliders;
    
    private SphereCollider sphereCollider;
    private Rigidbody rayEndPointRb;
    
    private Vector3 previousPosition;
    
    
    
    // Movement properties
    
    private ActionBasedController xrController;
    private XRDirectInteractor interactor;
    private Pointer pointer;
    
    private Vector3 targetPosition;
    private Vector3 smoothedMovement;
    private float lerpSpeed = 30f;
    
    
    private float smoothTime = 0.01f;
    private Vector3 velocity = Vector3.zero;
    
    
    private bool overlapped = false;
    
    private SpiralwaveRay inactiveSpiralwaveRay;
    private SpiralwaveRay activeSpiralwaveRay;
    private float sauronMovementMultiplier;
    private bool isInControl = false;
    
    
    private bool insideAnyCollider = false;
    
    private float colliderHapticFeedbackAmplitude = 0.5f;
    private float colliderHapticFeedbackDuration = 0.5f;
    
    
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

            
                    
            // Invoke the event that will notify the Neto module of the distance change
            //onSauronRayDistanceChange?.Invoke(inactiveSpiralwaveRay, activeSpiralwaveRay, finalNewDistance);
            
            
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
        
        if (interactor != null || pointer != null)
        {
            
            inactiveSpiralwaveRay = transform.parent.GetComponentInChildren<SpiralwaveRay>();
            activeSpiralwaveRay = inactiveSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            sauronMovementMultiplier = inactiveSpiralwaveRay.GetEndPointObject().GetEndpointMovementMultiplier();
            
            isInControl = true;
        }
    }

    
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() == interactor)
        {
            
            inactiveSpiralwaveRay = null;
            activeSpiralwaveRay = null;

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
            if (triggerValue <= Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD)
            {
                isInControl = true;
            }
        }
    }
   
    
    
    
    public void HandleRayEndpointMovement()
    {
        
        Vector3 currentPointerPosition = pointer.transform.position;
        Vector3 previousPointerPosition = pointer.GetPreviousPosition();

        // Calculate the pointer's movement vector
        Vector3 pointerMovement = currentPointerPosition - previousPointerPosition;
        Debug.Log($"DIO: Pointer Movement Vector: {pointerMovement}");

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = pointerMovement * sauronMovementMultiplier;
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
        
        Vector3 currentPointerPosition = pointer.transform.position;
        Vector3 previousPointerPosition = pointer.GetPreviousPosition();

        // Calculate the pointer's movement vector
        Vector3 pointerMovement = currentPointerPosition - previousPointerPosition;
        Debug.Log($"DIO: Pointer Movement Vector: {pointerMovement}");

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = pointerMovement * sauronMovementMultiplier;
        Debug.Log($"DIO: Scaled Movement Vector: {scaledMovement}");
        
        // Smooth the movement vector
        smoothedMovement = Vector3.Lerp(smoothedMovement, scaledMovement, smoothTime);
        Debug.Log($"DIO: Smoothed Movement Vector: {smoothedMovement}");

        // Calculate new potential position of the ray's endpoint
        Vector3 newEndPointPosition = rayEndPoint.position + smoothedMovement;
        Debug.Log($"DIO: Target Position: {newEndPointPosition}");
        
        
        Vector3 adjustedPosition = CalculateSlidePosition(rayEndPoint.position, smoothedMovement);

        
        // Update alpha and beta angles of the cone, so that the values based on them to be sent to the
        // physical Sauron can be computed
        UpdateConeAngles(newEndPointPosition);
        
        
        // Check for collision
        
        //if (!IsCollidingSimplified(newEndPointPosition))
        if (!IsColliding(newEndPointPosition))
        {
            previousPosition = rayEndPoint.position;
            rayEndPointRb.MovePosition(newEndPointPosition);
            Debug.Log($"DIO: Sphere moved to: {newEndPointPosition}");
        }
        else
        {
            /*Collision detected, revert to previous position
            rayEndPointRb.MovePosition(previousPosition);
            Debug.Log("DIO: Collision detected, movement blocked.");*/
            
            
            // Send haptic feedback to the hand controller
            ProvideHapticFeedback(colliderHapticFeedbackAmplitude, colliderHapticFeedbackDuration);
        }
        
        
        
        /*if (!IsColliding(newEndPointPosition))
        {
            previousPosition = rayEndPoint.position;
            // Update the ray's endpoint position
            //rayEndPoint.position = newEndPointPosition;
            rayEndPointRb.MovePosition(newEndPointPosition);
            // Calculate the distance from coreCenter to the new endpoint position
            //finalNewDistance = Vector3.Distance(coreCenter.position, newEndPointPosition);
        }
        else
        {
            // Collision detected, calculate slide vector
            Vector3 slidePosition = CalculateSlidePosition(rayEndPoint.position, smoothedMovement);
            
            // Ensure the slide position is valid
            if (IsPositionInsideColliders(slidePosition))
            {
                previousPosition = rayEndPoint.position;
                rayEndPointRb.MovePosition(slidePosition);
            }
            else
            {
                rayEndPointRb.MovePosition(previousPosition);
            }
            rayEndPointRb.MovePosition(slidePosition);
            //rayEndPoint.position = previousPosition;
            //finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);
        }*/
        
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
        /*Collider[] colliders = new Collider[10]; // Should be enough, since there are two colliders
        Physics.OverlapSphereNonAlloc(targetPos, sphereCollider.radius, colliders, LayerMask.GetMask("SauronLayer"));
        foreach (var coll in colliders)
        {
            if (coll == portalMeshCollider || coll == portalBoxCollider)
            {
                return true;
            }
        }
        return false;*/
        
        
        
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
    
    
    


    private void UpdateLineRenderers(SpiralwaveRay inactiveSpiralwaveRayToChange, SpiralwaveRay activeSpiralwaveRayToChange, float finalNewDistance)
    {
        
        
            
            
            
    }
    
    
    
    
    private void HandleControlInterruption()
    {
        Debug.Log("XR CONTROLLER: " + xrController);

        bool backTriggerPressed = xrController.activateActionValue.action.ReadValue<float>() > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD;
        if (backTriggerPressed)
        {
            isInControl = false;
        }

    }
    
    public void ProvideHapticFeedback(float amplitude, float duration)
    {
        Debug.Log("XR CONTROLLER: " + xrController);

        // Trigger the haptic feedback on the controller
        xrController.SendHapticImpulse(amplitude, duration);
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


}
