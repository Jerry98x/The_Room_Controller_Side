using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

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
    [SerializeField] GameObject humanSilhouette;
    [SerializeField] private Transform audioEmissionPosition;
    

    private VisualEffect silhouetteEffect;
    private Vector3 silhouetteOriginalPosition;
    private Vector3 particleDirection;
    private float soundSpeed; // Speed of the AudioSource object movement

    private bool shouldMove = false; // To control when the AudioSource object should start moving
    private Vector3 audiosourceInitialPosition;
    
    
    
    private Coroutine silhouetteGeneralCoroutine;
    private Coroutine silhouetteMoveCoroutine;
    private Coroutine silhouetteFadeInCoroutine;
    private Coroutine silhouetteFadeOutCoroutine;
    
    
    
    //public delegate void ParticleSystemEventHandler();
    //public event EventHandler OnParticleSystemStopped;
    public event EventHandler OnSpeedChanged;


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the ParticleSystem component and the AudioSource object's relevant properties
    /// </summary>
    private void Start()
    {
        // Get the ParticleSystem and VFX components
        partSystem = GetComponent<ParticleSystem>();
        soundSpeed = partSystem.main.startSpeed.constant;
        silhouetteEffect = humanSilhouette.GetComponent<VisualEffect>();

        // Store the initial position of the AudioSource object
        audiosourceInitialPosition = audioSource[0].transform.position;
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
        audiosourceInitialPosition = partSystem.transform.position;  // Should be the same position as the particle system
        
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
            audioSource[0].transform.position = audiosourceInitialPosition;

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
            float testingVolumeFromPhysicalModule = 3500f;
            HandleSilhouetteAndAudioEffects(testingVolumeFromPhysicalModule);
            
            /*partSystem.Play();
            shouldMove = true;
            audioSource[0].Play();*/
        }
    }




    public void HandleSilhouetteAndAudioEffects(float vol)
    {
        humanSilhouette.SetActive(true);
        silhouetteOriginalPosition = humanSilhouette.transform.position;
        
        float fadeInDuration = 0.2f;
        float movementDuration = 0.4f;
        bool forward = true;
        silhouetteFadeInCoroutine = StartCoroutine(FadeInSilhouette(fadeInDuration));
        //silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(movementDuration));
        silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(movementDuration, forward, () =>
        {
            AudioEffectStarted(vol);
            StartCoroutine(WaitForAudioEffectToEnd());
        }));
        
        
        
        
    }
    
    
    
    private IEnumerator MoveSilhouette(float duration, bool isForward, System.Action onComplete = null)
    {
        Debug.Log("MADONNA: Human silhouette position: " + humanSilhouette.transform.position);
        Vector3 initialPosition = humanSilhouette.transform.position;
        Vector3 movementDirection;
        float multiplier;
        Vector3 finalPosition;


        if (isForward)
        {
            movementDirection = audioEmissionPosition.position - initialPosition;
            //multiplier = Vector3.Distance(netoEndPoint.position, initialPosition);
            multiplier = Mathf.Abs(audioEmissionPosition.position.z - initialPosition.z);
            finalPosition = audioEmissionPosition.position + movementDirection.normalized * multiplier;
            //Debug.Log("MADONNA: Initial position: " + initialPosition);
            Debug.Log("MADONNA: Z of Neto endpoint is " + audioEmissionPosition.position.z + " and Z of initial position is " +
                      initialPosition.z + " and the difference is " + multiplier);
            Debug.Log("MADONNA: Neto endpoint position: " + audioEmissionPosition.position);
            Debug.Log("MADONNA: Distance between Neto endpoint and initial position: " + Vector3.Distance(audioEmissionPosition.position, initialPosition));
            Debug.Log("MADONNA: Particle endpoint position: " + particleEndpointPosition.position);
            Debug.Log("MADONNA: Movement direction: " + movementDirection);
            Debug.Log("MADONNA: Final position: " + finalPosition);
            
            /*movementDirection = netoEndPoint.position - initialPosition;
            multiplier = Mathf.Abs(netoEndPoint.position.z - initialPosition.z);
            finalPosition = initialPosition + movementDirection.normalized * multiplier;*/
        }
        else
        {
            movementDirection = silhouetteOriginalPosition - initialPosition;
            multiplier = Vector3.Distance(silhouetteOriginalPosition, initialPosition);
            //multiplier = Mathf.Abs(silhouetteOriginalPosition.z - initialPosition.z);
            finalPosition = silhouetteOriginalPosition;
        }
        
        
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            humanSilhouette.transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        humanSilhouette.transform.position = finalPosition;
        
        onComplete?.Invoke();
        //yield return new WaitForSeconds(0.1f);
        
    }
    
    
    private IEnumerator FadeInSilhouette(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            silhouetteEffect.SetFloat("Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        silhouetteEffect.SetFloat("Alpha", 1f);
        
    }
    
    
    private IEnumerator FadeOutSilhouette(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            silhouetteEffect.SetFloat("Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        silhouetteEffect.SetFloat("Alpha", 0f);
        humanSilhouette.SetActive(false);
        
    }
    
    
    private IEnumerator WaitForAudioEffectToEnd()
    {
        yield return new WaitUntil( () => !partSystem.IsAlive() );
        float fadeOutDuration = 0.2f;
        float movementDuration = 0.4f;
        StartCoroutine(MoveSilhouette(movementDuration, false, () =>
        {
            StartCoroutine(FadeOutSilhouette(fadeOutDuration));
        }));
    }
    
    




    public void AudioEffectStarted(float vol)
    {
        partSystem.transform.position = humanSilhouette.transform.position;
        ParticleSystem.MainModule main = partSystem.main;
        main.startSize = RangeRemappingHelper.Remap(vol, Constants.NETO_MIC_VOLUME_MAX, Constants.NETO_MIC_VOLUME_THRESHOLD, maxParticleSize, minParticleSize);
        partSystem.Play();
        shouldMove = true;
        audioSource[0].transform.position = humanSilhouette.transform.position;
        audioSource[0].volume = RangeRemappingHelper.Remap(vol, Constants.NETO_MIC_VOLUME_MAX, Constants.NETO_MIC_VOLUME_THRESHOLD, 0.5f, 1);
        audioSource[0].Play();
        
    }
    
    
    public void IncreaseAudioEffectVolume(float vol)
    {
        audioSource[0].volume = RangeRemappingHelper.Remap(vol, Constants.NETO_MIC_VOLUME_MAX, Constants.NETO_MIC_VOLUME_THRESHOLD, 0.5f, 1);
    }
    
    public void IncreaseParticleEffectLifeTime(float deltaLifetime)
    {
        ParticleSystem.MainModule main = partSystem.main;
        main.startLifetime = main.startLifetime.constant + deltaLifetime;
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
                if ((Vector3.Distance(source.transform.position, particleEndpointPosition.position) >= offset) && (Vector3.Distance(audiosourceInitialPosition, source.transform.position) >=
                    Vector3.Distance(audiosourceInitialPosition, particleEndpointPosition.position) + offset))
                {
                    source.transform.position = audiosourceInitialPosition;
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
        audioSource[0].transform.position = audiosourceInitialPosition;

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
