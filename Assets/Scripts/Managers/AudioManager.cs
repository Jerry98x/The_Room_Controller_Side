using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

// NOT USED

/// <summary>
/// Manager class that handles the audio in the game
/// </summary>
public class AudioManager : MonoBehaviour
{ 
    
    // Singleton pattern
    public static AudioManager Instance { get; private set; }
    
    //[SerializeField] private AudioClipRefsSO audioClipRefsSo;
    
    [SerializeField] private AudioSource[] music;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioMixer masterMixer;
    
    private float volume = 1f;


    #region MonoBehaviour callbacks

    /// <summary>
    /// Initializes the AudioManager instance according to the Singleton pattern
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one AudioManager instance!");
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Generate sources
        /*foreach (Sound sound in music)
        {
            // we can add sources to this or child objects
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.outputAudioMixerGroup = masterMixer.FindMatchingGroups(Constants.MIXERGROUP_MUSIC)[0];
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.priority = sound.priority;
            sound.source.panStereo = sound.panStereo;
            sound.source.spatialBlend = sound.spatialBlend;
        }
        foreach (Sound s in sfx)
        {
            // we can add sources to this or child objects
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = masterMixer.FindMatchingGroups(Constants.MIXERGROUP_SFX)[0];
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.priority = s.priority;
            s.source.panStereo = s.panStereo;
            s.source.spatialBlend = s.spatialBlend;
        }*/
    }

    /// <summary>
    /// Loads the volume settings at the start of the scene
    /// </summary>
    private void Start()
    {
        LoadVolumeSettings();
        //PlayBackground();
    }

    #endregion




    #region Relevant functions

    /// <summary>
    /// Initializes the settings of the mixer
    /// </summary>
    public void LoadVolumeSettings()
    {
        float masterVolume = GetMasterVolume();
        float musicVolume = GetMusicVolume();
        float sfxVolume = GetSFXVolume();
        SetMasterVolume(masterVolume, false);
        SetMusicVolume(musicVolume, false);
        SetSFXVolume(sfxVolume, false);
    }
    
    /// <summary>
    /// Plays a specific sound in the specified position
    /// </summary>
    /// <param name="audioClip"> AudioClip to play </param>
    /// <param name="position"> Point in the space where the AudioClip will be played </param>
    /// <param name="volumeMultiplier"> Parameter to regulate the volume </param>
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier *  volume);
    }
    
    
    /*private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }*/
    
    
    /// <summary>
    /// Assigns the input clip to the input source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clip"></param>
    public void AssignClipToSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
    }

    #endregion

    
    
    
    #region Getters and setters

    /// <summary>
    /// Returns the volume of the "Master" group of the mixer
    /// </summary>
    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_MASTERVOLUME, 1f);
    }

    /// <summary>
    /// Returns the volume of the "Music" group of the mixer
    /// </summary>
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_MUSICVOLUME, 1f);
    }

    /// <summary>
    /// Returns the volume of the "SFX"" group of the mixer
    /// </summary>
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_SFXVOLUME, 1f);
    }

    /// <summary>
    /// Sets the volume of the "Master" group of the mixer
    /// </summary>
    /// <param name="value"> Volume value </param>
    /// <param name="serialize"> To allow saving the value in the player's preferences </param>
    public void SetMasterVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_MASTERVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_MASTERVOLUME, value);
    }

    /// <summary>
    /// Sets the volume of the "Music" group of the mixer
    /// </summary>
    /// <param name="value"> Volume value </param>
    /// <param name="serialize"> To allow saving the value in the player's preferences </param>
    public void SetMusicVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_MUSICVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_MUSICVOLUME, value);
    }

    /// <summary>
    /// Sets the volume of the "SFX" group of the mixer
    /// </summary>
    /// <param name="value"> Volume value </param>
    /// <param name="serialize"> To allow saving the value in the player's preferences </param>
    public void SetSFXVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_SFXVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_SFXVOLUME, value);
    }

    #endregion

}
