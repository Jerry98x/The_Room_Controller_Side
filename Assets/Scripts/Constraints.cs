using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constraints : MonoBehaviour
{
    [SerializeField] private float min = -1f; // The minimum coordinate
    [SerializeField] private float max = 1f; // The maximum coordinate
    

    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(min);
        limits.Add(max);
        return limits;
    }
}