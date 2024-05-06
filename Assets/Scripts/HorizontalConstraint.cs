using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalConstraint : MonoBehaviour
{
    [SerializeField] private float minX; // The minimum y-coordinate
    [SerializeField] private float maxX; // The maximum y-coordinate
    /*private void OnTriggerStay(Collider other)
    {
        EndPoint endPoint = other.GetComponentInChildren<EndPoint>();
        if (endPoint != null)
        {
            Vector3 constrainedPosition = other.transform.position;
            constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, minX, maxX);
            other.transform.position = constrainedPosition;
        }
    }*/

    public List<float> GetLimits()
    {
        List<float> limits = new List<float>();
        limits.Add(minX);
        limits.Add(maxX);
        return limits;
    }
}
