using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalConstraint : MonoBehaviour
{
    [SerializeField] private float minX = -1f; // The minimum y-coordinate
    [SerializeField] private float maxX = 1f; // The maximum y-coordinate
    

    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(minX);
        limits.Add(maxX);
        return limits;
    }
}
