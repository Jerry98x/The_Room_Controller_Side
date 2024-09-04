using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathtrapUDPCommunicationEventsHandler : MonoBehaviour
{
    
    public event Action<int> OnPetalsOpeningChanged;
    public event Action<int> OnBadSmellEmittingChanged;

    

    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private Transform testingSphere;
    
    
    // The messages are the bytes that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, to use as little memory as possible.
    
    // Deathtrap module parameters
    // The message is a concatenation of the following values (for simplicity I shouldn't use booleans):
    // 1) An integer representing the action of the deathtrap of spraying (0 = not spraying, 1 = spraying)
    // 2) An integer representing both the action of the Deathtrap petals (0 = closing, 1 = opening)
    // and the action of the LEDs (0 = turning off 1 = turning on)
    // 2) An integer representing the action of the Deathtrap bad smell emission (0 = not emitting, 1 = emitting)


    //[SerializeField] private int spraying;
    [SerializeField, Range(0,1)] private int liquidSpraying;
    [SerializeField, Range(60,105)] private int petalsOpening;
    [SerializeField, Range(0,1)] private int badSmellEmitting;
    
    
    private string message;
    private byte[] messageBytes;
    
    private int[] lastMessage;
    
    
    private EndPointSO endPointSO;


    // Testing the UDP communication with the Deathtrap module
    // Use a sphere to see whether the two operations I can do with the Deathtrap module are working
    // (will switch to actual Deathtrap module operations later)

    // 1a) Real operation: opening/closing of the Deathtrap petals + LEDs turning on/off (two tied sub-operations)
    // 1b) Testing operation: move the sphere away from the camera / towards the camera
    // 2a) Real operation: emission of the bad smell
    // 2b) Testing operation: change the color gradient of the sphere



    // TODO: implement the actual operations to do at start time with the Deathtrap module
    private void Start()
    {
        
        endPointSO = roomElement.GetEndPointSO();

        lastMessage = new int[3];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
        
        
        
        // Event subscription
        OnPetalsOpeningChanged?.Invoke(petalsOpening);
        OnBadSmellEmittingChanged?.Invoke(badSmellEmitting);

    }
    
    
    private void Update()
    {
        Debug.Log("About to send a message to the ESP32!");
        
        
        // It's easier to send messages to the ESP32 at every frame, since a relatively
        // big amount of messages can be handled on the other side. No need to just send them
        // when some characteristics change!
        
        
        HandleValuesToSend();
        
        
        BuildByteArrayMessage();
        Debug.Log("Byte message to be sent: " + messageBytes);
        UDPManager.Instance.SendByteArrayUdp(messageBytes, endPointSO.EndPoint);
        
       
        Debug.Log("Message sent!");
    }


    private void HandleValuesToSend()
    {
        
        
    }

    private void BuildByteArrayMessage()
    {
        if(lastMessage[0] == liquidSpraying && lastMessage[1] == petalsOpening && lastMessage[2] == badSmellEmitting)
        {
            return;
        }
        
        lastMessage[0] = liquidSpraying;
        lastMessage[1] = petalsOpening;
        lastMessage[2] = badSmellEmitting;
        
        
        // Print the values to be sent in the byte array
        Debug.Log("SENDING DEATHTRAP VALUE liquid spraying: " + liquidSpraying);
        Debug.Log("SENDING DEATHTRAP VALUE petals opening: " + petalsOpening);
        Debug.Log("SENDING DEATHTRAP VALUE bad smell emitting: " + badSmellEmitting);
        
        // Convert each parameter into bytes
        byte liquidSprayingByte = System.Convert.ToByte(liquidSpraying);
        byte petalsOpeningByte = System.Convert.ToByte(petalsOpening);
        byte badSmellEmittingByte = System.Convert.ToByte(badSmellEmitting);
        
        Debug.Log("SENDING DEATHTRAP BYTE liquid spraying: " + liquidSprayingByte);
        Debug.Log("SENDING DEATHTRAP BYTE petals opening byte: " + petalsOpeningByte);
        Debug.Log("SENDING DEATHTRAP BYTE bad smell emitting byte: " + badSmellEmittingByte);
        
        
        // Concatenate the values to be sent in the byte array
        messageBytes = new byte[4];
        messageBytes[0] = liquidSprayingByte;
        messageBytes[1] = petalsOpeningByte;
        messageBytes[2] = badSmellEmittingByte;
        
        // Add the termination character
        messageBytes[messageBytes.Length - 1] = 0;
        
        
        string result = System.Text.Encoding.ASCII.GetString(messageBytes);
        Debug.Log("Resulting message: " + result);
        
    }
    
    
    
    
    
    
    
    /*
    public int PetalsOpening
    {
        get => petalsOpening;
        set
        {
            if (petalsOpening != value)
            {
                petalsOpening = value;
                OnPetalsOpeningChanged?.Invoke(petalsOpening);
            }
        }
    }

    public int BadSmellEmitting
    {
        get => badSmellEmitting;
        set
        {
            if (badSmellEmitting != value)
            {
                badSmellEmitting = value;
                OnBadSmellEmittingChanged?.Invoke(badSmellEmitting);
            }
        }
    }
    */
    
    
    private void OnValidate()
    {
        OnPetalsOpeningChanged?.Invoke(petalsOpening);
        OnBadSmellEmittingChanged?.Invoke(badSmellEmitting);
    }

}
