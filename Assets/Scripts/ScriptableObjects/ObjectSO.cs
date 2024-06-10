using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class ObjectSO : ScriptableObject
{

    public string objectName;
    public List<TestCharacteristicsSO> testIntensityCharacteristics;
    public List<TestCharacteristicsSO> testDurationCharacteristics;

    
}
