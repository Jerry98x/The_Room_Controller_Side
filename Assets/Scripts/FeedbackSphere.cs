using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class representing the main sphere of the Deathrap.
/// </summary>
public class FeedbackSphere : MonoBehaviour
{
    
    [SerializeField] InputActionReference aButtonAction;
    [SerializeField] InputActionReference xButtonAction;
    
    [SerializeField] [ColorUsage(true, true)] private Color inactiveSphereInitialColor;
    [SerializeField] [ColorUsage(true, true)] private Color initialEmissionColor;
    [SerializeField] private MeshRenderer inactiveSphereRenderer;
    [SerializeField] private ParticleSystem liquidSprayingParticleSystem;
    [SerializeField] private Renderer badSmellSphereRenderer;
    [SerializeField] [ColorUsage(true, true)] private Color badSmellLightColor;
    [SerializeField] private ParticleSystem badSmellParticleSystem;

    private Transform inactiveSphere;
    private Renderer sphereRenderer;
    
    private List<ActionBasedController> xrControllers = new List<ActionBasedController>();
    private List<XRDirectInteractor> interactors = new List<XRDirectInteractor>();
    private List<Pointer> pointers = new List<Pointer>();


    private bool isInControl = false;
    private bool isFirstFrame = true;
    private bool isSupposedToResetEmissiveIntensity = false;
    private bool isSupposedToResetPetalsOpening = false;
    
    
    private int liquidSprayingTest;
    private int petalsOpeningTest;
    private int badSmellEmittingTest;
    private int ledsBrightnessTest;


    private void Start()
    {
        sphereRenderer = GetComponent<Renderer>();
        if(transform.parent != null)
        {
            inactiveSphere = transform.parent;
        }
    }


    private void Update()
    {
        if (isInControl)
        {
            // In relation to the physical Deathtrap module, the following operations are handled to have a feedback in the application:
            
            // 1) Handle spraying
            HandleSprayingBasedOnButton();
            
            // 2) Handle opening/closing of the Deathtrap petals
            HandlePetalsOpeningBasedOnTrigger();
            
            // 3) Handle bad smell emission
            //HandleBadSmellEmittingBasedOnButton();
            
            // 4) Handle brightness of the LEDs
            HandleEmissiveIntensityBasedOnTrigger();
        }
        
    }


    /// <summary>
    /// Method that is called when the sphere's collider is entered by a hand controller.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        XRDirectInteractor interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        Pointer pointer = other.gameObject.GetComponent<Pointer>();
        ActionBasedController xrController = interactor?.GetComponentInParent<ActionBasedController>();
        
