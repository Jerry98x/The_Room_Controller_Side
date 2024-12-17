using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Class that handles the negative feedback effect when the player touches the Deathtrap.
/// </summary>
public class DeathtrapTouchNegativeFeedbackHandler : MonoBehaviour
{

    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;

    //private Vector3 particleEffectStopPosition;

    private VisualEffect effect;
    private float stripsLifetime;
    private float initialStripsLifetime;

    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 10f;
    private Vector3 spawnPosition;


    private void Start()
    {

        // Translate the VFX object to the origin of the scene, because in all the other instances
        // of the vines effect, the VFX object is placed at the origin of the scene, while in this case
        // it is place in the origin of the Deathtrap object (which is not the origin of the scene),
        // and the effect works properly only when the VFX object is placed at the origin of the scene
        transform.position -= transform.position;


        effect = GetComponent<VisualEffect>();
        stripsLifetime = effect.GetFloat("StripsLifetime");
        initialStripsLifetime = stripsLifetime;
        //spawnPosition = effect.GetVector3("SpawnPosition");
        spawnPosition = attractor.transform.position;
        

    }


    private void Update()
    {

        HandleEvents();
        MoveAttractor();
    }





    private void HandleEvents()
    {
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            VinesEffectStarted();
        }*/
    }



    /// <summary>
    /// Starts the negative feedback effect of the Deathtrap.
    /// </summary>
    public void VinesEffectStarted()
    {
        if (attractor.position != spawnPosition)
        {
            ResetInitialPosition();
        }
        else
        {
            // Clear the particles emitted by the VisualEffect object
            effect.Stop();
            //effect.SetFloat("StripsLifetime", stripsLifetime);
            effect.Reinit();

            effect.SetVector3("SpawnPosition", spawnPosition);
            attractor.position = spawnPosition;

            SetAttractorDirection();

            effect.SendEvent("VinesEffectPlay");
            shouldMove = true;
        }
    }


    /// <summary>
    /// Stops the negative feedback effect of the Deathtrap.
    /// </summary>
    public void StopEffect()
    {
        float quickFadeOutDuration = 0.2f;
        
        StartCoroutine(FadeOutNegativeEffect(quickFadeOutDuration));
    }

    /// <summary>
    /// Coroutine that fades out the negative effect of the Deathtrap.
    /// </summary>
    /// <param name="duration"> Fade-out duration </param>
    private IEnumerator FadeOutNegativeEffect(float duration)
    {
        float elapsed = 0.0f;
        float initialLifetime = effect.GetFloat("StripsLifetime");

        while (elapsed < duration)
        {
            float newLifetime = Mathf.Lerp(initialLifetime, 0f, elapsed / duration);
            effect.SetFloat("StripsLifetime", newLifetime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effect.SetFloat("StripsLifetime", 0f);
        effect.Stop();
        effect.Reinit();
        
    }




    public void SetAttractorPosition(Vector3 position)
    {
        attractor.position = position;
    }


    public void SetAttractorDirection()
    {
        particleDirection = particleEndpointPosition.position - attractor.transform.position;
        //effect.transform.rotation = Quaternion.LookRotation(particleDirection);
    }

    private void SetDirectionAndPosition()
    {
        gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.position = spawnPosition;
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);


        particleDirection = particleEndpointPosition.position - attractor.transform.position;

    }

    /// <summary>
    /// Resets the initial position of the Attractor object.
    /// </summary>
    public void ResetInitialPosition()
    {
        //effect.SetFloat("StripsLifetime", 0f);
        attractor.position = spawnPosition;
        shouldMove = false;
    }


    /// <summary>
    /// Translates the Attractor object towards the particleEndPointPosition.
    /// </summary>
    private void MoveAttractor()
    {
        if (shouldMove)
        {

            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;

            attractor.Translate(attractorSpeed * Time.deltaTime * normalizedDirection, Space.World);

            float offset = 0.1f;

            // When the Attractor object has reached the particleEndPointPosition, stops its translation without resetting its position
            if (Vector3.Distance(spawnPosition, particleEndpointPosition.position) <=
                Vector3.Distance(spawnPosition, attractor.position) + offset)
            {
                shouldMove = false;
                attractor.position = particleEndpointPosition.position;
            }


        }
    }

    
    /// <summary>
    /// Resets the initial lifetime of the particles emitted by the VisualEffect object.
    /// </summary>
    public void ResetInitialLifetime()
    {
        
        stripsLifetime = initialStripsLifetime;
    }

    /// <summary>
    /// Increases the lifetime of the particles emitted by the VisualEffect object.
    /// </summary>
    /// <param name="value"> Value of the lifetime increase </param>
    public void IncreaseParticlesLifetime(float value)
    {
        stripsLifetime += value;
        effect.SetFloat("StripsLifetime", stripsLifetime);
    }
    
    
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }
    
    public VisualEffect GetEffect()
    {
        return effect;
    }
    
    
    public float GetStripsLifetime()
    {
        return stripsLifetime;
    }
    
    public float GetInitialStripsLifetime()
    {
        return initialStripsLifetime;
    }
    
    
    /// <summary>
    /// Checks if the negative effect is playing.
    /// </summary>
    public bool IsNegativeEffectPlaying()
    {
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    

}
