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
    private Vector3 lastInteractablePosition;
    private Vector3 lastEndPointPosition;
    //private float minY, maxY; // Define min and max y positions for movement constraint

    //public float maxRaycastDistance = 100f;

    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
    }
    
    
    //
    private void HandleHoverEntered(HoverEnterEventArgs args)
    {
        LightningRay lightningRay = args.interactable.gameObject.TryGetComponent(out LightningRay interactable) ? interactable : null;
        EndPoint endPoint = interactable.gameObject.GetComponentInChildren<EndPoint>();
        initialEndPointPosition = endPoint.GetInitialPosition();
        if (lightningRay != null)
        {
           initialInteractablePosition = lightningRay.GetInitialPosition(); 
           lastEndPointPosition = initialEndPointPosition;
           lastInteractablePosition = initialInteractablePosition;
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
        
        // Create a temporary copy of the activeChildren list for iteration
        List<GameObject> tempActiveChildren = new List<GameObject>(activeChildren);

        
        foreach (GameObject child in tempActiveChildren)
        {
            
            // If movement is constrained, clamp the x or y position of the active child
            if (isConstrained)
            {
                
                //HandleEndPointRaycast(child);
                
                
                SinewaveRay sinewaveRay = child.GetComponentInChildren<SinewaveRay>();
                if (sinewaveRay != null)
                {
                    HandleSinewaveConstraints(child, sinewaveRay);
                }
                
                SpiralwaveRay spiralwaveRay = child.GetComponentInChildren<SpiralwaveRay>();
                if (spiralwaveRay != null)
                {
                    HandleSpiralwaveConstraints(child, spiralwaveRay);
                }
            }
        }
        

        
        
    }


    private void HandleSinewaveConstraints(GameObject child, SinewaveRay sinewaveRay)
    {
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

    /*private void HandleSpiralwaveConstraints(GameObject child, SpiralwaveRay spiralwaveRay)
    {
        Transform endPoint = spiralwaveRay.GetEndPoint();
        CircularConstraint circularConstraint = child.GetComponentInChildren<CircularConstraint>();
        if (circularConstraint != null)
        {
            float radius = circularConstraint.GetRadius();

            // Calculate the angle based on the current position
            Vector3 direction = endPoint.position - initialEndPointPosition;
            float angle = Mathf.Atan2(direction.y, direction.x);

            // Constrain the angle within the desired range
            float minAngle = Mathf.Deg2Rad * 270f; // 270° in radians
            float maxAngle = Mathf.Deg2Rad * (270f + 360f); // 270° + 360° in radians
            angle = Mathf.Clamp(angle, minAngle, maxAngle);
            
            // Calculate the new position based on the circular path
            Vector3 endPointPosition = new Vector3(
                initialEndPointPosition.x + radius * Mathf.Cos(angle),
                initialEndPointPosition.y + radius * Mathf.Sin(angle),
                initialEndPointPosition.z
            );

            // Update the position of the endPoint
            endPoint.transform.position = endPointPosition;
        }
    }*/
    
    
    private void HandleSpiralwaveConstraints(GameObject child, SpiralwaveRay spiralwaveRay)
    {
        Transform endPoint = spiralwaveRay.GetEndPoint();
        CircularConstraint circularConstraint = child.GetComponentInChildren<CircularConstraint>();
        if (circularConstraint != null)
        {
            float radius = circularConstraint.GetRadius();

            // Calculate the direction from the endPoint to the child
            Vector3 direction = child.transform.position - endPoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x);

            // Constrain the angle within the desired range
            float minAngle = Mathf.Deg2Rad * 270f; // 270° in radians
            float maxAngle = Mathf.Deg2Rad * (270f + 360f); // 270° + 360° in radians
            angle = Mathf.Clamp(angle, minAngle, maxAngle);

            // Calculate the new position of the child based on the circular path with the endPoint as the center
            Vector3 childPosition = new Vector3(
                endPoint.position.x + radius * Mathf.Cos(angle),
                endPoint.position.y + radius * Mathf.Sin(angle),
                endPoint.position.z
            );

            // Update the position of the child
            child.transform.position = childPosition;
        }
    }
    
    

    
    



    private void HandleEndPointRaycast(GameObject child)
    {
        
        Debug.Log("Entered HandleEndPointRaycast method");
        
        // Enable the rayInteractor at the start of the method
        rayInteractor.enabled = true;
        
        float interactDistance = 1f;
        EndPoint endPoint = child.GetComponentInChildren<EndPoint>();
        Vector3 direction = new Vector3(0, 0, endPoint.transform.position.z + 1);
        int layerMask = LayerMask.GetMask("NetoLayerWall");
        
        
        if(Physics.SphereCast(endPoint.transform.position, endPoint.GetComponent<SphereCollider>().radius, direction, out RaycastHit raycastHit, interactDistance, layerMask))
        {
            Debug.Log("Raycast hit: " + raycastHit.transform.name);
            Debug.Log("Layermask name: " + raycastHit.transform.gameObject.layer);
            if(raycastHit.transform.GetComponent<MovementArea>() != null)
            {
                Debug.Log("Raycast hit a movement area");
                // The ray has hit a movement area and it can be controlled
                rayInteractor.enabled = true;
                // Save the last useful positions
                lastEndPointPosition = endPoint.transform.position;
                lastInteractablePosition = child.transform.position;
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything");
            endPoint.transform.position = lastEndPointPosition;
            child.transform.position = lastInteractablePosition;
            //rayInteractor.enabled = false;
        }
        
        /*lastEndPointPosition = endPoint.transform.position;
        lastInteractablePosition = child.transform.position;
        */

        
        // Draw the direction of the sphere cast
        Debug.DrawRay(endPoint.transform.position, direction * interactDistance, Color.green);
        // Draw the sphere itself at the start of the cast
        //Debug.DrawWireSphere(endPoint.transform.position, endPoint.GetComponent<SphereCollider>().radius, Color.red);

    }
}
