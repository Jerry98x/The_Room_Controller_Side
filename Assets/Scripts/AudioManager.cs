using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager Instance { get; private set; }
    
    
    [SerializeField] private Sound[] music;
    [SerializeField] private Sound[] sfx;
    [SerializeField] private AudioMixer masterMixer;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one AudioManager instance!");
        }
        Instance = this;

        // Generate sources
        foreach (Sound sound in music)
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
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
        //PlayBackground();
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

    public void PlayBackground()
    {
        Sound tempSound = Array.Find(music, sound => sound.name == "Background");
        if (tempSound.IsUnityNull())
        {
            Debug.LogWarning("Background sound not found!");
            return;
        }
        tempSound.source.Play();
    }
    
    public void Play(string name, bool isSFX)
    {
        Sound tempSound;
        if (isSFX)
        {
            tempSound = Array.Find(sfx, sound => sound.name == name);
            if (tempSound.IsUnityNull())
            {
                Debug.LogWarning("Sound effect " + name + " not found! PopiPopi");
                return;
            }
        }
        else
        {
            tempSound = Array.Find(music, sound => sound.name == name);
            if (tempSound.IsUnityNull())
            {
                Debug.LogWarning("Music track " + name + " not found! PopiPopi");
                return;
            }
        }

        if (tempSound.loop)
        {
            tempSound.source.Play();
        }
        else
        {
          tempSound.source.PlayOneShot(tempSound.clip);  
        }
        
    }

    public void Stop(string name, bool isSFX)
    {
        if (isSFX)
        {

            foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
            {
                if (audioSource.clip == Array.Find(sfx, sound => sound.name == name).clip && audioSource.isPlaying)
                    audioSource.Stop();
                    
            }
        }
        else
        {
            foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
            {
                if (audioSource.clip == Array.Find(music, sound => sound.name == name).clip && audioSource.isPlaying)
                    audioSource.Stop();
                    
            }
        }
    }

    public void StopAllAudioSources()
    {
        foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
        {
            // It works just with "Background" because that's the only BGM we have; not scalable!
            if (audioSource.isPlaying && audioSource.clip.name != "Background")
                audioSource.Stop();
                    
        }
    }

    public void StopAllLoopingSources()
    {
        foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
        {
            // It works just with "Background" because that's the only BGM we have; not scalable!
            if (audioSource.isPlaying && audioSource.loop && audioSource.clip.name != "Background")
                audioSource.Stop();
        }
    }

    /*private AudioSource GetAudioSource(string name, bool isSFX)
    {
        if (isSFX)
        {
            foreach (AudioSource audioSource in instance.GetComponents<AudioSource>())
            {
                if (audioSource.clip == Array.Find(sfx, sound => sound.name == name).clip && audioSource.isPlaying)
                    return audioSource;
            }
        }
        else
        {
            foreach (AudioSource audioSource in instance.GetComponents<AudioSource>())
            {
                if (audioSource.clip == Array.Find(music, sound => sound.name == name).clip && audioSource.isPlaying)
                    return audioSource;
            }
        }

        return null;
    }*/
    
    public void PlayInPoint(string name, bool isSFX, Vector3 pos)
    {
        Sound tempSound;
        if (isSFX)
        {
            tempSound = Array.Find(sfx, sound => sound.name == name);
            if (tempSound.IsUnityNull())
            {
                Debug.LogWarning("Sound effect " + name + " not found! PopiPopi");
                return;
            }
        }
        else
        {
            tempSound = Array.Find(music, sound => sound.name == name);
            if (tempSound.IsUnityNull())
            {
                Debug.LogWarning("Music track " + name + " not found! PopiPopi");
                return;
            }
        }

        AudioSource.PlayClipAtPoint(tempSound.clip, pos, 1f);
    }

    public bool IsAudioSourcePlaying(string name, bool isSFX)
    {
        if (isSFX)
        {
            foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
            {
                /*if (audioSource.clip.name == "bonfire")
                {
                    Debug.Log(audioSource.clip.name);
                    Debug.Log(audioSource.isPlaying);
                }*/
                
                if (audioSource.clip == Array.Find(sfx, sound => sound.name == name).clip && audioSource.isPlaying)
                    return true;
            }
        }
        else
        {
            foreach (AudioSource audioSource in Instance.GetComponents<AudioSource>())
            {
                if (audioSource.clip == Array.Find(music, sound => sound.name == name).clip && audioSource.isPlaying)
                    return true;
            }
        }

        return false;
    }

}
