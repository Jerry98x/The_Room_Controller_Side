using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.VFX;

public class SauronFeedbackHandler : MonoBehaviour
{
    
    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;
    [SerializeField] private Transform rayEndPoint;
    [SerializeField] GameObject humanSilhouette;
    
    private Vector3 particleEffectStopPosition;
    
    private VisualEffect effect;
    private VisualEffect silhouetteEffect;
    private Vector3 silhouetteOriginalPosition;
    private float stripsLifetime;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 15f;
    private Vector3 spawnPosition;
    
    
    
    private Coroutine silhouetteGeneralCoroutine;
    private Coroutine silhouetteMoveCoroutine;
    private Coroutine silhouetteFadeInCoroutine;
    private Coroutine silhouetteFadeOutCoroutine;
    
    
    //private float yOffset = -0.5f;


    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        stripsLifetime = effect.GetFloat("StripsLifetime");
        spawnPosition = effect.GetVector3("SpawnPosition");
        silhouetteEffect = humanSilhouette.GetComponent<VisualEffect>();
        
        /*gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);*/
        
    }


    private void Update()
    {
        // Particles effect needs to stop at 2/3 the distance between the spawn position and the particle endpoint position
        // Basically the "Lerp" function
        particleEffectStopPosition = rayEndPoint.transform.position + 2f * (particleEndpointPosition.position - rayEndPoint.transform.position) / 3f;
        
        
        HandledEvents();
        MoveAttractor();
    }
    
    
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            HandleSilhouetteAndVinesEffects();
            //VinesEffectStarted();
        }
    }
    
    
    public void HandleSilhouetteAndVinesEffects()
    {
        /*// Position the silhouette in the smaller portal
        humanSilhouette.transform.position = rayEndPoint.transform.position - new Vector3(0f, 0f, 0.5f);
        Debug.Log("Positioning silhouette at: " + humanSilhouette.transform.position);
        // Make the silhouette look at the core center
        humanSilhouette.transform.LookAt(particleEndpointPosition.position);*/
        
        humanSilhouette.SetActive(true);
        silhouetteOriginalPosition = humanSilhouette.transform.position;
        
        float fadeInDuration = 0.4f;
        float movementDuration = 1f;
        float fadeOutDuration = 0.5f;
        bool forward = true;
        silhouetteFadeInCoroutine = StartCoroutine(FadeInSilhouette(fadeInDuration));
        //silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(movementDuration));
        silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(movementDuration, forward, () =>
        {
            VinesEffectStarted();
            StartCoroutine(WaitForVinesEffectToEnd());
        }));
        
        
        //VinesEffectStarted();
        
        //silhouetteFadeOutCoroutine = StartCoroutine(FadeOutSilhouette(fadeOutDuration));
        
        
        
        
    }
    
    
    
    
    
    
    
    private IEnumerator MoveSilhouette(float duration, bool isForward, System.Action onComplete = null)
    {
        
        Vector3 initialPosition = humanSilhouette.transform.position;
        Vector3 movementDirection;
        float multiplier;
        Vector3 finalPosition;


        if (isForward)
        {
            movementDirection = particleEndpointPosition.position - initialPosition;
            multiplier = Mathf.Abs(rayEndPoint.transform.position.z - initialPosition.z) - 0.2f;
            finalPosition = rayEndPoint.transform.position + movementDirection.normalized * multiplier;
        }
        else
        {
            movementDirection = silhouetteOriginalPosition - initialPosition;
            multiplier = Mathf.Abs(silhouetteOriginalPosition.z - initialPosition.z);
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
    
    
    private IEnumerator WaitForVinesEffectToEnd()
    {
        yield return new WaitForSeconds(stripsLifetime);
        float fadeOutDuration = 0.5f;
        float movementDuration = 1f;
        StartCoroutine(MoveSilhouette(movementDuration, false, () =>
        {
            StartCoroutine(FadeOutSilhouette(fadeOutDuration));
        }));
    }
    



    // TODO: improve effect by considering the duration of touch
    public void VinesEffectStarted()
    {
        if(attractor.position != rayEndPoint.transform.position)
        {
            ResetInitialPosition();
        }
        else
        {
            // Clear the particles emitted by the VisualEffect object
            effect.Stop();
            //effect.SetFloat("StripsLifetime", stripsLifetime);
            effect.Reinit();
                
            effect.SetVector3("SpawnPosition", rayEndPoint.transform.position);
            attractor.position = rayEndPoint.transform.position;
            SetAttractorDirection();
                
            effect.SendEvent("VinesEffectPlay");
            shouldMove = true;
        }
    }
    
    
    
    private void SetAttractorDirection()
    {
        particleDirection = particleEndpointPosition.position - attractor.transform.position;
        //effect.transform.rotation = Quaternion.LookRotation(particleDirection);
    }

    private void SetDirectionAndPosition()
    {
        gameObject.transform.position = rayEndPoint.transform.position;
        gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.position = rayEndPoint.transform.position;
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);
        
        
        particleDirection = particleEndpointPosition.position - attractor.transform.position;
        
    }
    
    private void ResetInitialPosition()
    {
        //effect.SetFloat("StripsLifetime", 0f);
        attractor.position = rayEndPoint.transform.position;
        shouldMove = false;
    }
    
    
    private void MoveAttractor()
    {
        if (shouldMove)
        {
            
            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;
            
            attractor.Translate(attractorSpeed * Time.deltaTime * normalizedDirection, Space.World);
            
            float offset = 0.1f;
            
            // When the Attractor object has reached the particleEndPointPosition, stops its translation without resetting its position
            if (Vector3.Distance(spawnPosition, particleEffectStopPosition) <=
                Vector3.Distance(spawnPosition, attractor.position) + offset)
            {
                shouldMove = false;
                attractor.position = particleEffectStopPosition;
            }
            
        
            
        }
    }
    
    
    public void IncreaseParticlesLifetime(float value)
    {
        stripsLifetime += value;
        effect.SetFloat("StripsLifetime", stripsLifetime);
    }
    
    
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }
    
    
    public bool IsEffectPlaying()
    {
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    
    
}

