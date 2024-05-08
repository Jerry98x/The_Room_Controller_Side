using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArea : MonoBehaviour
{
    
    [SerializeField] private LayerMask layerMask;
    
    public LayerMask GetLayerMask()
    {
        return layerMask;
    }
    
}
