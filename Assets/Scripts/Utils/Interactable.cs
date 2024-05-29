using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a generic interactable object in the context of the XR Ray Interactor component
/// </summary>
public class Interactable : MonoBehaviour
{
    
    // Maybe use events
    
    public void OnSelect()
    {
        // Code to run when this object is selected
    }

    public void OnDeselect()
    {
        // Code to run when this object is deselected
    }

    public void OnInteract()
    {
        // Code to run when this object is interacted with
    }
    
}
