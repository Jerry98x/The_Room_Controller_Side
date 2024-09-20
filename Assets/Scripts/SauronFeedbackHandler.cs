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
    
    private Vector3 particleEffectStopPosition;
    
    private VisualEffect effect;
    private float stripsLifetime;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 15f;
    private Vector3 spawnPosition;
    
    //private float yOffset = -0.5f;


    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        stripsLifetime = effect.GetFloat("StripsLifetime");
        spawnPosition = effect.GetVector3("SpawnPosition");
        
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
            VinesEffectStarted();
        }
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
            
            
            /*if (Vector3.Distance(attractor.position, particleEndpointPosition.position) <= 0.1f)
            {
                shouldMove = false;
                attractor.position = rayEndPoint.transform.position;
            }*/
        
            
        }
    }
    
    
}























/*using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.VFX;

public class SauronFeedbackHandler : MonoBehaviour
{
    
    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;
    
    private VisualEffect effect;
    private float stripsLifetime;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 15f;
    
    private Vector3 initialPosition;
    //private float yOffset = -0.5f;


    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        stripsLifetime = effect.GetFloat("StripsLifetime");
        
        initialPosition = attractor.position;
        
    }


    private void Update()
    {
        HandledEvents();
        SetAttractorDirection();
        MoveAttractor();
    }
    
    
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(attractor.position != initialPosition)
            {
                ResetInitialPosition();
            }
            
            else
            {
                // Clear the particles emitted by the VisualEffect object
                effect.Stop();
                //effect.SetFloat("StripsLifetime", stripsLifetime);
                effect.Reinit();
                effect.SendEvent("SauronPlay");
                shouldMove = true;
            }
            
            
        }
    }
    
    
    
    private void SetAttractorDirection()
    {
        particleDirection = particleEndpointPosition.position - attractor.transform.position;
        //effect.transform.rotation = Quaternion.LookRotation(particleDirection);
    }
    
    private void ResetInitialPosition()
    {
        //effect.SetFloat("StripsLifetime", 0f);
        attractor.position = initialPosition;
        shouldMove = false;
    }
    
    
    private void MoveAttractor()
    {
        if (shouldMove)
        {
            
            // Normalize the direction vector
            Vector3 normalizedDirection = particleDirection.normalized;
            
            attractor.Translate(attractorSpeed * Time.deltaTime * normalizedDirection, Space.World);
            
            // When the Attractor object has reached the particleEndPointPosition, stops its translation without resetting its position
            if (Vector3.Distance(attractor.position, particleEndpointPosition.position) <= 0.1f)
            {
                shouldMove = false;
            }
        
            
        }
    }
    
    
}*/
