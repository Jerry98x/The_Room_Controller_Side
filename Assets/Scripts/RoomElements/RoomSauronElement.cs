using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a virtual Sauron element in the room
/// </summary>
/// <remarks>
/// Applied to the more general objects that represent a Sauron element in the room.
/// Used mainly for handling the events related to the Sauron element after receiving messages from the physical Sauron module.
/// </remarks>
public class RoomSauronElement : RoomBasicElement
{

    [SerializeField] private SauronFeedbackHandler sauronFeedbackHandler;

    private Vector3 initialPosition;
    
    /// <summary>
    /// Initializes the initial position of the Sauron element
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
    /// Split the message received from the Sauron's endpoint and execute the corresponding actions.
    /// Save the current message content for later comparison.
    /// </summary>
    /// <param name="message"> Message received from the Sauron's endpoint </param>
    protected override void ExecuteMessageResponse(string message)
    {
        // Executed when the message is specifically related to the Sauron element
        Debug.Log("Sauron element received message: " + message);
        
        
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

        if (lastMessage[0] != messageContent[0])
        {
            TriggerSauronEffects(messageContent[0]);
        }
        else
        {
            if (sauronFeedbackHandler.IsEffectPlaying() && messageContent[0] > 0)
            {
                float deltaTimeToAdd = 0.03f;
                
                sauronFeedbackHandler.IncreaseParticlesLifetime(deltaTimeToAdd);
            }
        }
        
        lastMessage[0] = messageContent[0];
        lastMessage[1] = messageContent[1];
        
    }
    
    protected override void ExecuteMessageResponse(byte[] message)
    {
        // Executed when the message is specifically related to the Sauron element
        Debug.Log("Sauron element received message: " + message);
    }
    
    protected override void ExecuteMessageResponse(char[] message)
    {
        // Executed when the message is specifically related to the Sauron element
        Debug.Log("Sauron element received message: " + message);
    }
    
    
    
    /// <summary>
    /// Triggers the Sauron effects based on the intensity of the touch
    /// </summary>
    /// <param name="touchIntensity"> Intensity of the touch </param>
    private void TriggerSauronEffects(int touchIntensity)
    {

        // The value of the intensity has changes since the last message

        if (touchIntensity > 0)
        {
            sauronFeedbackHandler.SetTouchCheck(true);
            // Someone touched the Sauron element
            if (!sauronFeedbackHandler.IsEffectPlaying() && !sauronFeedbackHandler.GetHumanSilhouette().activeSelf)
            {
                sauronFeedbackHandler.HandleSilhouetteAndVinesEffects();
            }
            
        }
        else
        {
            sauronFeedbackHandler.ResetParticlesLifetime();
            sauronFeedbackHandler.SetTouchCheck(false);
        }
        
    }
    
    
    
    /// <summary>
    /// Returns the initial position of the Sauron element
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
    
    
}
