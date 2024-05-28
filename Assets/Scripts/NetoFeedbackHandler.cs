using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class NetoFeedbackHandler : MonoBehaviour
{
    
    private ParticleSystem partSystem;

    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private AudioSource[] audioSource;
    

    private Vector3 particleDirection;
    private float soundSpeed; // Speed of the AudioSource object movement

    private bool shouldMove = false; // To control when the AudioSource object should start moving
    private Vector3 initialPosition;

    
    
    private void Start()
    {
        // Get the ParticleSystem component
        partSystem = GetComponent<ParticleSystem>();
        soundSpeed = partSystem.main.startSpeed.constant;

        // Store the initial position of the AudioSource object
        initialPosition = audioSource[0].transform.position;
        audioSource[0].loop = true;

    }


    // Set the direction of the particle system
    private void SetParticleSystemDirection()
    {
        particleDirection = particleEndpointPosition.position - partSystem.transform.position;
        partSystem.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    
    private void Update()
    {
        HandledEvents();
        SetParticleSystemDirection();
        MoveAudioSource();
        
        
        // Check if the particle system is no longer alive and the sound is playing
        if (!partSystem.IsAlive() && shouldMove)
        {
            // Stop the sound
            foreach (AudioSource source in audioSource)
            {
                source.Stop();
            }

            // Reposition the AudioSource to its initial position
            audioSource[0].transform.position = initialPosition;

            // Set shouldMove to false
            shouldMove = false;
        }
        
        Debug.Log(shouldMove);
    }
    
    //TODO: rewrite it in a cleaner way; this is only for testing purposes
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            partSystem.Play();
            shouldMove = true;
            audioSource[0].Play();
        }
    }
    
    
    // Translate the AudioSource object in the same direction of the particles
    private void MoveAudioSource()
    {
        if (shouldMove) // Check if shouldMove is true before moving the AudioSource object
        {
            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;

            // Move the AudioSource object in the direction of the particles
            foreach (AudioSource source in audioSource)
            {
                source.transform.Translate(soundSpeed  * Time.deltaTime * normalizedDirection, Space.World);

                // Check if the AudioSource object has reached the particleEndpointPosition and reset its position
                if (Vector3.Distance(source.transform.position, particleEndpointPosition.position) <= 0.3f)
                {
                    source.transform.position = initialPosition;
                }
                
                
                // Check if the AudioSource object has moved past the particleEndpointPosition and reposition it in the initial position
                // Needed because the precision of the previous check may be too high
                float offset = 1.5f;
                if (Vector3.Distance(initialPosition, particleEndpointPosition.position) >=
                    Vector3.Distance(initialPosition, particleEndpointPosition.position) + offset)
                {
                    source.transform.position = initialPosition;
                }
            
            }
        }
    }
    
}
