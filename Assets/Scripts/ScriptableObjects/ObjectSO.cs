using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ScriptableObject for testing purposes
/// </summary>
[CreateAssetMenu()]
public class ObjectSO : ScriptableObject
{

    public string objectName;
    public List<TestCharacteristicsSO> testIntensityCharacteristics;
    public List<TestCharacteristicsSO> testDurationCharacteristics;

    
}
