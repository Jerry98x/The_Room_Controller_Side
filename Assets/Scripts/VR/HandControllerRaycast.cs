using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandControllerRaycast : MonoBehaviour
{
    private GameObject activeChild; // Reference to the current active child object
    public float maxRaycastDistance = 100f; // You can adjust this value as needed

    private void Update()
    {
        HandleRaycasting();
    }
    
    private void HandleRaycasting()
    {
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxRaycastDistance))
        {
            if (raycastHit.transform.TryGetComponent(out EndPoint endPoint))
            {
                // The ray has hit an endPoint
                if(transform.TryGetComponent(out XRController handController))
                {
                    // If there is a current active child, remove it from the parent
                    if (activeChild != null)
                    {
                        activeChild.transform.parent = null;
                    }
                    
                    // Set the hit object as the new active child
                    SetActiveChild(raycastHit.transform.parent.gameObject);

                }
            }
        }
    }
    
    
    private void SetActiveChild(GameObject newChild)
    {
        // If there is a current active child, remove it from the parent
        if (activeChild != null)
        {
            activeChild.transform.parent = null;
        }

        // Set the new object as the active child
        activeChild = newChild;
        activeChild.transform.SetParent(transform);
    }
}
