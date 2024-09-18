using System;
using System.Collections;
using System.Collections.Generic;
using Oasis.GameEvents;
using UnityEngine;

public class NetoUDPCommunicationEventsHandler : MonoBehaviour
{
    
    [SerializeField] private RoomBasicElement roomElement;
    [SerializeField] private Transform roomCore;
    [SerializeField] private Transform rayEndPoint;
    [SerializeField] private HandleNetoRayMovement netoRayMovementHandler;
    [SerializeField] private StringGameEventSO netoPositionChangeEvent;
    [SerializeField] private NetoFeedbackHandler netoFeedbackHandler;
    
    
    

    //private RayEndPoint rayEndPointObject;
    private ParticleSystem partSystem;
    private AudioSource audioSource;
    private Renderer rayRenderer;
    private ScaleParticleSystemFromMicrophone loudnessDetector;
    
    
    // The messages are the bytes that will be sent to the ESP32 via UDP. It won't use
    // key-value pairs, to use as little memory as possible.
    
    // Neto module parameters
    // The message is a concatenation of the following values:
    // 1) A number representing the type of sound emitted by the ESP32 of the Neto module
    // 2) A number representing the volume of the sound emitted by the ESP32 of the Neto module
    // 3) A number representing the angle of the servo motor that controls the Neto module, determined by the depth of the Neto ray in the application
    // 4) A number called "radius" representing the amount of leds that should be turned on in the ESP32 of the Neto module
    // 5) A number representing the brightness of the leds that should be turned on in the ESP32 of the Neto module
    // 6) The termination character '\0'
    
    private int soundTypeNeto;
    private int soundVolumeNeto;
    private int servoAngleNeto;
    private int radiusNeto;
    private int brightnessNeto;

        
    private string message;
    private byte[] messageBytes;
    
    private int[] lastMessage;
    
    
    private EndPointSO endPointSO;
    
    
    
    private void Start()
    {
        // First particle system in the hierarchy, don't move!
        partSystem = netoFeedbackHandler.GetComponent<ParticleSystem>();
        audioSource = netoFeedbackHandler.GetHandledAudioSource();
        loudnessDetector = roomElement.GetComponentInChildren<RayStartPoint>()
            .GetComponentInChildren<ScaleParticleSystemFromMicrophone>();
        

        SinewaveRay deselectedSinewaveRay = roomElement.GetComponentInChildren<SinewaveRay>();
        
        if(deselectedSinewaveRay)
        {
            SinewaveRay selectedSinewaveRay = deselectedSinewaveRay.transform.GetChild(0).GetComponent<SinewaveRay>();
            rayRenderer = selectedSinewaveRay.GetComponent<Renderer>();
        }
        
        Debug.Log("Renderer: " + rayRenderer);
        
        endPointSO = roomElement.GetEndPointSO();
        
        lastMessage = new int[5];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
        

    }

