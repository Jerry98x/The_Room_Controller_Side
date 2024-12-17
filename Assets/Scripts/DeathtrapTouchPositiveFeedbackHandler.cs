using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Class that handles the positive feedback effect when the player touches the Deathtrap.
/// </summary>
public class DeathtrapTouchPositiveFeedbackHandler : MonoBehaviour
{

    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;
    
    private Vector3 spawnPosition;
    private float distance;
    
    
    private VisualEffect effect;
    private float goodParticlesMinLifetime;
    private float goodParticlesMaxLifetime;
    private float initialParticlesLifetime;
    private Vector3 attractorInitialPosition;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving


    private void Start()
    {
        // Translate the VFX object to the origin of the scene, because in all the other instances
        // of the vines effect, the VFX object is placed at the origin of the scene, while in this case
        // it is place in the origin of the Deathtrap object (which is not the origin of the scene),
        // and the effect works properly only when the VFX object is placed at the origin of the scene
        transform.position -= transform.position;
        
        attractorInitialPosition = attractor.position;
        
        effect = GetComponent<VisualEffect>();
        goodParticlesMinLifetime = effect.GetFloat("Lifetime Min");
        goodParticlesMaxLifetime = effect.GetFloat("Lifetime Max");
        initialParticlesLifetime = goodParticlesMaxLifetime;
    }


    /// <summary>
    /// Starts the positive feedback effect of the Deathtrap.
    /// </summary>
    public void GoodParticlesEffectStarted()
    {
        if (attractor.position != attractorInitialPosition)
        {
            ResetAttractorInitialPosition();
        }
       
        // Clear the particles emitted by the VisualEffect object
        effect.Stop();
        //effect.SetFloat("StripsLifetime", stripsLifetime);
        effect.Reinit();
        
        effect.SetFloat("Lifetime Min", goodParticlesMinLifetime);
        effect.SetFloat("Lifetime Max", goodParticlesMaxLifetime);

        transform.position = spawnPosition;
        distance = Vector3.Distance(transform.position, particleEndpointPosition.position);
        
        effect.SendEvent("GoodParticlesEventPlay");
        attractor.position += attractor.right * distance; // The Attractor object is positioned specifically to work like this
        shouldMove = true;
        
    }
    
    
    /// <summary>
    /// Stops the positive feedback effect of the Deathtrap.
    /// </summary>
    public void StopEffect()
    {
        float quickFadeOutDuration = 0.2f;
        
        StartCoroutine(FadeOutPositiveEffect(quickFadeOutDuration));
        
        ResetAttractorInitialPosition();
        ResetEffectObjectInitialPosition();
    }
    
    /// <summary>
    /// Coroutine that fades out the positive effect of the Deathtrap.
    /// </summary>
    /// <param name="duration"> Fade-out duration </param>
    private IEnumerator FadeOutPositiveEffect(float duration)
    {
        float elapsed = 0;
        float initialLifetimeMax = effect.GetFloat("Lifetime Max");
        
        while (elapsed < duration)
        {
            float newLifetime = Mathf.Lerp(initialLifetimeMax, 0f, elapsed / duration);
            effect.SetFloat("Lifetime Max", newLifetime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        effect.SetFloat("Lifetime Min", 0);
        effect.SetFloat("Lifetime Max", 0);
        effect.Stop();
        effect.Reinit();
    }
    
    /// <summary>
    /// Resets the initial position of the Attractor object.
    /// </summary>
    public void ResetAttractorInitialPosition()
    {
        attractor.position = attractorInitialPosition;
        shouldMove = false;
    }

    /// <summary>
    /// Resets the initial position of the VFX object.
    /// </summary>
    public void ResetEffectObjectInitialPosition()
    {
        transform.position = Vector3.zero;
    }

    /// <summary>
    /// Resets the initial lifetime of the particles (min and max).
    /// </summary>
    public void ResetInitialLifetime()
    {
        goodParticlesMinLifetime = initialParticlesLifetime;
        goodParticlesMaxLifetime = initialParticlesLifetime;
    }
    
    /// <summary>
    /// Increases the min lifetime of the particles emitted by the VisualEffect object.
    /// </summary>
    /// <param name="value"> Value of the lifetime increase </param>
    public void IncreaseParticlesMinLifetime(float value)
    {
        goodParticlesMinLifetime += value;
        effect.SetFloat("Lifetime Min", goodParticlesMinLifetime);
    }
    
    /// <summary>
    /// Increases the max lifetime of the particles emitted by the VisualEffect object.
    /// </summary>
    /// <param name="value"> Value of the lifetime increase </param>
    public void IncreaseParticlesMaxLifetime(float value)
    {
        goodParticlesMaxLifetime += value;
        effect.SetFloat("Lifetime Max", goodParticlesMaxLifetime);
    }
    
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }
    
    public VisualEffect GetEffect()
    {
        return effect;
    }
    
    
    public float GetGoodParticlesMaxLifetime()
    {
        return goodParticlesMaxLifetime;
    }
    
    public float GetGoodParticlesMinLifetime()
    {
        return goodParticlesMinLifetime;
    }
    
    public Transform GetAttractor()
    {
        return attractor;
    }
    
    
    /// <summary>
    /// Checks if the positive effect is playing.
    /// </summary>
    public bool IsPositiveEffectPlaying()
    {
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    
    
}
