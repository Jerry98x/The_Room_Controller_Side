using System;
using System.Collections;
using System.Collections.Generic;
using Oasis.GameEvents;
using UnityEngine;

public class UDPCommunicationEventsHandler : MonoBehaviour
{
    // Events
    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private Transform roomCore;
    [SerializeField] private StringGameEventSO netoPositionChangeEvent;
    [SerializeField] private NetoFeedbackHandler netoFeedbackHandler;
    
    private EndPoint endPoint;
    private ParticleSystem particleSystem;
    private AudioSource audioSource;
    private Renderer renderer;
    

    // The message is the string that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, but a simple string, to use as little memory as possible.
    // The string is a concatenation of the following values:
    // 1) A number representing the type of sound emitted by the ESP32 of the Neto module
    // 2) A number representing the volume of the sound emitted by the ESP32 of the Neto module
    // 3) A number representing the angle of the servo motor that controls the Neto module, determined by the depth of the Neto ray in the application
    // 4) A number called "radius" representing the amount of leds that should be turned on in the ESP32 of the Neto module
    // 5) A number representing the brightness of the leds that should be turned on in the ESP32 of the Neto module
    // 6) The termination character '\0'
    
    private int soundType;
    private int soundVolume;
    private int servoAngle;
    private int radius;
    private int brightness;

        
    private string message;
    private byte[] messageBytes;
    private int[] messageInt;
    
    
    private EndPointSO endPointSO;
    
    
    
    private void Start()
    {
        endPoint = GetComponent<EndPoint>();
        particleSystem = netoFeedbackHandler.GetComponent<ParticleSystem>();
        audioSource = netoFeedbackHandler.GetHandledAudioSource();

        SinewaveRay deselectedSinewaveRay = roomElement.GetComponentInChildren<SinewaveRay>();
        SpiralwaveRay deselectedSpiralwaveRay = roomElement.GetComponentInChildren<SpiralwaveRay>();
        
        if(deselectedSinewaveRay)
        {
            SinewaveRay selectedSinewaveRay = deselectedSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            renderer = selectedSinewaveRay.GetComponent<Renderer>();
        }
        if(deselectedSpiralwaveRay)
        {
            SpiralwaveRay selectedSpiralwaveRay = deselectedSpiralwaveRay.transform.GetChild(0).GetComponent<SpiralwaveRay>();
            renderer = selectedSpiralwaveRay.GetComponent<Renderer>();
        }
        Debug.Log("Renderer: " + renderer);
        
        endPointSO = roomElement.GetEndPointSO();
        

    }

    private void Update()
    {
        Debug.Log("About to send a message to the ESP32!");
        
        
        // It's easier to send messages to the ESP32 at every frame, since a relatively
        // big amount of messages can be handled on the other side. No need to just send them
        // when some characteristics change!
        
        
        // TODO: If I am not controlling this Neto module, I don't need to send messages to the ESP32
        
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
    
    private void HandleValuesToSend()
    {
        HandleNetoParametersToSend();
        HandleSauronParametersToSend();

    }


    private void HandleNetoParametersToSend()
    {
        // Sound parameters
        soundType = Constants.NETO_SOUND_TYPE_1;
        // Sound volume in Unity is from 0 to 1
        soundVolume = (int) Mathf.Round(RangeRemappingHelper.Remap(audioSource.volume, 1, 0,
            Constants.NETO_SOUND_VOLUME_MAX, Constants.NETO_SOUND_VOLUME_MIN));
        
        
        // Position parameters
        Vector3 coreToEndPointVector = endPoint.transform.position - roomCore.position;
        Vector3 directionFromCoreToEndPoint = coreToEndPointVector.normalized;
        float distanceAlongDirection = Vector3.Dot(directionFromCoreToEndPoint, coreToEndPointVector);
        /*SphericalCoordinatesHandler.CartesianToSpherical(endPoint.transform.position, out float rad, out float inc, out float az);
        Debug.Log("ENDPOINT RADIUS: " + rad);*/
        servoAngle = (int) Mathf.Round(RangeRemappingHelper.Remap(distanceAlongDirection, Constants.ENDPOINT_REACH_Z_MAX,
            Constants.ENDPOINT_REACH_Z_MIN, Constants.NETO_SERVO_ANGLE_MAX, Constants.NETO_SERVO_ANGLE_MIN));
        Debug.Log("FINAL SERVO ANGLE: " + servoAngle);
        
        
        // Light parameters
        // TODO: Understand which dimension of the ray I should use to determine the radius
        radius = 5;
        
        //float emissionIntensity = renderer.material.GetFloat(Constants.EMISSION_INTENSITY_ID);
        Color color = renderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
        GetHDRIntensity.DecomposeHdrColor(color, out Color baseLinearColor, out float emissionIntensity);
        Debug.Log("EMISSION INTENSITY: " + emissionIntensity);
        float clampedIntensity = Mathf.Clamp(emissionIntensity, Constants.CAPPED_MIN_EMISSION_INTENSITY,
            Constants.CAPPED_MAX_EMISSION_INTENSITY);
        float remappedIntensity = RangeRemappingHelper.Remap(clampedIntensity, Constants.CAPPED_MAX_EMISSION_INTENSITY,
            Constants.CAPPED_MIN_EMISSION_INTENSITY, Constants.NETO_BRIGHTNESS_MAX, Constants.NETO_BRIGHTNESS_MIN);
        Debug.Log("REMAPPED INTENSITY: " + remappedIntensity);
        brightness = (int) Mathf.Round(remappedIntensity);
        Debug.Log("FINAL BRIGHTNESS: " + brightness);
    }

    private void HandleSauronParametersToSend()
    {
        
    }


    private void BuildStringMessage()
    {
        message = "";
        message += soundType.ToString();
        message += soundVolume.ToString();
        message += servoAngle.ToString();
        message += radius.ToString();
        message += brightness.ToString();
        message += Constants.TERMINATION_CHARACTER;
        
    }



    private void BuildByteArrayMessage()
    {
        // Convert each parameter to bytes
        byte soundTypeBytes = (byte)soundType;
        byte soundVolumeBytes = (byte)soundVolume;
        byte servoAngleBytes = (byte)servoAngle;
        byte radiusBytes = (byte)radius;
        byte brightnessBytes = (byte)brightness;
        
        // Concatenate all the bytes and leave space for the termination character
        messageBytes = new byte[6];
        messageBytes[0] = soundTypeBytes;
        messageBytes[1] = soundVolumeBytes;
        messageBytes[2] = servoAngleBytes;
        messageBytes[3] = radiusBytes;
        messageBytes[4] = brightnessBytes;
        
        // Add the termination character
        messageBytes[messageBytes.Length - 1] = 0;
        
        string result = System.Text.Encoding.ASCII.GetString(messageBytes);
        Debug.Log("Byte array: " + result);
        
    }


    private void BuildIntMessage()
    {
        messageInt = new int[5];
        messageInt[0] = soundType;
        messageInt[1] = soundVolume;
        messageInt[2] = servoAngle;
        messageInt[3] = radius;
        messageInt[4] = brightness;
        
        
    }
    
    
}
