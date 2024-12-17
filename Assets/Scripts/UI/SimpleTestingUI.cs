using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TESTING

public class SimpleTestingUI : MonoBehaviour
{
    
    [SerializeField] private List<TestCharacteristicsSO> testIntensityCharacteristics;
    [SerializeField] private List<TestCharacteristicsSO> testDurationCharacteristics;
    //[SerializeField] private List<TestCharacteristicsSO> testCharacteristics;
    [SerializeField] private TextMeshProUGUI intensityTextBox;
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private TextMeshProUGUI intensityValueTextBox;
    [SerializeField] private TextMeshProUGUI durationTextBox;
    [SerializeField] private Slider durationSlider;
    [SerializeField] private TextMeshProUGUI durationValueTextBox;
    
    
    private float intensityCharacteristicsToChange = 2;
    private float durationCharacteristicsToChange = 1;
    
    
    
    
    private Canvas uiCanvas;
    
    
    [Serializable]
    public struct Effect {
        public string name;
        public GameObject effectObject;
    }
    public TestingUI.Effect[] effects;
    private Dictionary<string, GameObject> effectsDictionary;
    
    
    
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

        
        // Add listeners to sliders
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);
        durationSlider.onValueChanged.AddListener(OnDurationChanged);
        
        
    }
    
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uiCanvas.enabled = !uiCanvas.enabled;
        }
    }
    
    
    
    
    
    
    
    private void OnIntensityChanged(float value)
    {
        intensityValueTextBox.text = value.ToString("F2");
        
        // Interpolate to find the correct value according to the characteristic's range
        // Update the intensity of the first X characteristics in the list
        for (int i = 0; i < intensityCharacteristicsToChange; i++)
        {
            TestCharacteristicsSO characteristic = testIntensityCharacteristics[i];
            float newValue = Mathf.Lerp(characteristic.lowerBound, characteristic.upperBound, value);
            ApplyVisualEffectChanges(characteristic.name, newValue, i);
            
            //TODO: See how to grant smooth transitions between the speed values, maintaining also the other 
            //TODO: directives on emission rate and lifetime
            // Trigger the speed changed event
            if(effectsDictionary[characteristic.name].TryGetComponent<NetoFeedbackHandler>(out NetoFeedbackHandler netoFeedbackHandler))
            {
                netoFeedbackHandler.TriggerAudioSourceSpeedChanged();
                //netoFeedbackHandler.TriggerSpeedChanged();
            }
        }
        
        
        
    }
    
    private void OnDurationChanged(float value)
    {
        durationValueTextBox.text = value.ToString("F2");
        
        // Interpolate to find the correct value according to the characteristic's range
        // Update the duration of the first X characteristics in the list
        for (int i = 0; i < durationCharacteristicsToChange; i++)
        {
            TestCharacteristicsSO characteristic = testDurationCharacteristics[i];
            float newValue = Mathf.Lerp(characteristic.lowerBound, characteristic.upperBound, value);
            Debug.Log("Characteristic: " + characteristic.name);
            Debug.Log("Lowerbound: " + characteristic.lowerBound);
            Debug.Log("Upperbound: " + characteristic.upperBound);
            Debug.Log("Value: " + newValue);
            ApplyVisualEffectChanges(characteristic.name, newValue, i);
        }
        
        
        
    }
    
    
    
    
    
    
    
    
    
    
         private void ApplyVisualEffectChanges(string characteristicName, float value, int index)
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
                            //audioSourceByVolume.outputAudioMixerGroup.audioMixer.SetFloat("volume", -20.0f);
                            
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
                    /*ParticleSystem partSystemByCircleColor = effect.GetComponent<ParticleSystem>();
                    Debug.Log("NEW COLOR: " + newColor);
                    if(partSystemByCircleColor != null)
                    {
                        if(partSystemByCircleColor.isPlaying)
                        {
                            var colorOverLifetime = partSystemByCircleColor.main.startColor;
                            colorOverLifetime.color = newColor;
                        }
                    }
                    else
                    {
                        Debug.LogError("No ParticleSystem component found in the object");
                    }*/
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
                case Constants.NETO_INTENSITY_BY_SPEED_MULTIPLIER:
                    ParticleSystem partSystemBySpeedMultiplier = effect.GetComponent<ParticleSystem>();
                    
                    if(partSystemBySpeedMultiplier != null)
                    {
                        if(partSystemBySpeedMultiplier.isPlaying)
                        {
                            // To change speed without changing the travelled distance, need to balance
                            // the speed and the lifetime of the particles.
                            // Then correct the rate of emission to keep the same visual effect
                            var main = partSystemBySpeedMultiplier.main;
                            var emission = partSystemBySpeedMultiplier.emission;
                            //var ininitalIntensity = testIntensityCharacteristics[index].initialValue;
                            var ininitalIntensity = 10.0f;
                            var initialStartLifetime = 1.0f;
                            main.startSpeed = ininitalIntensity * value;
                            main.startLifetime = initialStartLifetime / value;
                            emission.rateOverTime = ininitalIntensity * value;
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
