using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauronPortalVFX : MonoBehaviour
{
    [SerializeField] private Transform center;
    
    private Vector3 rayDirection;


    private void Start()
    {
        Vector3 yOffset = new Vector3(0, 0, 0);
        rayDirection = center.position - transform.position - yOffset;
        transform.rotation = Quaternion.LookRotation(rayDirection);
    }
}
