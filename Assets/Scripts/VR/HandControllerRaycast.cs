using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class that handles the raycast interaction with the modules of the Room and the movement constraints
/// </summary>
/// <remarks>
/// Used for the first version of the Room, where action/movement is on the endpoint (the whole ray moves in the space)
/// and perception of the Visitor is on the ray itself (reflected in the change in the characteristics of the ray)
/// </remarks>
public class HandControllerRaycast : MonoBehaviour
{
    
    //TODO: Clean the code and remove unnecessary comments and debug logs
    
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


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the rayInteractor and subscribes to the events for hover entered and hover exited
    /// </summary>
    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
    }
    
    
    /// <summary>
    /// Handles the active children (who become children of the HandControllerRaycast object)
    /// and updates their position applying the movement area constraints at each frame
    /// </summary>
    /// <remarks>
    /// The list approach is not strictly necessary because there is only one element, but it worked better.
    /// </remarks>
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
    

    #endregion




    #region Relevant functions

    /// <summary>
    /// Handles the hover entered event and adds the interactable object to the list of active children
    /// </summary>
    /// <param name="args"> Event data </param>
    public void HandleHoverEntered(HoverEnterEventArgs args)
    {
        RoomNetoElement roomNetoElement = args.interactable.gameObject.TryGetComponent(out RoomNetoElement interactable) ? interactable : null;
        RayEndPoint rayEndPoint = interactable.gameObject.GetComponentInChildren<RayEndPoint>();
        initialEndPointPosition = rayEndPoint.GetInitialPosition();
        if (roomNetoElement != null)
        {
           initialInteractablePosition = roomNetoElement.GetInitialPosition(); 
           lastEndPointPosition = initialEndPointPosition;
           lastInteractablePosition = initialInteractablePosition;
        }
        Debug.Log("THIS IS THE GAME OBJECT: " + args.interactable.gameObject.name);
        if (rayEndPoint != null)
        {
            
            //SetActiveChild(interactable.gameObject);
            AddActiveChild(args.interactable.gameObject);
        }
        
    }
    
    /// <summary>
    /// Handles the hover exited event and remove the interactable object from the list of active children
    /// </summary>
    /// <param name="args"> Event data </param>
    public void HandleHoverExited(HoverExitEventArgs args)
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
    
    /// <summary>
    /// Adds the object to the list of active children and sets it as the active child
    /// </summary>
    /// <param name="newChild"> Object that is becoming a child </param>
    private void AddActiveChild(GameObject newChild)
    {
        // Add the new object to the list of active children, if none of the other rays objects are already in the list
        if (gameObject.GetComponentInChildren<XRSimpleInteractable>() == null)
        { 
            activeChildren.Add(newChild);
            newChild.transform.SetParent(transform);
        }
        
        //TODO: same for SpiralwaveRay component
        // Check if the object has a SinewaveRay component
        SinewaveRay sinewaveRay = newChild.GetComponentInChildren<SinewaveRay>();
        if (sinewaveRay != null)
        {
            // If it does, set the isConstrained flag to true
            isConstrained = true;
        }

        
    }

    /// <summary>
    /// Removes the object from the list of active children and frees it from being a child
    /// </summary>
    /// <param name="oldChild"> Object that is being freed from being a child  </param>
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
    

    /// <summary>
    /// Applies the linear constraints to the movement of the endpoint of the sinusoidal ray active child
    /// (and so the ray itself), which is a Neto module, by limiting it along the inclination direction
    /// and by clamping the x and y positions of the interactable object
    /// </summary>
    /// <param name="child"> The active child of the hand controller </param>
    /// <param name="sinewaveRay"> The sinewave component of the object </param>
    private void HandleSinewaveConstraints(GameObject child, SinewaveRay sinewaveRay)
    {
        Transform endPoint = sinewaveRay.GetEndPoint();
        if (sinewaveRay != null)
        {
            Constraints constraints = child.GetComponentInChildren<Constraints>();
            List<float> limits = constraints.GetLimits();
            // Constrain movement depending on the inclination of the sinewave
            Vector3 endPointPosition = endPoint.transform.position;

            endPointPosition.x = Mathf.Clamp(endPointPosition.x, initialEndPointPosition.x + limits[0]*Mathf.Cos(sinewaveRay.GetInclination()*Mathf.Deg2Rad), initialEndPointPosition.x + limits[1]*Mathf.Cos(sinewaveRay.GetInclination()*Mathf.Deg2Rad));
            endPointPosition.y = Mathf.Clamp(endPointPosition.y, initialEndPointPosition.y + limits[0]*Mathf.Sin(sinewaveRay.GetInclination()*Mathf.Deg2Rad), initialEndPointPosition.y + limits[1]*Mathf.Sin(sinewaveRay.GetInclination()*Mathf.Deg2Rad));
            endPointPosition.z = initialEndPointPosition.z;
            Debug.Log("endPointPosition: " + endPointPosition);
            
            Vector3 interactablePosition = child.transform.position;
            interactablePosition.x = Mathf.Clamp(interactablePosition.x, initialInteractablePosition.x + limits[0]*Mathf.Cos(sinewaveRay.GetInclination()*Mathf.Deg2Rad), initialInteractablePosition.x + limits[1]*Mathf.Cos(sinewaveRay.GetInclination()*Mathf.Deg2Rad));
            interactablePosition.y = Mathf.Clamp(interactablePosition.y, initialInteractablePosition.y + limits[0]*Mathf.Sin(sinewaveRay.GetInclination()*Mathf.Deg2Rad), initialInteractablePosition.y + limits[1]*Mathf.Sin(sinewaveRay.GetInclination()*Mathf.Deg2Rad));
            interactablePosition.z = initialInteractablePosition.z;
            Debug.Log("interactablePosition: " + interactablePosition);
            
            child.transform.position = interactablePosition;
            endPoint.transform.position = endPointPosition;
            
            
            /*if (sinewaveRay.IsHorizontal())
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
            }*/
        }
    }

    //TODO: Fix or update the function for handling the constraints to the movement of Sauron modules
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
    
    
    /// <summary>
    /// Applies the rotatory constraints to the movement of the endpoint of the helicoidal ray active child
    /// (and so the ray itself), which is a Sauron module, by limiting it within a circle
    /// </summary>
    /// <param name="child"> The active child of the hand controller </param>
    /// <param name="spiralwaveRay"> The spiralwave component of the object </param>
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
    


    //TODO: see if this function is really necessary, and remove it if it is not
    /// <summary>
    /// Handles the raycast interaction with the movement areas
    /// </summary>
    /// <param name="child"> The object that is going to become the new child of the hand controller object </param>
    private void HandleEndPointRaycast(GameObject child)
    {
        
        Debug.Log("Entered HandleEndPointRaycast method");
        
        // Enable the rayInteractor at the start of the method
        rayInteractor.enabled = true;
        
        float interactDistance = 1f;
        RayEndPoint rayEndPoint = child.GetComponentInChildren<RayEndPoint>();
        Vector3 direction = new Vector3(0, 0, rayEndPoint.transform.position.z + 1);
        int layerMask = LayerMask.GetMask("NetoLayerWall");
        
        
        if(Physics.SphereCast(rayEndPoint.transform.position, rayEndPoint.GetComponent<SphereCollider>().radius, direction, out RaycastHit raycastHit, interactDistance, layerMask))
        {
            Debug.Log("Raycast hit: " + raycastHit.transform.name);
            Debug.Log("Layermask name: " + raycastHit.transform.gameObject.layer);
            if(raycastHit.transform.GetComponent<MovementArea>() != null)
            {
                Debug.Log("Raycast hit a movement area");
                // The ray has hit a movement area and it can be controlled
                rayInteractor.enabled = true;
                // Save the last useful positions
                lastEndPointPosition = rayEndPoint.transform.position;
                lastInteractablePosition = child.transform.position;
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything");
            rayEndPoint.transform.position = lastEndPointPosition;
            child.transform.position = lastInteractablePosition;
            //rayInteractor.enabled = false;
        }
        
        /*lastEndPointPosition = endPoint.transform.position;
        lastInteractablePosition = child.transform.position;
        */

        
        // Draw the direction of the sphere cast
        Debug.DrawRay(rayEndPoint.transform.position, direction * interactDistance, Color.green);
        // Draw the sphere itself at the start of the cast
        //Debug.DrawWireSphere(endPoint.transform.position, endPoint.GetComponent<SphereCollider>().radius, Color.red);

    }

    #endregion
    
    

}
