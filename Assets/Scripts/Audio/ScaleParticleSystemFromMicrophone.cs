using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScaleParticleSystemFromMicrophone : MonoBehaviour
{

    
    [SerializeField] private Transform particleEndpointPosition;
    
    [SerializeField] private AudioSource source;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private AudioLoudnessDetection loudnessDetector;
    
    [SerializeField] private float loudnessSensibility = 100f;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float smoothingSpeed = 5f;
    
    [SerializeField]private HandleNetoRayMovement handleNetoRayMovement;
    
    private float targetSize;
    private float lastSize;
    
    
    private ParticleSystem partSystem;
    private Vector3 particleDirection;
    
    private bool emergencyActive = false;



    private void Start()
    {
        partSystem = GetComponent<ParticleSystem>();
        SetParticleSystemDirection();
        
        targetSize = partSystem.main.startSize.constant;
        lastSize = 0;
        
        handleNetoRayMovement.OnEmergencyStatusChanged += UpdateEmergencyStatus;
    }
    
    
    private void Update()
    {
        float loudness = loudnessDetector.GetLoudnessFromMicrophone() * loudnessSensibility;
        
        if(loudness < threshold)
        {
            loudness = 0;
        }
        
        // To account for when after the emergency mode is interrupted
        //TODO: fix a bug here (maybe already solved, can't remember)
        if(partSystem != null && !partSystem.isPlaying && !emergencyActive && handleNetoRayMovement.IsInControl())
        {
            partSystem.Play();
        }
        
        Debug.Log("IL PARTICLE SYSTEM STA SUONANDO? " + partSystem.isPlaying);
        if (partSystem != null && partSystem.isPlaying && handleNetoRayMovement.IsInControl())
        {
            // Change the particles' size based on the loudness
            if(loudness > Constants.MAX_LOUDNESS)
            {
                loudness = Constants.MAX_LOUDNESS;
            }
            Debug.Log("The loudness is: " + loudness);
            ChangeParticleSize(loudness);
        }
        else if(partSystem != null && !handleNetoRayMovement.IsInControl() && !emergencyActive)
        {
            // Maintain the last useful size of the particles to be played when I'm not in control of the Neto anymore,
            // to give a sense of "leaving the Neto in that state" (until I will act on it again)
            partSystem.Stop();
            ParticleSystem.MainModule main = partSystem.main;
            main.startSize = lastSize;
            partSystem.Play();
        }
        
    }
    
    
    private void OnDestroy()
    {
        handleNetoRayMovement.OnEmergencyStatusChanged -= UpdateEmergencyStatus;
    }

    private void UpdateEmergencyStatus(bool hasEmergency)
    {
        emergencyActive = hasEmergency;
    }
    
    
    private void SetParticleSystemDirection()
    {
        particleDirection = particleEndpointPosition.position - transform.position;
        transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    private void ChangeParticleSize(float loudness)
    {
        if(!emergencyActive)
        {
            if(handleNetoRayMovement.IsInControl() && !partSystem.isPlaying)
            {
                partSystem.Play();
            }
            
            ParticleSystem.MainModule main = partSystem.main;
                    
            targetSize = Mathf.Lerp(minSize, maxSize, loudness);
            
            // Lerping the size based on the loudness
            float newSize = Mathf.Lerp(main.startSize.constant, targetSize, Time.deltaTime * smoothingSpeed);
            main.startSize = newSize;
            lastSize = newSize;
        }
        else
        {
            partSystem.Stop();
        }


    }
    
    public float GetMaxSize()
    {
        return maxSize;
    }
    
    public float GetMinSize()
    {
        return minSize;
    }
    
    public float GetLoudnessThreshold()
    {
        return threshold;
    }
    
    public float GetLoudnessSensibility()
    {
        return loudnessSensibility;
    }
    
    public float GetAudioLoudness()
    {
        return loudnessDetector.GetLoudnessFromMicrophone();
    }
    
    
}
