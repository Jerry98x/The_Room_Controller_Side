using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmergencyAudioEffect : MonoBehaviour
{

    public UnityEvent onEmergencyStart;
    
    
    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private AudioSource source;
    [SerializeField] private float shortSilenceDuration = 0.3f;
    [SerializeField] private float longSilenceDuration = 2.0f;
    [SerializeField] private ParticleSystem partSystem;
    [SerializeField]private HandleNetoRayMovement handleNetoRayMovement;
    
    
    private Vector3 particleDirection;
    private float emergencyParticleSize;
    private bool emergencyActive = false;
    
    
    private void Start()
    {
        emergencyParticleSize = partSystem.main.startSize.constant;
        SetParticleSystemDirection();
        
        onEmergencyStart.AddListener(StartEmergencyAudio);
        handleNetoRayMovement.OnEmergencyStatusChanged += UpdateEmergencyStatus;

    }


    private void Update()
    {
        
    }
    
    
    private void OnDestroy()
    {
        handleNetoRayMovement.OnEmergencyStatusChanged -= UpdateEmergencyStatus;
    }

    private void UpdateEmergencyStatus(bool hasEmergency)
    {
        emergencyActive = hasEmergency;
    }
    
    
    private void StartEmergencyAudio()
    {
        StartCoroutine(PlayAudioPattern());
    }
    
    private IEnumerator PlayAudioPattern()
    {
        
        // Set the initial size of the particles to zero, then start the particle system
        partSystem.Stop();
        ParticleSystem.MainModule main = partSystem.main;
        main.startSize = 0;
        partSystem.Play();
        
        
        while (emergencyActive)
        {
            for (int j = 0; j < 3; j++)
            {
                // Play the clip and emit a particle
                source.PlayOneShot(source.clip);
                main.startSize = emergencyParticleSize;
                partSystem.Emit(1);
                main.startSize = 0;

                yield return new WaitForSeconds(shortSilenceDuration);
                
                
            }
            yield return new WaitForSeconds(longSilenceDuration);
        }
            
        partSystem.Stop();
    }


    private void SetParticleSystemDirection()
    {
        particleDirection = particleEndpointPosition.position - transform.position;
        transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    
    
    
}
