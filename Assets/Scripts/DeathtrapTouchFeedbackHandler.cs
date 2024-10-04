using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class DeathtrapTouchFeedbackHandler : MonoBehaviour
{
    
    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;
    
    //private Vector3 particleEffectStopPosition;
    
    private VisualEffect effect;
    private float stripsLifetime;
    
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
        //spawnPosition = effect.GetVector3("SpawnPosition");
        spawnPosition = attractor.transform.position;
        
        
        
        /*gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);*/
        
    }
    
    
    private void Update()
    {
        
        HandledEvents();
        MoveAttractor();
    }
    
    
    
    
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            VinesEffectStarted();
        }
    }



    // TODO: improve effect by considering the duration of touch
    public void VinesEffectStarted()
    {
        if(attractor.position != spawnPosition)
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
    
    
    public void StopEffect(bool isPositiveEffect)
    {
        /*effect.Stop();
        effect.Reinit();*/
        float quickFadeOutDuration = 0.3f;

        if (isPositiveEffect)
        {
            
        }
        else
        {
            StartCoroutine(FadeOutNegativeEffect(quickFadeOutDuration)); // Adjust the duration as needed
        }
        

    }
    
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
        
        // TODO: Need to reassign the original one? Not sure...
        //effect.SetFloat("StripsLifetime", stripsLifetime);
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
    
    public void ResetInitialPosition()
    {
        //effect.SetFloat("StripsLifetime", 0f);
        attractor.position = spawnPosition;
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
            if (Vector3.Distance(spawnPosition, particleEndpointPosition.position) <=
                Vector3.Distance(spawnPosition, attractor.position) + offset)
            {
                shouldMove = false;
                attractor.position = particleEndpointPosition.position;
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
    
    
    public float GetStripsLifetime()
    {
        return stripsLifetime;
    }
    
    
    public bool IsNegativeEffectPlaying()
    {
        if (effect.aliveParticleCount > 0)
        {
            return true;
        }
        return false;
    }
    
    
    public bool IsPositiveEffectPlaying()
    {
        // TODO: Implement this method
        return false;
    }
    

}
