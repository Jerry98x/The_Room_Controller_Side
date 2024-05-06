using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandControllerRaycast : MonoBehaviour
{
    private XRRayInteractor rayInteractor;
    private List<GameObject> activeChildren = new List<GameObject>(); // List of current active child objects
    private GameObject activeChild = null; // Reference to the current active child object
    private Vector3 initialInteractablePosition; // Initial position of the interactable
    private Vector3 initialEndPointPosition; // Initial position of the interactable's endpoint
    private bool isConstrained = false; // Flag to indicate if movement is constrained
    private float deltaError = 0.1f;
    //private float minY, maxY; // Define min and max y positions for movement constraint

    //public float maxRaycastDistance = 100f;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
    }
    
    
    private void HandleHoverEntered(HoverEnterEventArgs args)
    {
        LightningRay lightningRay = args.interactable.gameObject.TryGetComponent(out LightningRay interactable) ? interactable : null;
        EndPoint endPoint = interactable.gameObject.GetComponentInChildren<EndPoint>();
        initialEndPointPosition = endPoint.GetInitialPosition();
        if (lightningRay != null)
        {
           initialInteractablePosition = lightningRay.GetInitialPosition(); 
        }
        Debug.Log("THIS IS THE GAME OBJECT: " + args.interactable.gameObject.name);
        if (endPoint != null)
        {
            
            //SetActiveChild(interactable.gameObject);
            AddActiveChild(args.interactable.gameObject);
        }
        
    }
    
    private void HandleHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Hover exited event triggered");
        Debug.Log("args.interactable.gameObject: " + args.interactable.gameObject);
        Debug.Log("activeChild: " + activeChild);
        if (activeChildren.Contains(args.interactable.gameObject))
        {
            Debug.Log("Hover exited event triggered for active child: if entered");
            //SetActiveChild(null);
            isConstrained = false;
            RemoveActiveChild(args.interactable.gameObject);
        }
    }
    
    private void AddActiveChild(GameObject newChild)
    {
        // Add the new object to the list of active children, if none of the other rays objects are already in the list
        if (gameObject.GetComponentInChildren<XRSimpleInteractable>() == null)
        { 
            activeChildren.Add(newChild);
            newChild.transform.SetParent(transform);
        }
        
        // Check if the object has a SinewaveRay component
        SinewaveRay sinewaveRay = newChild.GetComponentInChildren<SinewaveRay>();
        if (sinewaveRay != null)
        {
            // If it does, set the isConstrained flag to true
            isConstrained = true;
        }

        
    }

    private void RemoveActiveChild(GameObject oldChild)
    {
        Debug.Log("Entered RemoveActiveChild method");
        // If the object is in the list of active children, remove it
        if (!isConstrained && activeChildren.Contains(oldChild))
        {
            Debug.Log("Entered if statement in RemoveActiveChild method");
            oldChild.transform.parent = null;
            activeChildren.Remove(oldChild);
            Debug.Log("Active children are " + activeChildren.Count + ": " + activeChildren);
        }
    }
    
    private void Update()
    {
        foreach (GameObject child in activeChildren)
        {
            // If movement is constrained, clamp the x or y position of the active child
            if (isConstrained)
            {
                SinewaveRay sinewaveRay = child.GetComponentInChildren<SinewaveRay>();
                Transform endPoint = sinewaveRay.GetEndPoint();
                if (sinewaveRay != null)
                {
                    if (sinewaveRay.IsHorizontal())
                    {
                        HorizontalConstraint horizontalConstraint = child.GetComponentInChildren<HorizontalConstraint>();
                        List<float> limits = horizontalConstraint.GetLimits();
                        // Constrain movement to horizontal
                        Vector3 endPointPosition = endPoint.transform.position;
                        
                        // Limits are defined as [-x, x]
                        endPointPosition.x = Mathf.Clamp(endPointPosition.x, initialEndPointPosition.x + limits[0], initialEndPointPosition.x + limits[1]);
                        endPointPosition.y = initialEndPointPosition.y;
                        endPointPosition.z = initialEndPointPosition.z;
                        
                        Vector3 interactablePosition = child.transform.position;
                        interactablePosition.x = Mathf.Clamp(interactablePosition.x, initialInteractablePosition.x + limits[0], initialInteractablePosition.x + limits[1]);
                        interactablePosition.y = initialInteractablePosition.y;
                        interactablePosition.z = initialInteractablePosition.z;
                        
                        child.transform.position = interactablePosition;
                        endPoint.transform.position = endPointPosition;
                    }
                    else
                    {
                        VerticalConstraint verticalConstraint = child.GetComponentInChildren<VerticalConstraint>();
                        List<float> limits = verticalConstraint.GetLimits();
                        // Constrain movement to vertical
                        Vector3 endPointPosition = endPoint.transform.position;
                        endPointPosition.x = initialEndPointPosition.x;
                        // Limits are defined as [-y, y]
                        endPointPosition.y = Mathf.Clamp(endPointPosition.y, initialEndPointPosition.y + limits[0], initialEndPointPosition.y + limits[1]);
                        endPointPosition.z = initialEndPointPosition.z;
                        
                        Vector3 interactablePosition = child.transform.position;
                        interactablePosition.x = initialInteractablePosition.x;
                        interactablePosition.y = Mathf.Clamp(interactablePosition.y, initialInteractablePosition.y + limits[0], initialInteractablePosition.y + limits[1]);
                        interactablePosition.z = initialInteractablePosition.z;
                        
                        child.transform.position = interactablePosition;
                        endPoint.transform.position = endPointPosition;
                    }
                }
            }
        }
        

        
        
    }
    
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * maxRaycastDistance);
    }*/
    
    /*private void HandleRaycasting()
    {
        /*Debug.Log(gameObject.name + " is handling raycasting");
        Debug.Log("transform.position: " + transform.position);
        Debug.Log("transform.forward: " + transform.forward);
        Debug.DrawLine(transform.position, transform.forward * maxRaycastDistance, Color.red, 2f);#1#
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit raycastHit;


        if (Physics.Raycast(ray, out raycastHit, maxRaycastDistance))
        {
            //Debug.Log("Raycast hit: " + raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out EndPoint endPoint))
            {
                //Debug.Log("Raycast hit an endpoint");
                // The ray has hit an endPoint
                if(transform.TryGetComponent(out XRController handController))
                {
                    // If there is a current active child, remove it from the parent
                    /*if (activeChild != null)
                    {
                        Debug.Log("Active child is " + activeChild.name);
                        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                        activeChild.transform.parent = null;
                    }#1#
                    
                    // Set the hit object as the new active child
                    SetActiveChild(raycastHit.transform.parent.gameObject);
                    
                    /#1#/ Control the endpoint
                    endPoint.Move(transform.forward, 1f); // Move the endpoint 1 unit along the forward direction#1#

                }
            }
        }
        else
        {
            //Debug.Log("Raycast did not hit anything");
        }
    }*/
    
    

}
