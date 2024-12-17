using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Wrapper for the hover events that adds additional conditions to them. Used for the Neto ray.
/// </summary>
public class ConditionalNetoHoverEvents : ConditionalHoverEvents
{
    [SerializeField] private HandleNetoRayMovement handleNetoRayMovement;

    /// <summary>
    /// Additional condition for the hover entered event: the Neto ray must have at least one controller.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        List<ActionBasedController> controllers = handleNetoRayMovement.GetControllers();
        
        if (!handleNetoRayMovement.HasEmergency())
        {
            if (controllers.Count >= 1)
            {
                // Execute actions if the condition is true
                onHoverEnteredActions.Invoke();
            }
            
        }
    }

    /// <summary>
    /// Additional condition for the hover exited event: the Neto ray must have no controllers.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        List<ActionBasedController> controllers = handleNetoRayMovement.GetControllers();
        
        if (!handleNetoRayMovement.HasEmergency())
        {
            if (controllers.Count == 0)
            {
                // Execute actions if the condition is true
                onHoverExitedActions.Invoke();
            }
        }
    }
}