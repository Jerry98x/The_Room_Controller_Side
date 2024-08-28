
using UnityEngine;

/// <summary>
/// Class containing all the useful constants of the application
/// </summary>
public class Constants {
    
    #region Layers
    public const string NETO_LAYER = "NetoLayer";
    public const string SAURON_LAYER = "SauronLayer";
    public const string NETO_LAYER_WALL = "NetoLayerWall";
    public const string SAURON_LAYER_WALL = "SauronLayerWall";
    #endregion
    
    #region Mixer
    public const string MIXERGROUP_MASTER = "Master";
    public const string MIXERGROUP_MUSIC = "Master/Music";
    public const string MIXERGROUP_SFX = "Master/SFX";
    public const string MIXERPARAM_MASTERVOLUME = "MasterVolume";
    public const string MIXERPARAM_MUSICVOLUME = "MusicVolume";
    public const string MIXERPARAM_SFXVOLUME = "SFXVolume";
    #endregion



    #region ScriptableObjects
    public const string NETO_VISUALIZATION_TESTING = "NetoVisualizationTesting";
    public const string NETO_AUDIO_TESTING = "NetoAudioTesting";
    public const string NETO_STRAIN_TESTING = "NetoStrainTesting";
    public const string SAURON_TOUCH_TESTING = "SauronTouchTesting";
    
    public const string NETO_AUDIO_INTENSITY_BY_ADDITIONAL_NOISE = "NetoAudioIntensityByAdditionalNoise";
    public const string NETO_AUDIO_INTENSITY_BY_PITCH = "NetoAudioIntensityByPitch";
    public const string NETO_AUDIO_INTENSITY_BY_VOLUME = "NetoAudioIntensityByVolume";
    public const string NETO_DURATION_BY_CIRCLES_NUMBER = "NetoDurationByCirclesNumber";
    public const string NETO_DURATION_BY_NOISE = "NetoDurationByNoise";
    public const string NETO_DURATION_BY_SUBEMITTER = "NetoDurationBySubemitter";
    public const string NETO_INTENSITY_BY_CIRCLES_COLOR = "NetoIntensityByCirclesColor";
    public const string NETO_INTENSITY_BY_CIRCLES_NUMBER = "NetoIntensityByCirclesNumber";
    public const string NETO_INTENSITY_BY_SPEED_MULTIPLIER = "NetoIntensityBySpeedMultiplier";
    #endregion



    #region Useful ray values

    public const float NETO_INTENSITY_BY_CIRCLES_COLOR_VALUE = -1000f;

    public const float POINTER_REACH_DISTANCE_Z_AXIS = 0.8f;
    public const float ENDPOINT_REACH_DISTANCE_Z_AXIS = 8f;
    public const float ENDPOINT_REACH_Z_MIN = 2f;
    public const float ENDPOINT_REACH_Z_MAX = 10f;
    
    public const float CAPPED_MIN_EMISSION_INTENSITY = 2f;
    public const float CAPPED_MAX_EMISSION_INTENSITY = 5f;


    // Inversely proportional to the distance
    public const float NETO_AMPLITUDE_DISTANCE_RATE = 0.45f;
    public const float NETO_FREQUENCY_DISTANCE_RATE = 12f;


    public const float PARENT_LINERENDERER_WIDTH_MULTIPLIER = 0.8f;
    public const float CHILD_LINERENDERER_WIDTH_MUKTIPLIER = 1f;

    #endregion



    #region Effects values

    public const bool PASSIVE_AUDIO_NETO = true;

    #endregion




    #region Useful VR values

    public const float XR_CONTROLLER_MIN_GRIP_VALUE = 0.0f;
    public const float XR_CONTROLLER_MAX_GRIP_VALUE = 1.0f;
    public const float XR_CONTROLLER_GRIP_VALUE_THRESHOLD = 0.1f;
    public const float XR_CONTROLLER_TRIGGER_VALUE_THRESHOLD = 0.1f;

    #endregion




    #region Shader properties

    public static int BASE_COLOR_ID = Shader.PropertyToID("_BaseColor");
    public static int EMISSIVE_COLOR_ID = Shader.PropertyToID("_EmissiveColor");
    public static int EMISSIVE_INTENSITY_ID = Shader.PropertyToID("_EmissiveIntensity");

    #endregion
    

    
    
    #region UDP-ESP communication
    
    // ESP32 need int values, but here most of them are float for the sake of the remapping.
    // They will be casted to int before sending.
    
    public const int NETO_SOUND_TYPE_1 = 1;
    public const int NETO_SOUND_TYPE_2 = 2;
    
    public const float NETO_SOUND_VOLUME_MIN = 0f;
    public const float NETO_SOUND_VOLUME_MAX = 30f;
    
    public const float NETO_SERVO_ANGLE_HIGH = 0f;
    public const float NETO_SERVO_ANGLE_LOW = 120f;
    
    // Radius may be a misleading name, but it's the amount of leds
    // that should be turned on in the ESP32 of the Neto module along its height
    public const float NETO_RADIUS_MIN = 0f;
    public const float NETO_RADIUS_MAX = 10f;
    
    public const float NETO_BRIGHTNESS_MIN = 0f;
    public const float NETO_BRIGHTNESS_MAX = 255f;
    
    public const float SAURON_ROTATION_SERVO_ANGLE_HIGH = 0f;
    public const float SAURON_ROTATION_SERVO_ANGLE_LOW = 180f;
    
    public const float SAURON_INCLINATION_SERVO_ANGLE_HIGH = 0f;
    public const float SAURON_INCLINATION_SERVO_ANGLE_LOW = 60f;
    public const float SAURON_OFFSET_INCLINATION_SERVO_ANGLE = 90f - SAURON_INCLINATION_SERVO_ANGLE_LOW;
    
    public const string TERMINATION_CHARACTER = "\0";
    
    #endregion
    

}
