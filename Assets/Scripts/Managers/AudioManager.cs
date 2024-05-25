using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{ 
    
    public static AudioManager Instance { get; private set; }
    
    //[SerializeField] private AudioClipRefsSO audioClipRefsSo;
    
    [SerializeField] private AudioSource[] music;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioMixer masterMixer;
    
    
    private float volume = 1f;

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

    private void Start()
    {
        LoadVolumeSettings();
        //PlayBackground();
    }





    public void AssignClipToSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
    }
    
    
    
    
    

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_MASTERVOLUME, 1f);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_MUSICVOLUME, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(Constants.MIXERPARAM_SFXVOLUME, 1f);
    }

    public void SetMasterVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_MASTERVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_MASTERVOLUME, value);
    }

    public void SetMusicVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_MUSICVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_MUSICVOLUME, value);
    }

    public void SetSFXVolume(float value, bool serialize = true)
    {
        masterMixer.SetFloat(Constants.MIXERPARAM_SFXVOLUME, Mathf.Log10(value) * 20);
        if (serialize) PlayerPrefs.SetFloat(Constants.MIXERPARAM_SFXVOLUME, value);
    }

    public void LoadVolumeSettings()
    {
        float masterVolume = GetMasterVolume();
        float musicVolume = GetMusicVolume();
        float sfxVolume = GetSFXVolume();
        SetMasterVolume(masterVolume, false);
        SetMusicVolume(musicVolume, false);
        SetSFXVolume(sfxVolume, false);
    }
    
    
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier *  volume);
    }
    
    
    /*private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }*/
    

}
