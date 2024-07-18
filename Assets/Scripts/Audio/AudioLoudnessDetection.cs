using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{

    public int sampleWindow = 64;

    private AudioClip microphoneClip;
    

    void Start()
    {
        MicrophoneToAudioClip();
    }

    
    
    public void MicrophoneToAudioClip()
    {
        // Get first microphone
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    
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
        
        return totalLoudness / sampleWindow;
    }
    
    
    public float GetLoudnessFromMicrophone()
    {
        int clipPosition = Microphone.GetPosition(Microphone.devices[0]);
        return GetLoudnessFromAudioClip(clipPosition, microphoneClip);
    }
    
    
}
