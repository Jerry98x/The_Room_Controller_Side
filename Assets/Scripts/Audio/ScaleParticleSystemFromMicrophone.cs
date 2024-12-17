using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Applies a scaling effect to a Particle System based on the loudness of the microphone input
/// </summary>
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
    
    
    /// <summary>
    /// Scale the particles at each frame based on the loudness of the microphone input
    /// </summary>
    private void Update()
    {
        float loudness = loudnessDetector.GetLoudnessFromMicrophone() * loudnessSensibility;
        
        if(loudness < threshold)
        {
            loudness = 0;
        }
        
        // To account for when after the emergency mode is interrupted
        if(partSystem != null && !partSystem.isPlaying && !emergencyActive && handleNetoRayMovement.IsInControl())
        {
            partSystem.Play();
        }
        
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
    
    
    /// <summary>
    /// Sets the direction of the Particle System based on the position of the endpoint
    /// </summary>
    private void SetParticleSystemDirection()
    {
        particleDirection = particleEndpointPosition.position - transform.position;
        transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    /// <summary>
    /// Scale the particle's size based on the loudness value
    /// </summary>
    /// <param name="loudness"> The loudness value that determines the scale </param>
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
