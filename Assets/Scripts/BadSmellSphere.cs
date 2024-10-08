using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class BadSmellSphere : MonoBehaviour
{
    
    [SerializeField] InputActionReference aButtonAction;
    [SerializeField] InputActionReference xButtonAction;
    
    [SerializeField] private Transform deathtrapPortal;
    [SerializeField] [ColorUsage(true, true)] private Color inactiveBadSmellSphereInitialColor;
    [SerializeField] [ColorUsage(true, true)] private Color initialBadSmellEmissionColor;
    [SerializeField] private MeshRenderer inactiveBadSmellSphereRenderer;
    [SerializeField] private ParticleSystem badSmellParticleSystem;

    private Transform inactiveBadSmellSphere;
    private Renderer badSmellSphereRenderer;
    
    private Transform badSmellSmoke;
    private Vector3 smokeDirection;
    
    private List<ActionBasedController> xrControllers = new List<ActionBasedController>();
    private List<XRDirectInteractor> interactors = new List<XRDirectInteractor>();
    private List<Pointer> pointers = new List<Pointer>();
    
    
    private bool isInControl = false;
    private bool isSupposedToResetEmissiveIntensity = false;

    private int badSmellLedsBrightnessTest;
    
    private int badSmellEmittingTest;


    private void Start()
    {
        badSmellSphereRenderer = GetComponent<Renderer>();
        if(transform.parent != null)
        {
            inactiveBadSmellSphere = transform.parent;
        }
        
        
        badSmellSmoke = GetComponentInChildren<Transform>();
        smokeDirection = deathtrapPortal.position - transform.position;
        badSmellSmoke.rotation = Quaternion.LookRotation(smokeDirection);
    }
    
    private void Update()
    {
        badSmellSmoke.rotation = Quaternion.LookRotation(smokeDirection);

        if (isInControl)
        {
            // 1) Handle bad smell emission
            HandleBadSmellEmittingBasedOnButton();
            
            // 2) Handle brightness of the LEDs
            HandleEmissiveIntensityBasedOnTrigger();
        }
        
    }
    
    
    void OnTriggerEnter(Collider other)
    {
        XRDirectInteractor interactor = other.gameObject.GetComponent<XRDirectInteractor>();
        Pointer pointer = other.gameObject.GetComponent<Pointer>();
        ActionBasedController xrController = interactor?.GetComponentInParent<ActionBasedController>();

        Debug.Log("Interactor: " + interactor);
        
        if(interactor != null && !interactors.Contains(interactor))
        {
            interactors.Add(interactor);
            pointers.Add(pointer);
            xrControllers.Add(xrController);

            isInControl = true;
            isSupposedToResetEmissiveIntensity = false;
        }
        
        Debug.Log("INTERACTORS COUNT: " + interactors.Count);
        
        
    }

    void OnTriggerExit(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponent<XRDirectInteractor>();
        
        if (interactor != null && interactors.Contains(interactor))
        {
            int index = interactors.IndexOf(interactor);
            Debug.Log("Removing interactor at index: " + index);
            
            // Remove this interactor and corresponding elements from the lists
            interactors.RemoveAt(index);
            pointers.RemoveAt(index);
            xrControllers.RemoveAt(index);
            
            Debug.Log("Remainig interactors: " + interactors.Count);
            Debug.Log("Remaining interactors: " + interactors);
            
            // If no controllers remain in the list, disable control
            if (interactors.Count == 0)
            {
                
                isInControl = false;
                isSupposedToResetEmissiveIntensity = false;
            }
        }
    }
    
    
    
    private void HandleBadSmellEmittingBasedOnButton()
    {
        
        // Only way I could find to distinguish between the left and right hand controllers, since there are no
        // specific methods or fields in ActionBasedController to do so. I think it's acceptable in this specific
        // case, since the controllers are just two: one for the left hand and one for the right hand.
        string leftController = "LeftController";
        string rightController = "RightController";

        foreach (var xrController in xrControllers)
        {
            if((xrController.gameObject.CompareTag(leftController) && xButtonAction.action.ReadValue<float>() > 0) ||
               (xrController.gameObject.CompareTag(rightController) && aButtonAction.action.ReadValue<float>() > 0))
            {
                badSmellEmittingTest = 1;
                
                /*if(badSmellSphereRenderer != null)
                {
                    badSmellSphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, initialBadSmellEmissionColor);
                }*/
    
                if (badSmellParticleSystem != null)
                {
                    badSmellParticleSystem.Play();
                }
                
                
            }
            else if(xButtonAction.action.ReadValue<float>() == 0 && aButtonAction.action.ReadValue<float>() == 0)
            {
                badSmellEmittingTest = 0;
                
                /*if(badSmellSphereRenderer != null)
                {
                    badSmellSphereRenderer.material.SetColor(Constants.EMISSION_COLOR_ID, inactiveBadSmellSphereInitialColor);
                }*/
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
            
            float gripValue = 0;
            // Loop through each controller and get the maximum grip value
            foreach (var xrController in xrControllers)
            {
                float currentGripValue = xrController.selectActionValue.action.ReadValue<float>();
                gripValue = Mathf.Max(gripValue, currentGripValue);
            }
            
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Grip value: " + gripValue);
            
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                badSmellLedsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY_BAD_SMELL, Constants.CAPPED_MIN_EMISSION_INTENSITY_BAD_SMELL);
                Debug.Log("CAPPED EMISSIVE INTENSITY: " + cappedEmissiveIntensity);
                if (badSmellSphereRenderer != null)
                {
                    Color currentEmissionColor = badSmellSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(badSmellSphereRenderer, newEmissionColor);
                }
                if(inactiveBadSmellSphereRenderer != null)
                {
                    Color currentEmissionColor = inactiveBadSmellSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveBadSmellSphereRenderer, newEmissionColor);
                }

            }
            else
            {
                // Branch needed to reset the emissive intensity when I let the lateral trigger go (since it
                // could not detect the grip values smoothly)
                
                //if (badSmellSphereRenderer != null && badSmellEmittingTest == 0)
                if (badSmellSphereRenderer != null)
                {
                    Color newEmissionColor = initialBadSmellEmissionColor;
                
                    SetMaterialColor(badSmellSphereRenderer, newEmissionColor);
                
                }
                
                if(inactiveBadSmellSphereRenderer != null)
                {
                    Color newInactiveEmissionColor = inactiveBadSmellSphereInitialColor;
                
                    SetMaterialColor(inactiveBadSmellSphereRenderer, newInactiveEmissionColor);
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
            
            Debug.Log("GRIP VALUE TEST: " + gripValue);
            
            if (gripValue > Constants.XR_CONTROLLER_GRIP_VALUE_THRESHOLD)
            {
                Debug.Log("TRIGGER PRESSED! Grip value: " + gripValue);
            
                // Map the grip value to the capped emissive intensity range defined in the Constants class
                badSmellLedsBrightnessTest = (int)RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.DEATHTRAP_BRIGHTNESS_MAX, Constants.DEATHTRAP_BRIGHTNESS_MIN);
                float cappedEmissiveIntensity = RangeRemappingHelper.Remap(gripValue, Constants.XR_CONTROLLER_MAX_GRIP_VALUE, Constants.XR_CONTROLLER_MIN_GRIP_VALUE,
                    Constants.CAPPED_MAX_EMISSION_INTENSITY_BAD_SMELL, Constants.CAPPED_MIN_EMISSION_INTENSITY_BAD_SMELL);
                Debug.Log("CAPPED EMISSIVE INTENSITY: " + cappedEmissiveIntensity);
                if (badSmellSphereRenderer != null)
                {
                    Color currentEmissionColor = badSmellSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    //Color newEmissiveColor = new Color(currentEmissiveColor.r * cappedEmissiveIntensity, currentEmissiveColor.g * cappedEmissiveIntensity, currentEmissiveColor.b * cappedEmissiveIntensity, currentEmissiveColor.a);
                    Color newEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(badSmellSphereRenderer, newEmissionColor);
                }
                
                if(inactiveBadSmellSphereRenderer != null)
                {
                    Color currentInactiveEmissionColor = inactiveBadSmellSphereRenderer.material.GetColor(Constants.EMISSION_COLOR_ID);
                    Color newInactiveEmissionColor = GetHDRIntensity.AdjustEmissiveIntensity(currentInactiveEmissionColor, cappedEmissiveIntensity);
                
                    SetMaterialColor(inactiveBadSmellSphereRenderer, newInactiveEmissionColor);
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
    
    public int GetBadSmellEmittingTest()
    {
        return badSmellEmittingTest;
    }


    
    
}
