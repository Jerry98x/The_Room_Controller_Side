using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu()]
public class TestCharacteristicsSO : ScriptableObject
{

    public string characteristicName;
    public float lowerBound;
    public float upperBound;
    public float initialValue;
    public float value;
    //public float value;
    public float multiplier;
    
    public Color color;
    public Texture2D texture;
    
    public bool toActivate;


}
