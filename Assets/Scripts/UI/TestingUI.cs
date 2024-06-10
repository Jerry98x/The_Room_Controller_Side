using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class TestingUI : MonoBehaviour
{
    [SerializeField] private List<ObjectSO> objects;
    [SerializeField] private TMP_Dropdown objectDropdown;
    [SerializeField] private TMP_Dropdown intensityDropdown;
    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TextMeshProUGUI objectTextBox;
    [SerializeField] private TextMeshProUGUI intensityTextBox;
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private TextMeshProUGUI intensityValueTextBox;
    [SerializeField] private TextMeshProUGUI durationTextBox;
    [SerializeField] private Slider durationSlider;
    [SerializeField] private TextMeshProUGUI durationValueTextBox;
    
    private Canvas uiCanvas;
    
    private ObjectSO selectedObject;
    
    DropdownHandler intensityDropdownHandler;
    DropdownHandler durationDropdownHandler;

    
    [Serializable]
    public struct Effect {
        public string name;
        public GameObject effectObject;
    }

    public Effect[] effects;

    private Dictionary<string, GameObject> effectsDictionary;
    
    
    // Dictionaries to store the slider values for each option
    private Dictionary<string, float> intensityValues = new Dictionary<string, float>();
    private Dictionary<string, float> durationValues = new Dictionary<string, float>();
    

    private void Start()
    {

        // Set the testing UI to be disabled at the start
        uiCanvas = gameObject.GetComponent<Canvas>();
        uiCanvas.enabled = false;
        
        // Turn struct array into dictionary
        effectsDictionary = new Dictionary<string, GameObject>();
        foreach (var effect in effects)
        {
            effectsDictionary[effect.name] = effect.effectObject;
        }
        
        intensityDropdownHandler = intensityDropdown.GetComponent<DropdownHandler>();
        durationDropdownHandler = durationDropdown.GetComponent<DropdownHandler>();
        
        
        foreach(var obj in objects)
        {
            foreach(var characteristic in obj.testIntensityCharacteristics)
            {
                characteristic.value = characteristic.initialValue;
            }
        }
        
        // Clear the dictionaries before populating them to avoid values from previous executions
        intensityValues.Clear();
        durationValues.Clear();
        
        
        
        // Populate the dictionaries with the initial values from the scriptable objects
        foreach (var obj in objects)
        {
            foreach (var characteristic in obj.testIntensityCharacteristics)
            {
                intensityValues[characteristic.characteristicName] = characteristic.value;
            }
            foreach (var characteristic in obj.testDurationCharacteristics)
            {
                durationValues[characteristic.characteristicName] = characteristic.value;
            }
        }
        
        foreach (var val in intensityValues)
        {
            Debug.Log(val.Key + ": " + val.Value);
        }
        foreach (var val in durationValues)
        {
            Debug.Log(val.Key + ": " + val.Value);
        }

        /*foreach (var obj in intensityValues)
        {
            Debug.Log(obj.Key + ": " + obj.Value);
        }
        foreach (var obj in durationValues)
        {
            Debug.Log(obj.Key + ": " + obj.Value);
        }*/
        
        
        // Initialize selectedObject
        if (objects.Count > 0)
        {
            selectedObject = objects[0];
        }
        
        
        // Initialize the dropdown menus
        objectDropdown.options.Clear();
        foreach (var obj in objects)
        {
            objectDropdown.options.Add(new TMP_Dropdown.OptionData(obj.objectName));
        }
        UpdateDropdownMenus();
        
        
        
        //PopulateDropdown();
        
        //DropdownItemSelected(objectDropdown);
        
        //objectDropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(objectDropdown); });
        
        objectDropdown.onValueChanged.AddListener(OnObjectSelected);
        intensityDropdown.onValueChanged.AddListener(OnIntensityOptionSelected);
        //intensityDropdown.on
        durationDropdown.onValueChanged.AddListener(OnDurationOptionSelected);
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);
        durationSlider.onValueChanged.AddListener(OnDurationChanged);
        
        
        /*
        // Reset the sliders' values based on the selected options in the dropdowns
        OnIntensityOptionSelected(intensityDropdown.value);
        OnDurationOptionSelected(durationDropdown.value);
        */

        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uiCanvas.enabled = !uiCanvas.enabled;
        }
    }
    
    
    
    private void DropdownItemSelected(TMP_Dropdown tmpDropdown)
    {
        int index = tmpDropdown.value;
        objectTextBox.text = tmpDropdown.options[index].text;
    }

    /*private void PopulateDropdown()
    {
        objectDropdown.options.Clear();
        foreach (var obj in objects)
        {
            objectDropdown.options.Add(new TMP_Dropdown.OptionData(obj.objectName));
        }
    }*/
    
    
    
    private void OnObjectSelected(int index)
    {
        selectedObject = objects[index];
        UpdateDropdownMenus();
        //UpdateSliders();
    }
    
    
    
    private void UpdateDropdownMenus()
    {
        // Update the intensity dropdown menu
        intensityDropdown.options.Clear();
        foreach (var characteristic in selectedObject.testIntensityCharacteristics)
        {
            intensityDropdown.options.Add(new TMP_Dropdown.OptionData(characteristic.characteristicName));
        }

        if (intensityDropdown.options.Count > 0)
        {
           intensityDropdown.value = 0;
           intensityDropdown.captionText.text = intensityDropdown.options[0].text;
        }
        else
        {
            //TODO: do the empty object check in another way; value cannot be less than 0
            intensityDropdown.value = -1;
            intensityDropdown.captionText.text = "";
            intensityDropdownHandler.ClearTextBox();
            
        }
        
        // Set initial option visible on dropdown
        OnIntensityOptionSelected(0);
        
        // Update the duration dropdown menu
        durationDropdown.options.Clear();
        foreach (var characteristic in selectedObject.testDurationCharacteristics)
        {
            durationDropdown.options.Add(new TMP_Dropdown.OptionData(characteristic.characteristicName));
        }
        
        if(durationDropdown.options.Count > 0)
        {
            durationDropdown.value = 0;
            durationDropdown.captionText.text = durationDropdown.options[0].text;
        }
        else
        {
            durationDropdown.value = -1;
            durationDropdown.captionText.text = "";
            durationDropdownHandler.ClearTextBox();
        }
        
        // Set initial option visible on dropdown
        OnDurationOptionSelected(0);
    }
    
    
    
    private void OnIntensityOptionSelected(int index)
    {
        
        if(index < 0 || index >= selectedObject.testIntensityCharacteristics.Count)
        {
            return;
        }
        
        // Unsubscribe OnIntensityChanged from the onValueChanged event
        intensitySlider.onValueChanged.RemoveListener(OnIntensityChanged);

        
        // Get the selected option
        string option = selectedObject.testIntensityCharacteristics[index].characteristicName;

        Debug.Log("OnIntensityOptionSelected entered");
        Debug.Log("Index: " + index);
        Debug.Log("Option: " + option);
        
        // Check if a value for this option is stored in the dictionary
        if (intensityValues.ContainsKey(option))
        {
            Debug.Log("Retrieving value from dictionary:");
            Debug.Log("Value: " + intensityValues[option]);
            // Set the slider's value to the stored value
            intensitySlider.minValue = selectedObject.testIntensityCharacteristics[index].lowerBound;
            intensitySlider.maxValue = selectedObject.testIntensityCharacteristics[index].upperBound;
            intensitySlider.value = intensityValues[option];
            
            intensityValueTextBox.text = intensitySlider.value.ToString("F2");
        }
        
        
        // Re-subscribe OnIntensityChanged to the onValueChanged event
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);
        
        /*else
        {
            // Set the slider's value to the initial value from the scriptable object
            intensitySlider.value = selectedObject.testIntensityCharacteristics[index].multiplier;

            // Store the initial value in the dictionary
            intensityValues[option] = intensitySlider.value;
        }*/
        

    }
    
    private void OnDurationOptionSelected(int index)
    {
        
        if(index < 0 || index >= selectedObject.testDurationCharacteristics.Count)
        {
            return;
        }
        
        
        // Unsubscribe OnDurationChanged from the onValueChanged event
        durationSlider.onValueChanged.RemoveListener(OnDurationChanged);
        
        
        // Get the selected option
        string option = selectedObject.testDurationCharacteristics[index].characteristicName;
        
        if(durationValues.ContainsKey(option))
        {
            durationSlider.minValue = selectedObject.testDurationCharacteristics[index].lowerBound;
            durationSlider.maxValue = selectedObject.testDurationCharacteristics[index].upperBound;
            durationSlider.value = durationValues[option];
            
            durationValueTextBox.text = durationSlider.value.ToString("F2");
        }
        
        
        // Re-subscribe OnDurationChanged to the onValueChanged event
        durationSlider.onValueChanged.AddListener(OnDurationChanged);
        
    }
    
    

    private void OnIntensityChanged(float value)
    {
        Debug.Log("OnIntensityChanged entered");
        Debug.Log("Value from input parameter: " + value);
        // Update the intensity characteristic of the selected object
        selectedObject.testIntensityCharacteristics[intensityDropdown.value].value = value;

        // Update the intensity value text box
        intensityValueTextBox.text = value.ToString("F2");

        
        Debug.Log("Value before update: " + intensityValues[selectedObject.testIntensityCharacteristics[intensityDropdown.value].characteristicName]);
        
        // Store the new value in the dictionary
        intensityValues[selectedObject.testIntensityCharacteristics[intensityDropdown.value].characteristicName] = value;

        Debug.Log("Value after update: " + intensityValues[selectedObject.testIntensityCharacteristics[intensityDropdown.value].characteristicName]);
        
        
        ApplyVisualEffectChanges(selectedObject.testIntensityCharacteristics[intensityDropdown.value].characteristicName, value);
    }

    private void OnDurationChanged(float value)
    {
        // Update the duration characteristic of the selected object
        selectedObject.testDurationCharacteristics[durationDropdown.value].value = value;
    
        // Update the duration value text box
        durationValueTextBox.text = value.ToString("F2");

        // Store the new value in the dictionary
        durationValues[selectedObject.testDurationCharacteristics[durationDropdown.value].characteristicName] = value;
        
        
        // Apply the changes to the visual effects
        ApplyVisualEffectChanges(selectedObject.testDurationCharacteristics[durationDropdown.value].characteristicName, value);

    }



    private void ApplyVisualEffectChanges(string characteristicName, float value)
    {

        if (effectsDictionary.ContainsKey(characteristicName))
        {
            GameObject effect = effectsDictionary[characteristicName];
            
            // Check which characteristic is selected and apply the changes to the effect
            switch (characteristicName)
            {
                case Constants.NETO_AUDIO_INTENSITY_BY_ADDITIONAL_NOISE:
                    break;
                case Constants.NETO_AUDIO_INTENSITY_BY_PITCH:
                    AudioSource audioSourceByPitch = effect.GetComponent<AudioSource>();

                    if (audioSourceByPitch != null)
                    {
                        if (audioSourceByPitch.isPlaying)
                        {
                            var pitch = audioSourceByPitch.pitch;
                            audioSourceByPitch.pitch = value;
                        }
                        else
                        {
                            Debug.LogError("No AudioSource component found in the object");
                        }
                    }
                    break;
                case Constants.NETO_AUDIO_INTENSITY_BY_VOLUME:
                    AudioSource audioSourceByVolume = effect.GetComponent<AudioSource>();

                    if (audioSourceByVolume != null)
                    {
                        if (audioSourceByVolume.isPlaying)
                        {
                            audioSourceByVolume.volume = value;
                            audioSourceByVolume.outputAudioMixerGroup.audioMixer.SetFloat("volume", 10f);
                            
                        }
                        else
                        {
                            Debug.LogError("No AudioSource component found in the object");
                        }
                    }
                    break;
                case Constants.NETO_DURATION_BY_CIRCLES_NUMBER:
                    
                    ParticleSystem partSystemByCircleNumbersV1 = effect.GetComponent<ParticleSystem>();
                    
                    if(partSystemByCircleNumbersV1 != null)
                    {
                        if(partSystemByCircleNumbersV1.isPlaying)
                        {
                            var emission = partSystemByCircleNumbersV1.emission;
                            emission.rateOverTime = value;
                        }
                    }
                    else
                    {
                        Debug.LogError("No ParticleSystem component found in the object");
                    }
                    break;
                    break;
                case Constants.NETO_DURATION_BY_NOISE:
                    break;
                case Constants.NETO_DURATION_BY_SUBEMITTER:
                    break;
                case Constants.NETO_INTENSITY_BY_CIRCLES_COLOR:
                    break;
                case Constants.NETO_INTENSITY_BY_CIRCLES_NUMBER:
                    
                    ParticleSystem partSystemByCircleNumbersV2 = effect.GetComponent<ParticleSystem>();
                    
                    if(partSystemByCircleNumbersV2 != null)
                    {
                        if(partSystemByCircleNumbersV2.isPlaying)
                        {
                            var emission = partSystemByCircleNumbersV2.emission;
                            emission.rateOverTime = value;
                        }
                    }
                    else
                    {
                        Debug.LogError("No ParticleSystem component found in the object");
                    }
                    break;
                default:
                    Debug.LogError("Unknown characteristic: " + characteristicName);
                    break;
            }
            
        }
        
        
    }
}
