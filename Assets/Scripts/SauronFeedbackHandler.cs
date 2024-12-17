using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class SauronFeedbackHandler : MonoBehaviour
{
    
    [SerializeField] private Transform particleEndpointPosition; // Original stop position of the particles effect, it's the core center
    [SerializeField] private Transform attractor; // The attractor object for the vines effect
    [SerializeField] private Transform rayEndPoint; // The actual endpoint of the Sauron ray, which is moved by my VR hand controller
    [SerializeField] GameObject humanSilhouette; // The silhouette object, which is child of the ray endpoint object

    private Vector3 particleEffectStopPosition;
    
    private VisualEffect effect;
    private VisualEffect silhouetteEffect;
    private Vector3 silhouetteOriginalPosition;
    private Vector3 attractorOriginalPosition;
    private float particlesLifetime;
    private float stripsLifetime;
    private float originalLifetime;

    private bool isTouchCheck;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 15f;
    private Vector3 spawnPosition;
    private float distanceRate = 2f / 3f;
    
    
    private Coroutine silhouetteGeneralCoroutine;
    private Coroutine silhouetteMoveCoroutine;
    private Coroutine silhouetteFadeInCoroutine;
    private Coroutine silhouetteFadeOutCoroutine;
    
    
    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        particlesLifetime = effect.GetFloat("ParticlesLifetime");
        stripsLifetime = effect.GetFloat("StripsLifetime");
        originalLifetime = stripsLifetime;
        //spawnPosition = effect.GetVector3("SpawnPosition");
        spawnPosition = attractor.transform.position;
        
        silhouetteEffect = humanSilhouette.GetComponent<VisualEffect>();
        /*silhouetteOriginalPosition = humanSilhouette.transform.position;
        attractorOriginalPosition = attractor.transform.position;*/
        
        
        /*gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);*/

        // Reset the position of the TrackingPipelineVFX object (the object that emits the vines effect and this very
        // same object), because the VFX is not general and the scene origin is the position that makes it work correctly
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);

    }
    
    
    private void Update()
    {
        Debug.Log("StripsLifetime " + stripsLifetime);
        // Particles effect needs to stop at 2/3 the distance between the spawn position and the particle endpoint position
        // Basically the "Lerp" function
        //particleEffectStopPosition = rayEndPoint.transform.position + 2f * (particleEndpointPosition.position - rayEndPoint.transform.position) / 3f;
        
        HandledEvents();
        MoveAttractor();
    }
    
    /*private void LateUpdate()
    {
        // Reset LifetimeIncrement to 0 at the end of each frame to avoid continuous accumulation
        effect.SetFloat("LifetimeIncrement", 0f);
    }*/
    
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            HandleSilhouetteAndVinesEffects();
        }
    }



    public void HandleSilhouetteAndVinesEffects()
    {
        // Activate the silhouette and save the original local positions of silhouette and attractor, so that they can be used
        // to avoid inconsistencies in positioning and moving the objects when the parent object is moved
        humanSilhouette.SetActive(true);
        silhouetteOriginalPosition = humanSilhouette.transform.localPosition;
        attractorOriginalPosition = attractor.transform.localPosition;
        
        
        
        float fadeInDuration = 0.2f;
        float movementDuration = 0.4f;
        bool forward = true;
        
        // Start the coroutine to fade in the silhouette, then the coroutine to move the silhouette
        silhouetteFadeInCoroutine = StartCoroutine(FadeInSilhouette(fadeInDuration));
        silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(movementDuration, forward, () =>
        {
            // This callback is called when the silhouette has reached the endpoint position
            // It triggers the vines effect and a coroutine to wait for the vines effect to end, so that the fade out
            // coroutine of the silhouette won't start before the vines effect has ended
            VinesEffectStarted();
            StartCoroutine(WaitForVinesEffectToEnd());
        }));
        
    }


    private IEnumerator MoveSilhouette(float duration, bool isForward, System.Action onComplete = null)
    {
        // Save the initial local position of the silhouette for this execution of the coroutine, and also the
        // global initial position, for this execution of the coroutine
        Vector3 initialLocalPosition = humanSilhouette.transform.localPosition;
        Vector3 initialPosition = humanSilhouette.transform.position;
        
        Vector3 targetLocalPosition;
        Vector3 movementDirection;
        float multiplier;
        
        // Distinguish between forward and backward movement
        if (isForward)
        {
            // Calculate the movement direction, the distance the silhouette has to move and the target position
            
            // The movement direction is the vector connecting the silhouette's current position (global) and the core center
            movementDirection = particleEndpointPosition.position - initialPosition;
            
            // Move the silhouette by a distance equal to the distance between the core center and the silhouette's current
            // position (global) minus a small offset
            multiplier = Vector3.Distance(rayEndPoint.transform.position, initialPosition) - 0.2f;
            
            // Find the position where the silhouette has to move by adding the computed distance in that direction to the 
            // ray endpoint's position and then converting it to the silhouette's parent's local space
            targetLocalPosition = humanSilhouette.transform.parent.InverseTransformPoint(rayEndPoint.transform.position + movementDirection.normalized * multiplier);
        }
        else
        {
            // movementDirection and multiplier are not really necessary
            movementDirection = silhouetteOriginalPosition - humanSilhouette.transform.localPosition;
            multiplier = Vector3.Distance(silhouetteOriginalPosition, humanSilhouette.transform.localPosition);
            
            // The position where the silhouette has to move becomes the original local position
            targetLocalPosition = silhouetteOriginalPosition;
        }
        
        
        // The actual movement of the silhouette
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            // Update the target position dynamically relative to the parent object
            //targetLocalPosition = humanSilhouette.transform.parent.InverseTransformPoint(rayEndPoint.transform.position + movementDirection.normalized * multiplier);

            // Move relative to the parent object
            humanSilhouette.transform.localPosition = Vector3.Lerp(initialLocalPosition, targetLocalPosition, elapsed / duration);
        
            elapsed += Time.deltaTime;
            yield return null;
        }

        humanSilhouette.transform.localPosition = targetLocalPosition;
        onComplete?.Invoke();
        
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
        
        // Reset the attractor's initial position
        ResetAttractorInitialPosition();
        
    }
    
    
    
    private IEnumerator WaitForVinesEffectToEnd()
    {
        // Wait for the vines effect to end
        //yield return new WaitForSeconds(stripsLifetime);
        yield return new WaitUntil( () => IsEffectPlaying() );
        //yield return new WaitForSeconds(stripsLifetime);
        yield return new WaitUntil( () => !IsEffectPlaying() );
        //yield return new WaitWhile(() => IsTouchingCheck());
        //yield return new WaitForSeconds(stripsLifetime);
        //yield return new WaitUntil(() => !IsTouchingCheck() );
        //yield return new WaitUntil(() => stripsLifetime.Equals(originalLifetime));
        
        
        
        /*
        float timeout = effect.GetFloat("StripsLifetime");
        float elapsed = 0.0f;

        // Wait until the effect is no longer playing or the touch check is false, with a timeout
        while (elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            timeout = effect.GetFloat("StripsLifetime");
            Debug.Log("Lifetime - check: " + timeout);
            yield return null;
        }*/

        /*while (IsEffectPlaying() && IsTouchingCheck())
        {
            yield return null;
        }*/
        
        
        
        float fadeOutDuration = 0.3f;
        float movementDuration = 0.4f;
        
        // Start the coroutine to move the silhouette back to its original position, then the one to fade it out
        StartCoroutine(MoveSilhouette(movementDuration, false, () =>
        {
            // This callback is called when the silhouette has reached the original position
            // It triggers the fade out coroutine of the silhouette and resets the initial position of the silhouette
            // if needed
            StartCoroutine(FadeOutSilhouette(fadeOutDuration));
            effect.Stop();
            effect.Reinit();
        }));
    }
    
    
    public void VinesEffectStarted()
    {
        // Before each vines burst, reset the position of the TrackingPipelineVFX object (the object that emits the vines
        // effect and this very same object), because the VFX is not general and the scene origin is the position that
        // makes it work correctly
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        // Particles effect needs to stop at a certain rate of the distance between the spawn position and the particle
        // endpoint position. Basically, the "Lerp" function!
        particleEffectStopPosition = rayEndPoint.transform.position + distanceRate * (particleEndpointPosition.position - rayEndPoint.transform.position);
        
        if(attractor.position != rayEndPoint.transform.position)
        {
            Debug.Log("Gesù gay");
            // Mainly for testing purposes
            ResetAttractorInitialPosition();
        }
        else
        {
            Debug.Log("Gesù etero");
            // Clear the particles emitted by the VisualEffect object
            effect.Stop();
            //effect.SetFloat("StripsLifetime", stripsLifetime);
            effect.Reinit();
                
            effect.SetFloat("ParticlesLifetime", originalLifetime);
            effect.SetFloat("StripsLifetime", originalLifetime);
            effect.SetFloat("LifetimeIncrement", 0);
            spawnPosition = rayEndPoint.transform.position;
            effect.SetVector3("SpawnPosition", spawnPosition);
            attractor.position = rayEndPoint.transform.position;
            SetAttractorDirection();
                
            //SetTouchCheck(true);
            effect.SendEvent("VinesEffectPlay");
            shouldMove = true;
        }
    }
    
    
    private void SetAttractorDirection()
    {
        particleDirection = particleEndpointPosition.position - rayEndPoint.transform.position;
        //effect.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    
    
    private void ResetAttractorInitialPosition()
    {
        /*// Reset the attractor in local space
        attractor.transform.localPosition = attractorOriginalPosition;*/
        // Reset the attractor in global space
        attractor.position = rayEndPoint.transform.position;
        shouldMove = false;
    }


    private void MoveAttractor()
    {
        if (shouldMove)
        {
            // Correct the particle endpoint position, because the length of the ray may have changed and so the
            // position I want the particles to stop at (2/3 of the distance between he spawn position and the core center)
            //particleEffectStopPosition = rayEndPoint.transform.position + 2f * (particleEndpointPosition.position - rayEndPoint.transform.position) / 3f;
            
            // Correct the direction of the attractor object
            SetAttractorDirection();
            
            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;
            
            
            Vector3 movement = attractorSpeed * Time.deltaTime * normalizedDirection;
            Vector3 projectedMovement = Vector3.Project(movement, particleDirection);
            attractor.Translate(projectedMovement, Space.World);
            //attractor.Translate(attractorSpeed * Time.deltaTime * normalizedDirection, Space.World);
            
            float offset = 0.1f;
            
            // When the Attractor object has reached the particleEndPointPosition, stops its translation without resetting its position
            if (Vector3.Distance(rayEndPoint.transform.position, particleEffectStopPosition) <=
                Vector3.Distance(rayEndPoint.transform.position, attractor.position) + offset)
            {
                shouldMove = false;
                attractor.position = particleEffectStopPosition;
            }
        }
        else
        {
            // Handle the case when the attractor reached its final position: in this situation, if the ray is moved by the user,
            // the attractor object may not stick to the wanted position, so it's updated manually in this branch
            // Note: there is still a short moment when the user can move the ray and so the attractor would not stick to the wanted position,
            // namely the time when the silhouette goes back to its original position and fades out. However, the vines
            // effect is fading away as well in this moment, so it can be ignored
            if(effect.aliveParticleCount > 0)
            {
                //particleEffectStopPosition = rayEndPoint.transform.position + 2f * (particleEndpointPosition.position - rayEndPoint.transform.position) / 3f;

                if(attractor.position != particleEffectStopPosition)
                {
                    attractor.position = particleEffectStopPosition;
                }
            }
        }
    }
    

        
    public void IncreaseParticlesLifetime(float value)
    {
        //particlesLifetime += value;
        stripsLifetime += value;
        Debug.Log("Lifetime - settaggio: " + stripsLifetime);
        //effect.SetFloat("ParticlesLifetime", particlesLifetime);
        effect.SetFloat("LifetimeIncrement", value);
        Debug.Log("NEW LIFETIME: " + effect.GetFloat("StripsLifetime"));
    }

    public void ResetParticlesLifetime()
    {
        particlesLifetime = originalLifetime;
        stripsLifetime = originalLifetime;
        effect.SetFloat("LifetimeIncrement", 0);
    }
    
    
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }
    
    
    public bool IsEffectPlaying()
    {
        Debug.Log("PARTICELLE ALIVE:" + effect.aliveParticleCount);
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    
    public GameObject GetHumanSilhouette()
    {
        return humanSilhouette;
    }


    public void SetTouchCheck(bool touching)
    {
        isTouchCheck = touching;
    }

    public bool IsTouchingCheck()
    {
        return isTouchCheck;
    }



}
