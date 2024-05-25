using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetoSoundManager : MonoBehaviour
{
    
    
    //[SerializeField] private AudioClip soundEffect;
    [SerializeField] private AudioSource audioSource;
    
    private float volumeScale = 1.0f;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; // Make the sound 3D
    }

    public void PlaySoundEffect()
    {
        audioSource.PlayOneShot(audioSource.clip, volumeScale);
    }
    
    
    
    
}
