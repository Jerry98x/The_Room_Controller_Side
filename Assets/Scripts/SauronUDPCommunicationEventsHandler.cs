using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles the creation of the message to send to the Sauron endpoint via UDP.
/// </summary>
public class SauronUDPCommunicationEventsHandler : MonoBehaviour
{
    
    
    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private Transform roomCore;
    [SerializeField] private Transform rayEndPoint;
    [SerializeField] private HandleSauronRayMovementV2 sauronRayMovementHandler;
    [SerializeField] private SauronFeedbackHandler sauronFeedbackHandler;
    

    
    private int rotationAlphaAngleSauron;
    private int elevationBetaAngleSauron;
    
    private Renderer rayRenderer;
    
    
        
    // The messages are the bytes that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, to use as little memory as possible.
    
    // Sauron module parameters
    // The message is a concatenation of the following values:
    // 1) A number representing the angle of the servo motor that controls the rotation of the Sauron module around the vertical axis
    // 2) A number representing the angle of the servo motor that controls the inclination of the Sauron module
    // 3) The termination character '\0'
    
    
    
    private string message;
    private byte[] messageBytes;
    
    private int[] lastMessage;
    
    
    private EndPointSO endPointSO;



    private void Start()
    {
        
        
        SpiralwaveRay deselectedSpiralwaveRay = roomElement.GetComponentInChildren<SpiralwaveRay>();
        
        if(deselectedSpiralwaveRay)
        {
            SpiralwaveRay selectedSpiralwaveRay = deselectedSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            rayRenderer = selectedSpiralwaveRay.GetComponent<Renderer>();
        
            endPointSO = roomElement.GetEndPointSO();
        }
        
        lastMessage = new int[2];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
        
        
    }
    
    
    /// <summary>
    /// Checks if the Controller is in control of the Sauron and builds the message to send to the ESP32.
    /// </summary>
    private void Update()
    {
        Debug.Log("About to send a message to the ESP32!");
        
        
        // It's easier to send messages to the ESP32 at every frame, since a relatively
        // big amount of messages can be handled on the other side. No need to just send them
        // when some characteristics change!
        
        if(sauronRayMovementHandler.IsInControl())
        {
           
            BuildByteArrayMessage();
            Debug.Log("Byte message to be sent: " + messageBytes);
            UDPManager.Instance.SendByteArrayUdp(messageBytes, endPointSO.EndPoint);
            
            
            Debug.Log("Message sent!");
        }
        
    }
    
    
    /// <summary>
    /// Retrieve the relevant values and builds the byte array message to send to the ESP32 by converting the parameters into bytes.
    /// </summary>
    private void BuildByteArrayMessage()
    {
        
        rotationAlphaAngleSauron = (int)sauronRayMovementHandler.GetAlphaRotationAngle();
        elevationBetaAngleSauron = (int)sauronRayMovementHandler.GetBetaElevationAngle();

        if (lastMessage[0] == rotationAlphaAngleSauron && lastMessage[1] == elevationBetaAngleSauron)
        {
            return;
        }
        
        lastMessage[0] = rotationAlphaAngleSauron;
        lastMessage[1] = elevationBetaAngleSauron;
        
        Debug.Log("SENDING SAURON VALUE rotation servo angle: " + rotationAlphaAngleSauron);
        Debug.Log("SENDING SAURON VALUE inclination servo angle: " + elevationBetaAngleSauron);
        
        // Convert each parameter into bytes
        byte rotationServoAngleByteSauron = (byte)rotationAlphaAngleSauron;
        byte inclinationServoAngleByteSauron = (byte)elevationBetaAngleSauron;
        
        Debug.Log("SENDING SAURON BYTE rotation servo angle: " + rotationServoAngleByteSauron);
        Debug.Log("SENDING SAURON BYTE inclination servo angle: " + inclinationServoAngleByteSauron);
        
        // Concatenate all the bytes and leave space for the termination character
        messageBytes = new byte[3];
        messageBytes[0] = rotationServoAngleByteSauron;
        messageBytes[1] = inclinationServoAngleByteSauron;
        
        // Add the termination character
        messageBytes[messageBytes.Length - 1] = 0;
        
        string result = System.Text.Encoding.ASCII.GetString(messageBytes);
        Debug.Log("Sauron byte array: " + result);
    }
    
    
    /// <summary>
    /// Builds the final message to send to the ESP32 when the application is closed.
    /// </summary>
    private void SendFinalMessage()
    {
        // Define initial values
        int initialRotationServoAngle = 90;
        int initialInclinationServoAngle = 30;

        // Convert each parameter into bytes
        byte rotationServoAngleByte = (byte)initialRotationServoAngle;
        byte inclinationServoAngleByte = (byte)initialInclinationServoAngle;

        // Concatenate the values to be sent in the byte array
        byte[] finalMessageBytes = new byte[3];
        finalMessageBytes[0] = rotationServoAngleByte;
        finalMessageBytes[1] = inclinationServoAngleByte;

        // Add the termination character
        finalMessageBytes[finalMessageBytes.Length - 1] = 0;

        // Send the final message
        UDPManager.Instance.SendByteArrayUdp(finalMessageBytes, endPointSO.EndPoint);
    }

    private void OnApplicationQuit()
    {
        SendFinalMessage();
    }
    
    
    
}
