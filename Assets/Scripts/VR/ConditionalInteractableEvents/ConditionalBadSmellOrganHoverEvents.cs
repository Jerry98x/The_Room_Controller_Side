using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Wrapper for the hover events that adds additional conditions to them. Used for the Deathtrap bad smell sphere.
/// </summary>
public class ConditionalBadSmellOrganHoverEvents : ConditionalHoverEvents
{
    
    [SerializeField] private BadSmellSphere badSmellSphere;
    
    /// <summary>
    /// Additional condition for the hover entered event: the bad smell sphere must have at least one controller.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        List<ActionBasedController> controllers = badSmellSphere.GetControllers();
        if (controllers.Count >= 1)
        {
            // Execute actions if the condition is true
            onHoverEnteredActions.Invoke();
        }
    }
    
    
    /// <summary>
    /// Additional condition for the hover exited event: the bad smell sphere must have no controllers.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        List<ActionBasedController> controllers = badSmellSphere.GetControllers();
        if (controllers.Count == 0)
        {
            // Execute actions if the condition is true
            onHoverExitedActions.Invoke();
        }
    }
    
}
