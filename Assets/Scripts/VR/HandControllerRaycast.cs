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
    private Vector3 initialInteractablePosition; // Initial position of the interactable object
    private bool isConstrained = false; // Flag to indicate if movement is constrained
    private float deltaError = 0.5f;
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
        
        EndPoint endPoint = args.interactable.gameObject.GetComponentInChildren<EndPoint>();
        initialInteractablePosition = endPoint.GetInitialPosition();
        Debug.Log("THIS IS THE GAME OBJECT: " + args.interactable.gameObject.name);
        if (endPoint != null)
        {
            
            //SetActiveChild(interactable.gameObject);
            AddActiveChild(args.interactable.gameObject);
        }
        
    }
    
    private void HandleHoverExited(HoverExitEventArgs args)
    {
        if (args.interactable.gameObject == activeChild)
        {
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

            /*// Get the VerticalConstraint component of the object
            VerticalConstraint verticalConstraint = endPoint.GetComponent<VerticalConstraint>();
            if (verticalConstraint != null)
            {
                // If the object has a VerticalConstraint component, set the minY and maxY values
                List<float> limits = verticalConstraint.GetLimits();
                minY = limits[0];
                maxY = limits[1];
            }*/
        }

        
    }

    private void RemoveActiveChild(GameObject oldChild)
    {
        // If the object is in the list of active children, remove it
        if (!isConstrained && activeChildren.Contains(oldChild))
        {
            oldChild.transform.parent = null;
            activeChildren.Remove(oldChild);
            Debug.Log("Active children are " + activeChildren.Count + ": " + activeChildren);
        }
    }
    
    /*private void SetActiveChild(GameObject newChild)
    {
        // If there is a current active child, remove it from the parent
        if (activeChild != null)
        {
            Debug.Log("Active child is " + activeChild.name);
            Debug.Log("Active child parent is " + activeChild.transform.parent.name);
            activeChild.transform.parent = null;
        }


        // Set the new object as the active child
        activeChild = newChild;
        activeChild.transform.SetParent(transform);
        Debug.Log("Active child is " + activeChild.name);
    }*/
    
    private void Update()
    {
        Debug.Log("Number of active children: " + activeChildren.Count);
        foreach (GameObject activeChild in activeChildren)
        {
            Debug.Log("Active child is " + activeChild.name + " and its position is " + activeChild.transform.position);
            // If movement is constrained, clamp the x or y position of the active child
            if (isConstrained)
            {
                SinewaveRay sinewaveRay = activeChild.GetComponentInChildren<SinewaveRay>();
                Transform endPoint = sinewaveRay.GetEndPoint();
                if (sinewaveRay != null)
                {
                    Debug.Log("SinewaveRay is not null");
                    if (sinewaveRay.IsHorizontal())
                    {
                        Debug.Log("SinewaveRay is horizontal");
                        HorizontalConstraint horizontalConstraint = activeChild.GetComponentInChildren<HorizontalConstraint>();
                        List<float> limits = horizontalConstraint.GetLimits();
                        // Constrain movement to horizontal
                        Vector3 position = endPoint.transform.position;
                        // Limits are defined as [-x, x]
                        position.x = Mathf.Clamp(position.x, initialInteractablePosition.x + limits[0], initialInteractablePosition.x + limits[1]);
                        position.y = initialInteractablePosition.y;
                        position.z = initialInteractablePosition.z;
                        endPoint.transform.position = position;
                    }
                    else
                    {
                        Debug.Log("SinewaveRay is vertical");
                        VerticalConstraint verticalConstraint = activeChild.GetComponentInChildren<VerticalConstraint>();
                        List<float> limits = verticalConstraint.GetLimits();
                        Debug.Log("Limits: " + limits[0] + " " + limits[1]);
                        // Constrain movement to vertical
                        Vector3 position = endPoint.transform.position;
                        position.x = initialInteractablePosition.x;
                        // Limits are defined as [-y, y]
                        position.y = Mathf.Clamp(position.y, initialInteractablePosition.y + limits[0], initialInteractablePosition.y + limits[1]);
                        position.z = initialInteractablePosition.z;
                        Debug.Log("Clamped position: " + position);
                        endPoint.transform.position = position;
                    }
                    Debug.Log("New position: " + activeChild.transform.position);
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
