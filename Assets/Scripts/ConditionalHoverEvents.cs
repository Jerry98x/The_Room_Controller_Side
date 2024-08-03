using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class ConditionalHoverEvents : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private HandleNetoRayMovement handleNetoRayMovement;

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

    void OnDestroy()
    {
        // Unsubscribe from the hover events
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (!handleNetoRayMovement.HasEmergency())
        {
            // Execute actions if the condition is true
            onHoverEnteredActions.Invoke();
        }
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (!handleNetoRayMovement.HasEmergency())
        {
            // Execute actions if the condition is true
            onHoverExitedActions.Invoke();
        }
    }
}