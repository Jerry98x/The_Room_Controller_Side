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

    private Vector3 initialPosition;
    
    /// <summary>
    /// Initializes the initial position of the Sauron element
    /// </summary>
    protected override void Start()
    {
        base.Start();
        
        initialPosition = transform.position;
        Debug.Log("PORCODIO: " + endPointSO.EndPoint);
    }
    
    
    
    protected override void ExecuteMessageResponse(string message)
    {
        // Executed when the message is specifically related to the Sauron element
        Debug.Log("Sauron element received message: " + message);
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
    /// Returns the initial position of the Sauron element
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
    
    
}
