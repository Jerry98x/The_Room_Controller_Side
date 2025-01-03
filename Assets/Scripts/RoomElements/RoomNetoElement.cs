using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a virtual Neto element in the room
/// </summary>
/// <remarks>
/// Applied to the more general objects that represent a Neto element in the room.
/// Used mainly for handling the events related to the Neto element after receiving messages from the physical Neto module.
/// </remarks>
public class RoomNetoElement : RoomBasicElement
{

    [SerializeField] private NetoFeedbackHandler netoFeedbackHandler;
    [SerializeField] private HandleNetoRayMovement emergencyModeHandler;
    [SerializeField] private ParticleSystem voiceParticleSystem;
    
    private Vector3 initialPosition;
    
    /// <summary>
    /// Initializes the initial position of the Neto element
    /// </summary>
    protected override void Start()
    {
        base.Start();
        
        initialPosition = transform.position;
        
        
        lastMessage = new int[2];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
    }
    
    
    /// <summary>
    /// Split the message received from the Neto's endpoint and execute the corresponding actions.
    /// Save the current message content for later comparison.
    /// </summary>
    /// <param name="message"> Message received from the Neto's endpoint </param>
    protected override void ExecuteMessageResponse(string message)
    {
        Debug.Log("Neto element received message: " + message);
        
        
        // Split the message string by the colon separator
        string[] parts = message.Split(separator);
        
        

        // Initialize the message array with the same length as parts, excluding the first two
        // (the sender type and the specific sender, which are not needed)
        this.messageContent = new int[parts.Length - 2];

        // Convert each part to an integer and store it in the message array
        for (int i = 2; i < parts.Length; i++)
        {
            this.messageContent[i-2] = int.Parse(parts[i]);
        }
        
        
        
        if(lastMessage[0] < Constants.NETO_MIC_VOLUME_THRESHOLD)
        {
            TriggerNetoEffects(messageContent[0]);
            
        }
        else
        {
            if (messageContent[0] >= Constants.NETO_MIC_VOLUME_THRESHOLD)
            {
               float deltaTimeToAdd = 0.03f;
               // Increase the lifetime of the audio feedback and, if needed, change the volume
               netoFeedbackHandler.IncreaseParticleEffectLifeTime(deltaTimeToAdd);
               netoFeedbackHandler.IncreaseAudioEffectVolume(messageContent[0]); 
            }
            
        }

        if (lastMessage[1] != messageContent[1])
        {
            //CheckEmergencyMode(messageContent[1]);
        }
        
        lastMessage[0] = messageContent[0];
        lastMessage[1] = messageContent[1];

    }
    
    protected override void ExecuteMessageResponse(byte[] message)
    {
        Debug.Log("Neto element received message: " + message);
        Debug.Log("message[0]: " + message[0]);
    }
    
    protected override void ExecuteMessageResponse(char[] message)
    {
        Debug.Log("Neto element received message: " + message);
        Debug.Log("message[0]: " + message[0]);
    }
    
    
    /// <summary>
    /// Triggers the Neto effects based on the microphone intensity
    /// </summary>
    /// <param name="micIntensity"> Intensity value from the microphone </param>
    private void TriggerNetoEffects(int micIntensity)
    {
        if (micIntensity > Constants.NETO_MIC_VOLUME_THRESHOLD)
        {
            netoFeedbackHandler.HandleSilhouetteAndAudioEffects((float)micIntensity);
        }
        
        
    }
    
    
    private void CheckEmergencyMode(int emergencyMode)
    {
        if(emergencyMode > 0)
        {
            emergencyModeHandler.StartEmergencyMode();
        }
    }
    
    
    
    
    /// <summary>
    /// Returns the initial position of the Neto element
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
}
