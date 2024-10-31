using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
        Debug.Log("Lifetime Min in Start: " + goodParticlesMinLifetime);
        Debug.Log("Lifetime Max in Start: " + goodParticlesMaxLifetime);
    }


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
    
    
    public void StopEffect()
    {
        /*effect.Stop();
        effect.Reinit();*/
        float quickFadeOutDuration = 0.2f;
        
        StartCoroutine(FadeOutPositiveEffect(quickFadeOutDuration));
        
        ResetAttractorInitialPosition();
        ResetEffectObjectInitialPosition();
    }
    
    
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
    
    
    public void ResetAttractorInitialPosition()
    {
        attractor.position = attractorInitialPosition;
        shouldMove = false;
    }

    public void ResetEffectObjectInitialPosition()
    {
        transform.position = Vector3.zero;
    }

    public void ResetInitialLifetime()
    {
        goodParticlesMinLifetime = initialParticlesLifetime;
        goodParticlesMaxLifetime = initialParticlesLifetime;
    }
    
    public void IncreaseParticlesMinLifetime(float value)
    {
        goodParticlesMinLifetime += value;
        effect.SetFloat("Lifetime Min", goodParticlesMinLifetime);
    }
    
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
        //return effect.GetFloat("Lifetime Max");
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
    
    
    public bool IsPositiveEffectPlaying()
    {
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    
    
}
