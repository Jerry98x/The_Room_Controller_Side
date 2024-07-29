using System.Collections;
using System.Collections.Generic;
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
    
    
    private MeshCollider portalMeshCollider;
    private BoxCollider portalBoxCollider;
    private MeshCollider[] portalSupportColliders;
    private SphereCollider sphereCollider;
    private Rigidbody rayEndPointRb;
    
    private Vector3 previousPosition;
    
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
    
    
    bool insideAnyCollider = false;
    
    
    
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
        Debug.Log($"Smoothed Movement Vector: {smoothedMovement}");

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
    
    
    
    
    public void HandleRayEndpointMovement(out float finalNewDistance)
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
        Debug.Log($"Smoothed Movement Vector: {smoothedMovement}");

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
            // Check on the box collider
            if(portalBoxCollider == coll)
            {
                Debug.Log("Toccato il box collider, dio serpente!");
                return true;
            }
            
            // Checks on the mesh colliders
            foreach (var portalCollider in portalSupportColliders)
            {
                if(portalCollider == coll)
                {
                    Debug.Log("Toccato uno dei mesh collider, dio sanguisuga!");
                    return true;
                }
            }
        }
        return false;
    }
    
    
    
    bool IsPositionInsideColliders(Vector3 position)
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
    }
    
    
    


    private void UpdateLineRenderers(SpiralwaveRay inactiveSpiralwaveRayToChange, SpiralwaveRay activeSpiralwaveRayToChange, float finalNewDistance)
    {
        
        
            
            
            
    }


}
