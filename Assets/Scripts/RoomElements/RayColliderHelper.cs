using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/*public class RayColliderHelper : MonoBehaviour
{
    
    /*public UnityEvent<RayColliderHelper> onPointerEnterRay;
    public UnityEvent<RayColliderHelper> onPointerExitRay;
    #1#
    
    /*public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;#1#
    
    
    public UnityEvent<RayColliderHelper> onPointerEnter;
    public UnityEvent<RayColliderHelper> onPointerExit;
    //public UnityEvent<RayColliderHelper> onPointerStay;
    
    public UnityEvent<LineRenderer, LineRenderer> onPointerEnterLineRendererActivation;
    public UnityEvent<LineRenderer, LineRenderer> onPointerExitLineRendererDeactivation;
    

    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Transform rayEndpoint;
    [SerializeField] private LineRenderer parentLineRenderer;
    [SerializeField] private LineRenderer childLineRenderer;
    
    private float yOffset = 0f;
    private float zOffset = 1f;
    
    private bool firstExecution = true;


    private void Start()
    {
        this.transform.position = rayOrigin.position;
        this.transform.LookAt(rayEndpoint.position);
    }
    
    private void Update()
    {
        // Calculate the direction of the ray
        Vector3 rayDirection = (rayEndpoint.position - rayOrigin.position).normalized;
        // Calculate the zOffset along the ray direction
        Vector3 zOffsetVector = zOffset * rayDirection;

        
        
        // Orient the ray collider object towards the ray endpoint, so that it is always
        // pointing along the ray direction
        //this.transform.LookAt(rayEndpoint.position);

        if (firstExecution)
        {
            this.transform.position += new Vector3(0, yOffset, 0) + zOffsetVector;
            firstExecution = false;
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.GetComponent<Pointer>() != null)
        {
            onPointerEnter?.Invoke(this);
            onPointerEnterLineRendererActivation?.Invoke(parentLineRenderer, childLineRenderer);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (other.gameObject.GetComponent<Pointer>() != null)
        {
            onPointerExit?.Invoke(this);
            onPointerExitLineRendererDeactivation?.Invoke(parentLineRenderer, childLineRenderer);
        }
    }


    /*private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
        if (other.gameObject.GetComponent<Pointer>() != null)
        {
            onPointerStay?.Invoke(this);
        }
    }#1#
}*/
