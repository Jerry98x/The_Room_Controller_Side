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
    
    
    private ParticleSystem partSystem;
    private Vector3 particleDirection;
    
    private bool emergencyActive = false;



    private void Start()
    {
        partSystem = GetComponent<ParticleSystem>();
        SetParticleSystemDirection();
        
        targetSize = partSystem.main.startSize.constant;
        
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
        //TODO: fix a bug here
        if(!partSystem.isPlaying)
        {
            partSystem.Play();
        }
        
        if (partSystem != null && partSystem.isPlaying)
        {
            // Change the particles' size based on the loudness
            ChangeParticleSize(loudness);
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
            ParticleSystem.MainModule main = partSystem.main;
                    
            targetSize = Mathf.Lerp(minSize, maxSize, loudness);
            
            // Lerping the size based on the loudness
            main.startSize = Mathf.Lerp(main.startSize.constant, targetSize, Time.deltaTime * smoothingSpeed);
        }
        else
        {
            partSystem.Stop();
        }

    }
    
    
    
}
