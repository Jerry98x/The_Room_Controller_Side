using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalConstraint : MonoBehaviour
{
    
    [SerializeField] private float minY; // The minimum y-coordinate
    [SerializeField] private float maxY; // The maximum y-coordinate
    /*private void OnTriggerStay(Collider other)
    {
        EndPoint endPoint = other.GetComponentInChildren<EndPoint>();
        if (endPoint != null)
        {
            Vector3 constrainedPosition = other.transform.position;
            constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, minY, maxY);
            other.transform.position = constrainedPosition;
        }
    }*/

    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(minY);
        limits.Add(maxY);
        return limits;
    }
}
