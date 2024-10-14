using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConditionalDeathtrapHoverEvents : ConditionalHoverEvents
{
    
    [SerializeField] private FeedbackSphere feedbackSphere;
    
    
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        List<ActionBasedController> controllers = feedbackSphere.GetControllers();
        if (controllers.Count >= 1)
        {
            // Execute actions if the condition is true
            onHoverEnteredActions.Invoke();
        }
    }
    
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        List<ActionBasedController> controllers = feedbackSphere.GetControllers();
        if (controllers.Count == 0)
        {
            // Execute actions if the condition is true
            onHoverExitedActions.Invoke();
        }
    }
    
}
