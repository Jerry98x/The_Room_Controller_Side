using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class that handles the feedback received by the Controller when perceiving the Visitor through a Neto module
/// </summary>
/// <remarks>
/// Neto modules can provide feedback in the form of audio (and its visualization)
/// </remarks>
public class NetoFeedbackHandler : MonoBehaviour
{
    
    private ParticleSystem partSystem;

    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private AudioSource[] audioSource;
    

    private Vector3 particleDirection;
    private float soundSpeed; // Speed of the AudioSource object movement

    private bool shouldMove = false; // To control when the AudioSource object should start moving
    private Vector3 initialPosition;


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the ParticleSystem component and the AudioSource object's relevant properties
    /// </summary>
    private void Start()
    {
        // Get the ParticleSystem component
        partSystem = GetComponent<ParticleSystem>();
        soundSpeed = partSystem.main.startSpeed.constant;

        // Store the initial position of the AudioSource object
        initialPosition = audioSource[0].transform.position;
        audioSource[0].loop = true;

    }
    
    
    /// <summary>
    /// Handles events and updates the AudioSource object's position at each frame
    /// </summary>
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

    #endregion



    #region Relevant functions

    // Set the direction of the particle system
    private void SetParticleSystemDirection()
    {
        particleDirection = particleEndpointPosition.position - partSystem.transform.position;
        partSystem.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    
    /// <summary>
    /// Handles the events that trigger the feedback, playing the particle system and the audio source
    /// </summary>
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            partSystem.Play();
            shouldMove = true;
            audioSource[0].Play();
        }
    }
    
    
    /// <summary>
    /// Translate the AudioSource object in the same direction of the particles emitted by the ParticleSystem
    /// </summary>
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

    #endregion

    
    
}
