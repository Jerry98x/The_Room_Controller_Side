using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauronUDPCommunicationEventsHandler : MonoBehaviour
{
    
    
    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private Transform roomCore;
    [SerializeField] private Transform rayEndPoint;
    [SerializeField] private HandleSauronRayMovementV2 sauronRayMovementHandler;
    [SerializeField] private SauronFeedbackHandler sauronFeedbackHandler;
    
    
        
    // TODO: Remove these as soon as I've figured out a mapping between the position of the Sauron ray endpoint and the two servo angles values
    [SerializeField, Range(0, 180)] private int rotationServoAngleSauron;
    [SerializeField, Range(0, 60)] private int inclinationServoAngleSauron;

    
    private int rotationAlphaAngleSauron;
    private int elevationBetaAngleSauron;
    
    
    //private RayEndPoint rayEndPointObject;
    private Renderer rayRenderer;
    
    
        
    // The messages are the bytes that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, to use as little memory as possible.
    
    // Sauron module parameters
    // The message is a concatenation of the following values:
    // 1) A number representing the angle of the servo motor that controls the rotation of the Sauron module around the vertical axis
    // 2) A number representing the angle of the servo motor that controls the inclination of the Sauron module
    // 3) The termination character '\0'
    
    /*private int rotationServoAngleSauron;
    private int inclinationServoAngleSauron;*/
    
    
    
    private string message;
    private byte[] messageBytes;
    private int[] messageInt;
    
    
    private EndPointSO endPointSO;



    private void Start()
    {
        
        
        SpiralwaveRay deselectedSpiralwaveRay = roomElement.GetComponentInChildren<SpiralwaveRay>();
        
        if(deselectedSpiralwaveRay)
        {
            SpiralwaveRay selectedSpiralwaveRay = deselectedSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            rayRenderer = selectedSpiralwaveRay.GetComponent<Renderer>();
            
            Debug.Log("Renderer: " + rayRenderer);
        
            endPointSO = roomElement.GetEndPointSO();
        }
        
        
    }
    
    
    
    private void Update()
    {
        Debug.Log("About to send a message to the ESP32!");
        
        
        // It's easier to send messages to the ESP32 at every frame, since a relatively
        // big amount of messages can be handled on the other side. No need to just send them
        // when some characteristics change!
        
        
        // TODO: If I am not controlling this Sauron module, I don't need to send messages to the ESP32
        
        if(sauronRayMovementHandler.IsInControl())
        {
            Debug.Log("In control of Sauron " + roomElement.GetEndPointSO().IP);
            
            HandleValuesToSend();
                    
            //BuildStringMessage();
            //Debug.Log("Message to be sent: " + message);
            //UDPManager.Instance.SendStringUdp(message, endPointSO.EndPoint);
            
            
            BuildByteArrayMessage();
            Debug.Log("Byte message to be sent: " + messageBytes);
            UDPManager.Instance.SendByteArrayUdp(messageBytes, endPointSO.EndPoint);
            
            
            //BuildIntMessage();
            //Debug.Log("Int message to be sent: " + messageInt);
            //UDPManager.Instance.SendIntUdp(messageInt, endPointSO.EndPoint);
            
            
            Debug.Log("Message sent!");
        }
        
        
    }
    
    
    
    private void HandleValuesToSend()
    {
        // Position parameters
        
    }
    
    
    private void BuildByteArrayMessage()
    {
        
        rotationAlphaAngleSauron = (int)sauronRayMovementHandler.GetAlphaRotationAngle();
        elevationBetaAngleSauron = (int)sauronRayMovementHandler.GetBetaElevationAngle();
        
        Debug.Log("SENDING SAURON VALUE rotation servo angle: " + rotationAlphaAngleSauron);
        Debug.Log("SENDING SAURON VALUE inclination servo angle: " + elevationBetaAngleSauron);
        
        // Convert each parameter into bytes
        byte rotationServoAngleByteSauron = (byte)rotationServoAngleSauron;
        byte inclinationServoAngleByteSauron = (byte)inclinationServoAngleSauron;
        
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
    
    
    
}
