using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class ConditionalHoverEvents : MonoBehaviour
{

    [SerializeField] private XRSimpleInteractable interactable;
    
    
    // Actions to be performed on hover entered and exited
    public UnityEngine.Events.UnityEvent onHoverEnteredActions;
    public UnityEngine.Events.UnityEvent onHoverExitedActions;
    
    
    void Start()
    {
        // Ensure interactable is assigned
        if (interactable == null)
        {
            interactable = GetComponent<XRSimpleInteractable>();
        }

        // Subscribe to the hover events
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        
    }
    
    
    protected virtual void OnDestroy()
    {
        // Unsubscribe from the hover events
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
    }


    protected abstract void OnHoverEntered(HoverEnterEventArgs args);
    protected abstract void OnHoverExited(HoverExitEventArgs args);

    

}