        if(interactor != null && !interactors.Contains(interactor))
        {
            interactors.Add(interactor);
            pointers.Add(pointer);
            xrControllers.Add(xrController);

            isInControl = true;
            isSupposedToResetEmissiveIntensity = false;
            isSupposedToResetPetalsOpening = false;
        }
        
    }

    /// <summary>
    /// Method that is called when the sphere's collider is exited by a hand controller.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponent<XRDirectInteractor>();
        
        if (interactor != null && interactors.Contains(interactor))
        {
            int index = interactors.IndexOf(interactor);
            
            // Remove this interactor and corresponding elements from the lists
            interactors.RemoveAt(index);
            pointers.RemoveAt(index);
            xrControllers.RemoveAt(index);
            
            // If no controllers remain in the list, disable control
            if (interactors.Count == 0)
            {
                isInControl = false;
                isSupposedToResetEmissiveIntensity = false;
                isSupposedToResetPetalsOpening = false;
            }
        }
    }


    /// <summary>
    /// Handles the spraying of the liquid action, including the corner cases.
    /// </summary>
    private void HandleSprayingBasedOnButton()
    {
        // Only way I could find to distinguish between the left and right hand controllers, since there are no
        // specific methods or fields in ActionBasedController to do so. I think it's acceptable in this specific
        // case, since the controllers are just two: one for the left hand and one for the right hand.
        string leftController = "LeftController";
        string rightController = "RightController";

        if (xrControllers.Count == 1)
        {
            // Check if the X or A button is being pressed while the corresponding controller is inside the collider
            foreach (var xrController in xrControllers)
            {
                if((xrController.gameObject.CompareTag(leftController) && xButtonAction.action.ReadValue<float>() > 0) ||
                   (xrController.gameObject.CompareTag(rightController) && aButtonAction.action.ReadValue<float>() > 0))
                {
                    if (isFirstFrame)
                    {
                        // The physical motor for spraying must be activated only once when the button is pressed,
                        // ignoring if it's being held down
                        liquidSprayingTest = 1;
                                
                        if(liquidSprayingParticleSystem != null)
                        {
                            liquidSprayingParticleSystem.Play();
                        }
                    
                        isFirstFrame = false;
                    }
                    else
                    {
                        liquidSprayingTest = 0;
                    }
                }
                else if((xrController.gameObject.CompareTag(leftController) && xButtonAction.action.ReadValue<float>() == 0) || 
                        (xrController.gameObject.CompareTag(rightController) && aButtonAction.action.ReadValue<float>() == 0))
                {
                    liquidSprayingTest = 0;
            
                    // Reset the boolean to true so that the next time the button is pressed, the motor is activated
                    isFirstFrame = true;
                }
            }
        }
        else if(xrControllers.Count == 2)
        {
            // Check if the X or A button is being pressed while the corresponding controller is inside the collider
            foreach (var xrController in xrControllers)
            {
                if ((xrController.gameObject.CompareTag(leftController) && xButtonAction.action.ReadValue<float>() > 0 && aButtonAction.action.ReadValue<float>() == 0) ||
                    (xrController.gameObject.CompareTag(rightController) && aButtonAction.action.ReadValue<float>() > 0 && xButtonAction.action.ReadValue<float>() == 0))
                {
                    if (isFirstFrame)
                    {
                        // The physical motor for spraying must be activated only once when the button is pressed,
                        // ignoring if it's being held down
                        liquidSprayingTest = 1;
                                
                        if(liquidSprayingParticleSystem != null)
                        {
                            liquidSprayingParticleSystem.Play();
                        }
                    
                        isFirstFrame = false;
                    }
                    else
                    {
                        liquidSprayingTest = 0;
                    }
                }
                else if((xrController.gameObject.CompareTag(leftController) && xButtonAction.action.ReadValue<float>() == 0 && aButtonAction.action.ReadValue<float>() == 0) || 
                        (xrController.gameObject.CompareTag(rightController) && aButtonAction.action.ReadValue<float>() == 0 && xButtonAction.action.ReadValue<float>() == 0))
                {
                    liquidSprayingTest = 0;
            
                    // Reset the boolean to true so that the next time the button is pressed, the motor is activated
                    isFirstFrame = true;
                }
            }
        }
        
        
    }
    
    
    /// <summary>
    /// Handles the opening of the Deathtrap petals action, which corresponds to a scaling of the sphere.
    /// </summary>
    private void HandlePetalsOpeningBasedOnTrigger()
    {
        
        float triggerValue = 0;
        // Loop through each controller and get the maximum trigger value
        foreach (var xrController in xrControllers)
        {
            float currentTriggerValue = xrController.activateActionValue.action.ReadValue<float>();
            triggerValue = Mathf.Max(triggerValue, currentTriggerValue);
        }
        
        
        if(triggerValue > 0)
        {
            petalsOpeningTest = (int)RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                Constants.DEATHTRAP_PETALS_OPENING_MAX, Constants.DEATHTRAP_PETALS_OPENING_MIN);
            float newScale = RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                Constants.DEATHTRAP_CORE_MAX_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE);
            
            // Increase the size of the father object. This object, which is its child, will be scaled accordingly
            inactiveSphere.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            transform.localScale = new Vector3(Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE);
            inactiveSphere.transform.localScale = new Vector3(Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE);
        }

    }


    /// <summary>
    /// Handles the main sphere light emission action.
    /// </summary>
    private void HandleEmissiveIntensityBasedOnTrigger()
    {

        if (isSupposedToResetEmissiveIntensity)
        {
            // Case in which I end up as soon as I press the lateral trigger for the first time after gaining
            // control of the Deathtrap. All the following times I press the lateral trigger, I will stay here.
            // The boolean gets reset to false when I loose control of the Deathtrap.
            
            float gripValue = 0;
            // Loop through each controller and get the maximum grip value
            foreach (var xrController in xrControllers)
            {
                float currentGripValue = xrController.selectActionValue.action.ReadValue<float>();
                gripValue = Mathf.Max(gripValue, currentGripValue);
            }
            
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                ledsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                
                if (sphereRenderer != null)
                {
                    Color currentEmissionColor = sphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(sphereRenderer, newEmissionColor);
                }
                if(inactiveSphereRenderer != null)
                {
                    Color currentInactiveEmissionColor = inactiveSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    Color newInactiveEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentInactiveEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveSphereRenderer, newInactiveEmissionColor);
                }

            }
            else
            {
                // Branch needed to reset the emissive intensity when I let the lateral trigger go (since it
                // could not detect the grip values smoothly)
                
                if (sphereRenderer != null && liquidSprayingTest == 0)
                {
                    Color newEmissionColor = initialEmissionColor;
                
                    SetMaterialColor(sphereRenderer, newEmissionColor);
                
                }
                
                if(inactiveSphereRenderer != null  && liquidSprayingTest == 0)
                {
                    Color newInactiveEmissionColor = inactiveSphereInitialColor;
                
                    SetMaterialColor(inactiveSphereRenderer, newInactiveEmissionColor);
                }
                
                
            }
        }
        else
        {
            // Case in which I end up as soon as I gain control of the Deathtrap. I will stay here until I press the
            // lateral trigger. I want a possible emissivity different from the initial one to be maintained when I lose
            // control and then I regain it, because the physical Deathtrap need to illuminate the Room.
            // The ELSE branch of the inner IF statement placed in the IF branch of the outer if statement prevents
            // this from happening, but is needed to properly reset the emissive intensity when I let the lateral
            // trigger go (since it could not detect the grip values smoothly).
            
            float gripValue = 0;
            // Loop through each controller and get the maximum grip value
            foreach (var xrController in xrControllers)
            {
                float currentGripValue = xrController.selectActionValue.action.ReadValue<float>();
                gripValue = Mathf.Max(gripValue, currentGripValue);
            }
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                ledsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                
                if (sphereRenderer != null)
                {
                    Color currentEmissionColor = sphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(sphereRenderer, newEmissionColor);
                }
                if(inactiveSphereRenderer != null)
                {
                    Color currentInactiveEmissionColor = inactiveSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    Color newInactiveEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentInactiveEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveSphereRenderer, newInactiveEmissionColor);
                }

                // Set the boolean to true so that as soon as I press the lateral trigger, I end up in the other
                // branch of the if statement
                isSupposedToResetEmissiveIntensity = true;
            }
            
        }
        
        
    }
    
    
    private void SetMaterialColor(Renderer rend, Color newEmissionColor)
    {
        rend.material.SetColor(Constants.EMISSION_COLOR_ID, newEmissionColor);
    }
    
    


    public bool IsInControl()
    {
        return isInControl;
    }
    
    public List<ActionBasedController> GetControllers()
    {
        return xrControllers;
    }
    
    public int GetLiquidSprayingTest()
    {
        return liquidSprayingTest;
    }
    
    public int GetPetalsOpeningTest()
    {
        return petalsOpeningTest;
    }
    
    public int GetBadSmellEmittingTest()
    {
        return badSmellEmittingTest;
    }
    
    public int GetLedsBrightnessTest()
    {
        return ledsBrightnessTest;
    }


}
