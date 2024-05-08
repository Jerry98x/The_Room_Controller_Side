using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalConstraint : MonoBehaviour
{
    
    [SerializeField] private float minY; // The minimum y-coordinate
    [SerializeField] private float maxY; // The maximum y-coordinate
    

    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(minY);
        limits.Add(maxY);
        return limits;
    }
}
