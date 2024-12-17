using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT USED

/// <summary>
/// Manager class that handles the sound effects related to the Neto module in the game
/// </summary>
/// <remarks>
/// Sound effetcs for the Neto are related to the interaction of the Visitor with the Neto module.
/// The physical Neto module has a microphone that captures the sound of the Visitor's voice and that can emit sound.
/// 3D spatial audio is used to provide feedback to the Controller.
/// </remarks>
public class NetoSoundManager : MonoBehaviour
{
    
    
    //[SerializeField] private AudioClip soundEffect;
    [SerializeField] private AudioSource audioSource;
    
    private float volumeScale = 1.0f;

    /// <summary>
    /// Initializes the AudioSource component to have spatial audio
    /// </summary>
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; // Make the sound 3D
    }

    /// <summary>
    /// Plays the sound effect
    /// </summary>
    public void PlaySoundEffect()
    {
        audioSource.PlayOneShot(audioSource.clip, volumeScale);
    }
    
    
}
