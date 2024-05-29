using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: See if this class can be deleted because it is not used
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
