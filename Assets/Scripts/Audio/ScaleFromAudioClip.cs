using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Applies a scale to the object based on the loudness of an audio clip
/// </summary>
public class ScaleFromAudioClip : MonoBehaviour
{

    [SerializeField] private AudioSource source;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private AudioLoudnessDetection loudnessDetector;
    
    [SerializeField] private float loudnessSensibility = 100f;
    [SerializeField] private float threshold = 0.1f;


    private void Update()
    {
        float loudness = loudnessDetector.GetLoudnessFromAudioClip(source.timeSamples, source.clip) * loudnessSensibility;
        
        if(loudness < threshold)
        {
            loudness = 0;
        }
        
        // Lerping the scale based on the loudness
        transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
    }
}
