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
    private Vector3 initialPosition;
    
    /// <summary>
    /// Initializes the initial position of the Neto element
    /// </summary>
    protected override void Start()
    {
        base.Start();
        
        initialPosition = transform.position;
        Debug.Log("PORCODIO: " + endPointSO.EndPoint);
    }
    
    
    
    protected override void ExecuteMessageResponse(string message)
    {
        Debug.Log("Neto element received message: " + message);
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
    /// Returns the initial position of the Neto element
    /// </summary>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
}
