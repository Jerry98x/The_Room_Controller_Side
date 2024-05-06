using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{

    [SerializeField] private Vector3 initialPosition;
    
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }


}
