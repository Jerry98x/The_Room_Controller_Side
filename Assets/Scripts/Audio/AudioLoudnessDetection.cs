using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Detects the loudness of the audio input
/// </summary>
public class AudioLoudnessDetection : MonoBehaviour
{

    public int sampleWindow = 64;

    private AudioClip microphoneClip;
    

    void Start()
    {
        MicrophoneToAudioClip();
    }
    
    /// <summary>
    ///  Stores the audio from the microphone into an AudioClip
    /// </summary>
    public void MicrophoneToAudioClip()
    {
        // Get first microphone
        string microphoneName = Microphone.devices[0];
        Debug.Log("Using microphone: " + microphoneName);
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    
    /// <summary>
    /// Compute the loudness of the audio clip at a given position in the waveform 
    /// </summary>
    /// <param name="clipPosition"> Position within the audioclip </param>
    /// <param name="audioClip"> Audioclip to get the loudness from </param>
    /// <returns></returns>
    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip audioClip)
    {

        int startPosition = clipPosition - sampleWindow;
        
        if(startPosition < 0)
        {
            return 0;
        }
        
        
        float[] waveData = new float[sampleWindow];
        audioClip.GetData(waveData, startPosition);
        
        // Compute loudness
        float totalLoudness = 0;
        
        
        for(int i = 0; i < sampleWindow; i++)
        {
            // Range of the waveform values is [-1, 1], so we take the absolute value
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        
        float averageLoudness = totalLoudness / sampleWindow;
        return averageLoudness;
    }
    
    
    public float GetLoudnessFromMicrophone()
    {
        int clipPosition = Microphone.GetPosition(Microphone.devices[0]);
        return GetLoudnessFromAudioClip(clipPosition, microphoneClip);
    }
    
    
    
    public static bool IsPresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }
    
    
}
