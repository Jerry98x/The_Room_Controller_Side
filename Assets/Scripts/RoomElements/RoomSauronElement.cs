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
        Debug.Log("PORCODIO: " + receivingEndPointSO.EndPoint);
        
        
        lastMessage = new int[2];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
    }
    
    
    
    protected override void ExecuteMessageResponse(string message)
    {
        // Executed when the message is specifically related to the Sauron element
        Debug.Log("Sauron element received message: " + message);
        
        
        // Split the message string by the colon separator
        string[] parts = message.Split(separator);
        
        
        
        
        for(int i = 0; i < parts.Length; i++)
        {
            Debug.Log("SAURON MESSAGE PART " + i + ": " + parts[i]);
        }
        
        
        
        

        // Initialize the message array with the same length as parts, excluding the first two
        // (the sender type and the specific sender, which are not needed)
        this.messageContent = new int[parts.Length - 2];

        // Convert each part to an integer and store it in the message array
        for (int i = 2; i < parts.Length; i++)
        {
            this.messageContent[i-2] = int.Parse(parts[i]);
            Debug.Log("SAURON MESSAGE CONTENT " + (i-2) + ": " + messageContent[i-2]);
        }

        if (lastMessage[0] != messageContent[0])
        {
            TriggerSauronEffects(messageContent[0]);
        }
        else
        {
            if (sauronFeedbackHandler.IsEffectPlaying())
            {
                float deltaTimeToAdd = 0.3f;
                sauronFeedbackHandler.IncreaseParticlesLifetime(deltaTimeToAdd);
            }
        }
        
        lastMessage[0] = messageContent[0];
        lastMessage[1] = messageContent[1];
        
        
        /*if(lastMessage[0] == messageContent[0] && lastMessage[1] == messageContent[1])
        {
            return;
        }
        
        
        // Apply actions
        TriggerVinesEffect(messageContent[0]);*/
        
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
    
    
    
    private void TriggerSauronEffects(int touchIntensity)
    {

        // The value of the intensity has changes since the last message

        if (touchIntensity > 0)

        {

            // Someone touched the Sauron element

            sauronFeedbackHandler.HandleSilhouetteAndVinesEffects();

        }
        else
        {

            //Someone who was touching the Sauron element has stopped

            

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
