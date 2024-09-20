using System;
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
    
    //TODO: remove the dependency from HandleNetoRayMovement; now it's only for testing purposes
    private HandleNetoRayMovement netoMovementHandler;
    
    private ParticleSystem partSystem;

    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private float minParticleSize;
    [SerializeField] private float maxParticleSize;
    

    private Vector3 particleDirection;
    private float soundSpeed; // Speed of the AudioSource object movement

    private bool shouldMove = false; // To control when the AudioSource object should start moving
    private Vector3 initialPosition;
    
    
    //public delegate void ParticleSystemEventHandler();
    //public event EventHandler OnParticleSystemStopped;
    public event EventHandler OnSpeedChanged;


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
        
        
        
        netoMovementHandler = transform.parent.parent.GetComponentInChildren<HandleNetoRayMovement>();
        
        
        // Add event listeners to the ParticleSystem component
        //OnParticleSystemStopped += HandleParticleSystemStopped;
        OnSpeedChanged += HandleSpeedChanged;
        OnSpeedChanged += HandleAudioSourceSpeedChanged;


    }
    
    
    /// <summary>
    /// Handles events and updates the AudioSource object's position at each frame
    /// </summary>
    private void Update()
    {
        
        // Redefine the initial position of the AudioSource object to account for eventual
        // cases in which the endpoint of the ray moves
        initialPosition = partSystem.transform.position;  // Should be the same position as the particle system
        
        HandledEvents();
        SetParticleSystemDirection();
        MoveAudioSource();
        
        //OnSpeedChanged?.Invoke(this, EventArgs.Empty);

        
        
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
        
        
        /*if (!partSystem.IsAlive() && OnParticleSystemStopped != null)
        {
            OnParticleSystemStopped?.Invoke(this, EventArgs.Empty);
        }*/
        
        
        
        //Debug.Log(shouldMove);
    }
    
    
    
    
    //TODO: rewrite it in a cleaner way; this is only for testing purposes

    #endregion



    #region Relevant functions
    
    
    /// <summary>
    /// Set the direction of the particle system
    /// </summary>
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
        if (Input.GetKeyDown(KeyCode.P) && netoMovementHandler.IsInControl())
        {
            partSystem.Play();
            shouldMove = true;
            audioSource[0].Play();
        }
    }




    public void AudioEffectStarted(float vol)
    {
        
        ParticleSystem.MainModule main = partSystem.main;
        main.startSize = RangeRemappingHelper.Remap(vol, Constants.NETO_MIC_VOLUME_MAX, Constants.NETO_MIC_VOLUME_MIN, maxParticleSize, minParticleSize);
        partSystem.Play();
        shouldMove = true;
        audioSource[0].volume = RangeRemappingHelper.Remap(vol, Constants.NETO_MIC_VOLUME_MAX, Constants.NETO_MIC_VOLUME_MIN, 0, 1);
        audioSource[0].Play();
        
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

                // Check if the AudioSource object has moved past the particleEndpointPosition and reposition it in the initial position
                // when the distance between the AudioSource object and the particleEndpointPosition is greater than 3 units
                // (adjustable offset parameter to allow the Controller to perceive better the spatialization of the sound)
                float offset = 3f;
                if ((Vector3.Distance(source.transform.position, particleEndpointPosition.position) >= offset) && (Vector3.Distance(initialPosition, source.transform.position) >=
                    Vector3.Distance(initialPosition, particleEndpointPosition.position) + offset))
                {
                    source.transform.position = initialPosition;
                }
            
            }
        }
    }

    #endregion

    
    
    private void HandleParticleSystemStopped(object sender, EventArgs e)
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
    
    private void HandleSpeedChanged(object sender, EventArgs e)
    {
        Debug.Log("Speed changed");
        // Wait a second and then destroy all previously emitted particles
        // (to avoid seeing both new particles and those slower than the new speed)
        StartCoroutine(DestroyParticles());
    }
    
    private IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(1f);
        partSystem.Clear();
    }
    
    
    private void HandleAudioSourceSpeedChanged(object sender, EventArgs e)
    {
        // Change the speed of the AudioSource object
        soundSpeed = partSystem.main.startSpeed.constant;
    }
   
    
    
    public void TriggerSpeedChanged()
    {
        OnSpeedChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void TriggerAudioSourceSpeedChanged()
    {
        OnSpeedChanged?.Invoke(this, EventArgs.Empty);
    }
    
    
    
    public bool IsAnyAudioSourcePlaying()
    {
        foreach (AudioSource source in audioSource)
        {
            if (source.isPlaying)
            {
                return true;
            }
        }
        
        return false;
    }
    
    


    public AudioSource GetHandledAudioSource()
    {
        return audioSource[0];
    }
    
}
