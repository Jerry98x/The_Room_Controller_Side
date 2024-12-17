using UnityEngine;

// NOT USED

/// <summary>
/// Class representing a sound (reduced version of the Unity AudioSource)
/// </summary>
[System.Serializable]
public class Sound
{
    // --- Public members
    public string name;
    public AudioClip clip;
    [Range (0f,1f)] public float volume = 0.5f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
    [Range(0, 256)] public int priority = 128;
    [Range(-1f, 1f)] public float panStereo = 0f;
    [Range (0f,1f)] public float spatialBlend = 0f;
    // AudioSource set in the AudioManager
    [HideInInspector] public AudioSource source;
     

}