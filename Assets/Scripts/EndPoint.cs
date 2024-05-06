using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{

    private Vector3 initialPosition;
    
    void Start()
    {
        initialPosition = transform.position;
    }
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }


}
