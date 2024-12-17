using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TESTING

/// <summary>
/// Class that handles the actions and animation of the VR hand model
/// </summary>
/// <remarks>
/// Dummy class used to learn VR
/// </remarks>
public class AnimateHandOnInput : MonoBehaviour
{

    [SerializeField] private InputActionProperty pinchAnimationAction;
    [SerializeField] private InputActionProperty gripAnimationAction;
    [SerializeField] private Animator handAnimator;

    private float triggerValue = 0f;
    private float gripValue = 0f;

    /// <summary>
    /// Updates the values of the hand's actions at each frame
    /// </summary>
    private void Update()
    {
        triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        
        gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
