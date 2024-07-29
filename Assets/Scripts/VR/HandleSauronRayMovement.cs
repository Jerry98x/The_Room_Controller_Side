using System.Collections;
using System.Collections.Generic;
using UniColliderInterpolator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class HandleSauronRayMovement : MonoBehaviour
{
    
    
    public UnityEvent<SpiralwaveRay, SpiralwaveRay, float> onSauronRayDistanceChange;
    
    

    [SerializeField] private Transform coreCenter;
    [SerializeField] private Transform rayEndPoint;

    //[SerializeField] private Collider portalMeshCollider;
    [SerializeField] private GameObject portal;
    
    
    private MeshCollider[] portalSupportColliders;
    private SphereCollider sphereCollider;
    
    private ActionBasedController xrController;
    private XRDirectInteractor interactor;
    private Pointer pointer;
    
    
    private Vector3 targetPosition;
    private float lerpSpeed = 30f;
    
    
    private float smoothTime = 0.03f;
    private Vector3 velocity = Vector3.zero;
    
    private Rigidbody rayEndPointRb;
    
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
        
        portalSupportColliders = portal.GetComponents<MeshCollider>();
        
        
        sphereCollider = rayEndPoint.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("rayEndPoint does not have a SphereCollider component.");
        }
        
        targetPosition = rayEndPoint.position;
        
        
        // Listeners
        onSauronRayDistanceChange.AddListener(UpdateLineRenderers);
    }
    
    
    private void Update()
    {
        if (isInControl)
        {
            
            // Handle ray's movement
            HandleRayEndpointMovement();
            
            
            
            // Lerp to the target position
            rayEndPoint.position = Vector3.Lerp(rayEndPoint.position, targetPosition, lerpSpeed * Time.deltaTime);

            // Calculate final new distance
            float finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);

            
                    
            // Invoke the event that will notify the Neto module of the distance change
            onSauronRayDistanceChange?.Invoke(inactiveSpiralwaveRay, activeSpiralwaveRay, finalNewDistance);
            
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

        // Apply a multiplier to make the movement more significant
        Vector3 scaledMovement = pointerMovement * sauronMovementMultiplier;

        // Calculate new potential position of the ray's endpoint
        Vector3 newEndPointPosition = rayEndPoint.position + scaledMovement;


        // Check if the new position is inside any of the portal support colliders
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
        }
        
        
        
        
        
        
        
        
        /*// Prevent the rayEndPoint from moving outside the portal support colliders
        bool isInside = false;
        foreach (Collider coll in portalSupportColliders)
        {
            if (coll.bounds.Contains(newEndPointPosition))
            {
                isInside = true;
                break;
            }
        }

        if (!isInside)
        {
            // If the new position is outside, revert the movement
            rayEndPointRb.MovePosition(rayEndPoint.position);
        }
        else
        {
            // Update the target position
            targetPosition = newEndPointPosition;

            // Calculate final new distance
            float finalNewDistance = Vector3.Distance(coreCenter.position, rayEndPoint.position);

            // Invoke the event that will notify the Neto module of the distance change
            onSauronRayDistanceChange?.Invoke(null, null, finalNewDistance);
        }*/
        
        
    }
    
    
    


    private void UpdateLineRenderers(SpiralwaveRay inactiveSpiralwaveRayToChange, SpiralwaveRay activeSpiralwaveRayToChange, float finalNewDistance)
    {
        
        
            
            
            
    }


}
