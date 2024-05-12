using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private Vector3 pointerPosition;
    
    void Start()
    {
        pointerPosition = transform.position;
    }
    public Vector3 GetInitialPosition()
    {
        return pointerPosition;
    }
    
}
