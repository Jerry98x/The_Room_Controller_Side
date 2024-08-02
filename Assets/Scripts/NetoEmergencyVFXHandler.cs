using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class NetoEmergencyVFXHandler : MonoBehaviour
{
    
    
    [SerializeField] private Transform particleEndpointPosition;
    [SerializeField] private Transform attractor;
    [SerializeField] private Transform spawnPoint;
    [SerializeField]private HandleNetoRayMovement handleNetoRayMovement;
    
    private VisualEffect effect;
    private float stripsLifetime;
    
    private bool shouldMove = false; // To control when the Attractor object should start moving
    private Vector3 particleDirection;
    private float attractorSpeed = 15f;

    private bool emergencyActive = false;
    
    
    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        stripsLifetime = effect.GetFloat("StripsLifetime");
        
        /*gameObject.transform.rotation = Quaternion.LookRotation(particleEndpointPosition.position - transform.position);
        attractor.rotation = Quaternion.LookRotation(particleEndpointPosition.position - attractor.position);*/
        
        
        handleNetoRayMovement.OnEmergencyStatusChanged += UpdateEmergencyStatus;
        handleNetoRayMovement.OnEmergencyStatusChanged += GrowVines;

    }
    
    
    
    private void Update()
    {
        if(emergencyActive)
        {
            MoveAttractor();
        }
    }
    
    
    
    private void GrowVines(bool hasEmergency)
    {
        if (hasEmergency)
        {
            if(attractor.position != spawnPoint.transform.position)
            {
                ResetInitialPosition();
            }
            
            // Clear the particles emitted by the VisualEffect object
            effect.Stop();
            //effect.SetFloat("StripsLifetime", stripsLifetime);
            effect.Reinit();
            
            effect.SetVector3("SpawnPosition", spawnPoint.transform.position);
            attractor.position = spawnPoint.transform.position;
            SetAttractorDirection();
            

            effect.SendEvent("VinesEffectPlay");
            shouldMove = true;
            
            
            
        }
        else
        {
            effect.Stop();
            effect.Reinit();
            ResetInitialPosition();
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
        attractor.position = spawnPoint.transform.position;
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
    
    
    private void OnDestroy()
    {
        handleNetoRayMovement.OnEmergencyStatusChanged -= UpdateEmergencyStatus;
    }
    
    
    private void UpdateEmergencyStatus(bool hasEmergency)
    {
        emergencyActive = hasEmergency;
    }
    
    
}
