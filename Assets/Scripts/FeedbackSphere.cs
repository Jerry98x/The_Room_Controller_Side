using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class FeedbackSphere : MonoBehaviour
{
    
    [SerializeField] InputActionReference aButtonAction;
    [SerializeField] InputActionReference bButtonAction;
    [SerializeField] InputActionReference xButtonAction;
    [SerializeField] InputActionReference yButtonAction;
    
    [SerializeField] [ColorUsage(true, true)] private Color initialEmissionColor;
    [SerializeField] [ColorUsage(true, true)] private Color testingColor1;
    [SerializeField] [ColorUsage(true, true)] private Color testingColor2;

    private Renderer sphereRenderer;
    
    private ActionBasedController xrController;
    private XRDirectInteractor interactor;
    private Pointer pointer;


    private bool isInControl = false;
    private bool isSupposedToResetEmissiveIntensity = false;
    private bool isSupposedToResetPetalsOpening = false;
    
    
    private int liquidSprayingTest;
    private int petalsOpeningTest;
    private int badSmellEmittingTest;
    private int ledsBrightnessTest;


    private void Start()
    {
        sphereRenderer = GetComponent<Renderer>();
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
            HandleBasSmellEmittingBasedOnButton();
            
            // 4) Handle brightness of the LEDs
            HandleEmissiveIntensityBasedOnTrigger();
        }


        Debug.Log("VALORI DEATHTRAP: liquidSprayingTest: " + liquidSprayingTest);
        Debug.Log("VALORI DEATHTRAP: petalsOpeningTest: " + petalsOpeningTest);
        Debug.Log("VALORI DEATHTRAP: badSmellEmittingTest: " + badSmellEmittingTest);
        Debug.Log("VALORI DEATHTRAP: ledsBrightnessTest: " + ledsBrightnessTest);
    }


    void OnTriggerEnter(Collider other)
    {
        if(interactor != null || pointer != null)
        {
            Debug.Log("INTERACTOR NOT NULL: " + interactor);
            return;
        }

        interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        pointer = other.gameObject.GetComponent<Pointer>();
        xrController = interactor.GetComponentInParent<ActionBasedController>();
        
        if (interactor != null || pointer != null)
        {
            isInControl = true;
            isSupposedToResetEmissiveIntensity = false;
            isSupposedToResetPetalsOpening = false;
        }
        
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<XRDirectInteractor>() == interactor)
        {
            pointer = null;
            interactor = null;
            isInControl = false;
            isSupposedToResetEmissiveIntensity = false;
            isSupposedToResetPetalsOpening = false;
        }
    }



    private void HandleSprayingBasedOnButton()
    {

        if((xButtonAction.action.ReadValue<float>() > 0 || aButtonAction.action.ReadValue<float>() > 0) &&
           !(yButtonAction.action.ReadValue<float>() > 0 || bButtonAction.action.ReadValue<float>() > 0))
        {
            Debug.Log("BUTTON X OR A PRESSED!");
            
            liquidSprayingTest = 1;
            
            if(sphereRenderer != null)
            {
                Debug.Log("SETTING COLOR TO TESTING COLOR 1");
                sphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, testingColor1);
            }
            
        }
        else if((xButtonAction.action.ReadValue<float>() == 0 && aButtonAction.action.ReadValue<float>() == 0) &&
                !(yButtonAction.action.ReadValue<float>() > 0 || bButtonAction.action.ReadValue<float>() > 0))
        {
            liquidSprayingTest = 0;
            
            if(sphereRenderer != null)
            {
                sphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, initialEmissionColor);
            }
        }
        
        /*if(xrController.gameObject.tag.Equals("LeftController"))
        {
            float buttonValue = xButtonAction.action.ReadValue<float>();
            if (buttonValue > 0)
            {
                Debug.Log("BUTTON X PRESSED!");
            }
        }
        else if(xrController.gameObject.tag.Equals("RightController"))
        {
            float buttonValue = aButtonAction.action.ReadValue<float>();
            if (buttonValue > 0)
            {
                Debug.Log("BUTTON A PRESSED!");
            }
        }*/
        
        
    }
    
    
    private void HandlePetalsOpeningBasedOnTrigger()
    {
        float triggerValue = xrController.activateActionValue.action.ReadValue<float>();
        if (triggerValue > 0)
        {
            Debug.Log("TRIGGER PRESSED! Trigger value: " + triggerValue);
        }
        
        
        if(triggerValue > 0)
        {
            Debug.Log("TRIGGER PRESSED! Trigger value: " + triggerValue);
            
            petalsOpeningTest = (int)RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                Constants.DEATHTRAP_PETALS_OPENING_MAX, Constants.DEATHTRAP_PETALS_OPENING_MIN);
            Debug.Log("TRIGGER PRESSED! Petals opening: " + petalsOpeningTest);
            float newScale = RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                Constants.DEATHTRAP_CORE_MAX_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE);
            Debug.Log("TRIGGER PRESSED! Final scale: " + newScale);
            /*float lerpSpeed = 1.0f;
            float newScale = Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime * lerpSpeed);*/
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            transform.localScale = new Vector3(Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE, Constants.DEATHTRAP_CORE_MIN_SIZE);
        }

        /*
        if (isSupposedToResetPetalsOpening)
        {
            if (triggerValue > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Trigger value: " + triggerValue);
                
                petalsOpeningTest = (int)RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                    Constants.DEATHTRAP_PETALS_OPENING_MAX, Constants.DEATHTRAP_PETALS_OPENING_MIN);
                float targetScale = RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                    maxTestingScale, minTestingScale);
                
                float lerpSpeed = 1.0f;
                float newScale = Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime * lerpSpeed);
                transform.localScale = new Vector3(newScale, newScale, newScale);
                
            }
            else
            {
                transform.localScale = new Vector3(minTestingScale, minTestingScale, minTestingScale);
            }
        }
        else
        {
            if (triggerValue > Constants.XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Trigger value: " + triggerValue);
                
                petalsOpeningTest = (int)RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                    Constants.DEATHTRAP_PETALS_OPENING_MAX, Constants.DEATHTRAP_PETALS_OPENING_MIN);
                float newScale = RangeRemappingHelper.Remap(triggerValue, Constants.XR_CONTROLLER_MAX_TRIGGER_VALUE, Constants.XR_CONTROLLER_MIN_TRIGGER_VALUE, 
                    maxTestingScale, minTestingScale);
                transform.localScale = new Vector3(newScale, newScale, newScale);
                
            }
            isSupposedToResetPetalsOpening = true;
        }
        */
        

    }
    
    
    
    
    private void HandleBasSmellEmittingBasedOnButton()
    {
        
        if((yButtonAction.action.ReadValue<float>() > 0 || bButtonAction.action.ReadValue<float>() > 0) &&
           !(xButtonAction.action.ReadValue<float>() > 0 || aButtonAction.action.ReadValue<float>() > 0))
        {
            Debug.Log("BUTTON Y OR B PRESSED!");
            
            badSmellEmittingTest = 1;
            
            if(sphereRenderer != null)
            {
                sphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, testingColor2);
            }

            
        }
        else if((yButtonAction.action.ReadValue<float>() == 0 && bButtonAction.action.ReadValue<float>() == 0) &&
                !(xButtonAction.action.ReadValue<float>() > 0 || aButtonAction.action.ReadValue<float>() > 0))
        {
            badSmellEmittingTest = 0;
            
            if(sphereRenderer != null)
            {
                sphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, initialEmissionColor);
            }
        }
        
    }



    private void HandleEmissiveIntensityBasedOnTrigger()
    {

        if (isSupposedToResetEmissiveIntensity)
        {
            // Case in which I end up as soon as I press the lateral trigger for the first time after gaining
            // control of the Deathtrap. All the following times I press the lateral trigger, I will stay here.
            // The boolean gets reset to false when I loose control of the Deathtrap.
            
            float gripValue = xrController.selectActionValue.action.ReadValue<float>();
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Grip value: " + gripValue);
            
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                ledsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                Debug.Log("CAPPED EMISSIVE INTENSITY: " + cappedEmissiveIntensity);
                if (sphereRenderer != null)
                {
                    Color currentEmissionColor = sphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(newEmissionColor);
                }

            }
            else
            {
                // Branch needed to reset the emissive intensity when I let the lateral trigger go (since it
                // could not detect the grip values smoothly)
                
                if (sphereRenderer != null && liquidSprayingTest == 0 && badSmellEmittingTest == 0)
                {
                    Color newEmissionColor = initialEmissionColor;
                
                    SetMaterialColor(newEmissionColor);
                
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
            
            float gripValue = xrController.selectActionValue.action.ReadValue<float>();
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Grip value: " + gripValue);
            
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                ledsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY, Constants.CAPPED_MIN_EMISSION_INTENSITY);
                Debug.Log("CAPPED EMISSIVE INTENSITY: " + cappedEmissiveIntensity);
                if (sphereRenderer != null)
                {
                    Color currentEmissionColor = sphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(newEmissionColor);
                }

                // Set the boolean to true so that as soon as I press the lateral trigger, I end up in the other
                // branch of the if statement
                isSupposedToResetEmissiveIntensity = true;
            }
            
        }
        
        
    }
    
    
    private void SetMaterialColor(Color newEmissionColor)
    {
        sphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, newEmissionColor);
    }
    
    
    
    
    

    
    
    
    
    
    
    




    public bool IsInControl()
    {
        return isInControl;
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