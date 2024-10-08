using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class DeathtrapUDPCommunicationEventsHandler : MonoBehaviour
{
    
    public event Action<int> OnPetalsOpeningChanged;
    public event Action<int> OnBadSmellEmittingChanged;

    

    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private FeedbackSphere feedbackSphere;
    [SerializeField] private BadSmellSphere badSmellSphere;

    private SphereCollider testingSphereCollider;

    private bool isInControl = false;
    
    
    // The messages are the bytes that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, to use as little memory as possible.
    
    // Deathtrap module parameters
    // The message is a concatenation of the following values (for simplicity I shouldn't use booleans):
    // 1) An integer representing the spraying action of the Deathtrap (0 = not spraying, 1 = spraying)
    // 2) An integer representing the opening action of the Deathtrap's petals (0 = closing, 1 = opening)
    // 3) An integer representing the bad smell emission action of the Deathtrap (0 = not emitting, 1 = emitting)
    // 4) An integer representing the brightness emission action of the LEDs (0 = off 255 = maximum brightness)


    //[SerializeField] private int spraying;
    [SerializeField, Range(0,1)] private int liquidSpraying;
    [SerializeField, Range(40,90)] private int petalsOpening;
    [SerializeField, Range(0,1)] private int badSmellEmitting;
    [SerializeField, Range(0,255)] private int ledsBrightness;
    
    
    private int liquidSprayingTest;
    private int petalsOpeningTest;
    private int badSmellEmittingTest;
    private int ledsBrightnessTest;
    
    
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
        testingSphereCollider = feedbackSphere.GetComponent<SphereCollider>();
        
        endPointSO = roomElement.GetEndPointSO();

        lastMessage = new int[4];
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


        if (feedbackSphere.IsInControl())
        {
         
            // It's easier to send messages to the ESP32 at every frame, since a relatively
            // big amount of messages can be handled on the other side. No need to just send them
            // when some characteristics change!
        
        
            HandleValuesToSend();
        
        
            BuildByteArrayMessage();
            Debug.Log("Byte message to be sent: " + messageBytes);
            UDPManager.Instance.SendByteArrayUdp(messageBytes, endPointSO.EndPoint);
        
       
            Debug.Log("Message sent!");
            
        }
        
        
    }
    
    
    


    private void HandleValuesToSend()
    {
        
        // Momentarily used to get the buttons and triggers values to send to the ESP32
        liquidSprayingTest = feedbackSphere.GetLiquidSprayingTest();
        petalsOpeningTest = feedbackSphere.GetPetalsOpeningTest();
        badSmellEmittingTest = feedbackSphere.GetBadSmellEmittingTest();
        ledsBrightnessTest = feedbackSphere.GetLedsBrightnessTest();
        
        
    }

    private void BuildByteArrayMessage()
    {
        if(lastMessage[0] == liquidSprayingTest && lastMessage[1] == petalsOpeningTest && lastMessage[2] == badSmellEmittingTest && lastMessage[3] == ledsBrightnessTest)
        {
            return;
        }
        
        /*lastMessage[0] = liquidSpraying;
        lastMessage[1] = petalsOpening;
        lastMessage[2] = badSmellEmitting;
        lastMessage[3] = ledsBrightness;*/

        lastMessage[0] = liquidSprayingTest;
        lastMessage[1] = petalsOpeningTest;
        lastMessage[2] = badSmellEmittingTest;
        lastMessage[3] = ledsBrightnessTest;
        
        
        // Print the values to be sent in the byte array
        Debug.Log("SENDING DEATHTRAP VALUE liquid spraying: " + liquidSprayingTest);
        Debug.Log("SENDING DEATHTRAP VALUE petals opening: " + petalsOpeningTest);
        Debug.Log("SENDING DEATHTRAP VALUE bad smell emitting: " + badSmellEmittingTest);
        Debug.Log("SENDING DEATHTRAP VALUE LEDs brightness: " + ledsBrightnessTest);
        
        // Convert each parameter into bytes
        byte liquidSprayingByte = System.Convert.ToByte(liquidSprayingTest);
        byte petalsOpeningByte = System.Convert.ToByte(petalsOpeningTest);
        byte badSmellEmittingByte = System.Convert.ToByte(badSmellEmittingTest);
        byte ledsBrightnessByte = System.Convert.ToByte(ledsBrightnessTest);
        
        Debug.Log("SENDING DEATHTRAP BYTE liquid spraying: " + liquidSprayingByte);
        Debug.Log("SENDING DEATHTRAP BYTE petals opening byte: " + petalsOpeningByte);
        Debug.Log("SENDING DEATHTRAP BYTE LEDs brightness: " + ledsBrightnessByte);
        
        
        // Concatenate the values to be sent in the byte array
        messageBytes = new byte[5];
        messageBytes[0] = liquidSprayingByte;
        messageBytes[1] = petalsOpeningByte;
        messageBytes[2] = badSmellEmittingByte;
        messageBytes[3] = ledsBrightnessByte;
        
        // Add the termination character
        messageBytes[messageBytes.Length - 1] = 0;
        
        
        string result = System.Text.Encoding.ASCII.GetString(messageBytes);
        Debug.Log("Resulting message: " + result);
        
    }
    
    
    
    
    
    
    private void SendFinalMessage()
    {
        // Define initial values
        int initialLiquidSpraying = 0;
        int initialPetalsOpening = 90;
        int initialBadSmellEmitting = 0;
        int initialLedsBrightness = 0;

        // Convert each parameter into bytes
        byte liquidSprayingByte = System.Convert.ToByte(initialLiquidSpraying);
        byte petalsOpeningByte = System.Convert.ToByte(initialPetalsOpening);
        byte badSmellEmittingByte = System.Convert.ToByte(initialBadSmellEmitting);
        byte ledsBrightnessByte = System.Convert.ToByte(initialLedsBrightness);

        // Concatenate the values to be sent in the byte array
        byte[] finalMessageBytes = new byte[5];
        finalMessageBytes[0] = liquidSprayingByte;
        finalMessageBytes[1] = petalsOpeningByte;
        finalMessageBytes[2] = badSmellEmittingByte;
        finalMessageBytes[3] = ledsBrightnessByte;

        // Add the termination character
        finalMessageBytes[finalMessageBytes.Length - 1] = 0;

        // Send the final message
        UDPManager.Instance.SendByteArrayUdp(finalMessageBytes, endPointSO.EndPoint);
    }

    private void OnApplicationQuit()
    {
        SendFinalMessage();
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
