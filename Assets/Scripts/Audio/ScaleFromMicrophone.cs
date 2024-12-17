using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies a scale to an object based on the loudness of the microphone input
/// </summary>
public class ScaleFromMicrophone : MonoBehaviour
{

    [SerializeField] private AudioSource source;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private AudioLoudnessDetection loudnessDetector;
    
    [SerializeField] private float loudnessSensibility = 100f;
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float smoothingSpeed = 5f;

    
    private Vector3 targetScale;
    
    private void Start()
    {
        targetScale = transform.localScale;
    }

    private void Update()
    {
        float loudness = loudnessDetector.GetLoudnessFromMicrophone() * loudnessSensibility;
        
        if(loudness < threshold)
        {
            loudness = 0;
        }
        
        // Calculate the target scale based on loudness
        targetScale = Vector3.Lerp(minScale, maxScale, loudness);

        
        // Smoothly interpolate the object's scale towards the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothingSpeed);
    }
}