    private void Update()
    {
        Debug.Log("About to send a message to the ESP32!");
        
        
        // It's easier to send messages to the ESP32 at every frame, since a relatively
        // big amount of messages can be handled on the other side. No need to just send them
        // when some characteristics change!
        
        
        // TODO: If I am not controlling this Neto module, I don't need to send messages to the ESP32

        if (netoRayMovementHandler.IsInControl())
        {
            Debug.Log("In control of Neto " + roomElement.GetEndPointSO().IP);
            
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
        // Sound parameters
        soundTypeNeto = Constants.NETO_SOUND_TYPE_1;
        // Sound volume in Unity is from 0 to 1
        /*soundVolumeNeto = (int) Mathf.Round(RangeRemappingHelper.Remap(audioSource.volume, 1, 0,
            Constants.NETO_SOUND_VOLUME_MAX, Constants.NETO_SOUND_VOLUME_MIN));*/
        float loudnessThreshold = loudnessDetector.GetLoudnessThreshold();
        float loudnessSensibility = loudnessDetector.GetLoudnessSensibility();
        float loudness = loudnessDetector.GetAudioLoudness() * loudnessSensibility;
        if(loudness < loudnessThreshold)
        {
            loudness = 0;
        }
        if(loudness > Constants.MICROPHONE_LOUDNESS_CAP_FOR_NETO)
        {
            loudness = Constants.MICROPHONE_LOUDNESS_CAP_FOR_NETO;
        }

        float targetLoudness = Mathf.Lerp(0, Constants.MICROPHONE_LOUDNESS_CAP_FOR_NETO, loudness);
        soundVolumeNeto = (int) Mathf.Round(RangeRemappingHelper.Remap(targetLoudness, Constants.MICROPHONE_LOUDNESS_CAP_FOR_NETO, 0,
            Constants.NETO_SOUND_VOLUME_MAX, Constants.NETO_SOUND_VOLUME_MIN));
        
        
        
        // Position parameters
        Vector3 coreToEndPointVector = rayEndPoint.position - roomCore.position;
        Vector3 directionFromCoreToEndPoint = coreToEndPointVector.normalized;
        float distanceAlongDirection = Vector3.Dot(directionFromCoreToEndPoint, coreToEndPointVector);
        Debug.Log("DISTANCE ALONG DIRECTION: " + distanceAlongDirection);
        /*SphericalCoordinatesHandler.CartesianToSpherical(endPoint.transform.position, out float rad, out float inc, out float az);
        Debug.Log("ENDPOINT RADIUS: " + rad);*/
        servoAngleNeto = (int) Mathf.Round(RangeRemappingHelper.Remap(distanceAlongDirection, Constants.ENDPOINT_REACH_Z_MAX,
            Constants.ENDPOINT_REACH_Z_MIN, Constants.NETO_SERVO_ANGLE_HIGH, Constants.NETO_SERVO_ANGLE_LOW));
        Debug.Log("FINAL SERVO ANGLE: " + servoAngleNeto);
        
        
        // Light parameters
        // TODO: Understand which dimension of the ray I should use to determine the radius
        radiusNeto = 5;
        
        //float emissionIntensity = renderer.material.GetFloat(Constants.EMISSION_INTENSITY_ID);
        Color color = rayRenderer.material.GetColor(Constants.EMISSIVE_COLOR_ID);
        GetHDRIntensity.DecomposeHdrColor(color, out Color baseLinearColor, out float emissionIntensity);
        Debug.Log("EMISSION INTENSITY: " + emissionIntensity);
        float clampedIntensity = Mathf.Clamp(emissionIntensity, Constants.CAPPED_MIN_EMISSION_INTENSITY,
            Constants.CAPPED_MAX_EMISSION_INTENSITY);
        float remappedIntensity = RangeRemappingHelper.Remap(clampedIntensity, Constants.CAPPED_MAX_EMISSION_INTENSITY,
            Constants.CAPPED_MIN_EMISSION_INTENSITY, Constants.NETO_BRIGHTNESS_MAX, Constants.NETO_BRIGHTNESS_MIN);
        Debug.Log("REMAPPED INTENSITY: " + remappedIntensity);
        brightnessNeto = (int) Mathf.Round(remappedIntensity);
        Debug.Log("FINAL BRIGHTNESS: " + brightnessNeto);
        

    }
    
    


    private void BuildByteArrayMessage()
    {

        if (lastMessage[0] == soundTypeNeto && lastMessage[1] == soundVolumeNeto && lastMessage[2] == servoAngleNeto &&
            lastMessage[3] == radiusNeto && lastMessage[4] == brightnessNeto)
        {
            return;
        }
        
        lastMessage[0] = soundTypeNeto;
        lastMessage[1] = soundVolumeNeto;
        lastMessage[2] = servoAngleNeto;
        lastMessage[3] = radiusNeto;
        lastMessage[4] = brightnessNeto;
        
        
        // Print the values to be sent in the byte array
        Debug.Log("SENDING NETO VALUE sound type: " + soundTypeNeto);
        Debug.Log("SENDING NETO VALUE sound volume: " + soundVolumeNeto);
        Debug.Log("SENDING NETO VALUE servo angle: " + servoAngleNeto);
        Debug.Log("SENDING NETO VALUE radius: " + radiusNeto);
        Debug.Log("SENDING NETO VALUE brightness: " + brightnessNeto);
        
        // Convert each parameter into bytes
        byte soundTypeBytesNeto = (byte)soundTypeNeto;
        byte soundVolumeBytesNeto = (byte)soundVolumeNeto;
        byte servoAngleBytesNeto = (byte)servoAngleNeto;
        byte radiusBytesNeto = (byte)radiusNeto;
        byte brightnessBytesNeto = (byte)brightnessNeto;
        
        Debug.Log("SENDING NETO BYTE sound type: " + soundTypeBytesNeto);
        Debug.Log("SENDING NETO BYTE sound volume: " + soundVolumeBytesNeto);
        Debug.Log("SENDING NETO BYTE servo angle: " + servoAngleBytesNeto);
        Debug.Log("SENDING NETO BYTE radius: " + radiusBytesNeto);
        Debug.Log("SENDING NETO BYTE brightness: " + brightnessBytesNeto);
        
        
        // Concatenate all the bytes and leave space for the termination character
        messageBytes = new byte[6];
        messageBytes[0] = soundTypeBytesNeto;
        messageBytes[1] = soundVolumeBytesNeto;
        messageBytes[2] = servoAngleBytesNeto;
        messageBytes[3] = radiusBytesNeto;
        messageBytes[4] = brightnessBytesNeto;
        
        // Add the termination character
        messageBytes[messageBytes.Length - 1] = 0;
        

        
        string result = System.Text.Encoding.ASCII.GetString(messageBytes);
        Debug.Log("Neto byte array: " + result);
        
    }
    
    
    
    
    private void SendFinalMessage()
    {
        // Define initial values
        int initialSoundType = 1;
        int initialSoundVolume = 0;
        int initialServoAngle = 0;
        int initialRadius = 0;
        int initialBrightness = 0;

        // Convert each parameter into bytes
        byte soundTypeByte = (byte)initialSoundType;
        byte soundVolumeByte = (byte)initialSoundVolume;
        byte servoAngleByte = (byte)initialServoAngle;
        byte radiusByte = (byte)initialRadius;
        byte brightnessByte = (byte)initialBrightness;

        // Concatenate the values to be sent in the byte array
        byte[] finalMessageBytes = new byte[6];
        finalMessageBytes[0] = soundTypeByte;
        finalMessageBytes[1] = soundVolumeByte;
        finalMessageBytes[2] = servoAngleByte;
        finalMessageBytes[3] = radiusByte;
        finalMessageBytes[4] = brightnessByte;

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
