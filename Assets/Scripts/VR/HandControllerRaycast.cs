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
            RemoveActiveChild(args.interactable.gameObject);
        }
    }
    
    private void AddActiveChild(GameObject newChild)
    {
        // Add the new object to the list of active children
        activeChildren.Add(newChild);
        newChild.transform.SetParent(transform);
        Debug.Log("Active child is " + newChild.name);
    }

    private void RemoveActiveChild(GameObject oldChild)
    {
        // If the object is in the list of active children, remove it
        if (activeChildren.Contains(oldChild))
        {
            Debug.Log("Active child is " + oldChild.name);
            Debug.Log("Active child parent is " + oldChild.transform.parent.name);
            oldChild.transform.parent = null;
            activeChildren.Remove(oldChild);
        }
    }
    
    private void SetActiveChild(GameObject newChild)
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
    }
    
    private void Update()
    {
        //HandleRaycasting();
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
