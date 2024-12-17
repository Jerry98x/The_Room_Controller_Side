using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT USED

/// <summary>
/// Class that defines the area where the Controller can move
/// </summary>
public class MovementArea : MonoBehaviour
{
    
    [SerializeField] private LayerMask layerMask;
    
    public LayerMask GetLayerMask()
    {
        return layerMask;
    }
    
}
