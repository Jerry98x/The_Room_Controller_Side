using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class ConditionalNetoHoverEvents : ConditionalHoverEvents
{
    [SerializeField] private HandleNetoRayMovement handleNetoRayMovement;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        List<ActionBasedController> controllers = handleNetoRayMovement.GetControllers();
        
        if (!handleNetoRayMovement.HasEmergency())
        {
            if (controllers.Count == 1)
            {
                // Execute actions if the condition is true
                onHoverEnteredActions.Invoke();
            }
            
        }
    }

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