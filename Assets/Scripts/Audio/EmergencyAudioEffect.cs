using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// NOT USED

/// <summary>
/// Handles the audio specifically for the (unused) emergency effect
/// </summary>
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
    
    private float soundSpeed;
    
    private bool shouldMove = false;
    private Vector3 initialPosition;
    
    
    private void Start()
    {
        
        soundSpeed = partSystem.main.startSpeed.constant;
        
        emergencyParticleSize = partSystem.main.startSize.constant;
        SetParticleSystemDirection();
        
        initialPosition = source.transform.position;
        
        onEmergencyStart.AddListener(StartEmergencyAudio);
        handleNetoRayMovement.OnEmergencyStatusChanged += UpdateEmergencyStatus;

    }


    private void Update()
    {
        
        //MoveAudioSource();
        
    }
    
    
    private void OnDestroy()
    {
        handleNetoRayMovement.OnEmergencyStatusChanged -= UpdateEmergencyStatus;
    }

    private void UpdateEmergencyStatus(bool hasEmergency)
    {
        emergencyActive = hasEmergency;
        shouldMove = hasEmergency;
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



    private void MoveAudioSource()
    {

        if (shouldMove)
        {
            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;
            
            source.transform.Translate(soundSpeed  * Time.deltaTime * normalizedDirection, Space.World);

            
            float offset = 3f;
            if ((Vector3.Distance(source.transform.position, particleEndpointPosition.position) >= offset) && (Vector3.Distance(initialPosition, source.transform.position) >=
                    Vector3.Distance(initialPosition, particleEndpointPosition.position) + offset))
            {
                source.transform.position = initialPosition;
            }

            
        }
        
    }
    
    
    
}
