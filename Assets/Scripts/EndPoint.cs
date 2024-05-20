using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{

    private Vector3 initialPosition;
    
    //[SerializeField] private List<GameObject> vfxPrefab; // Reference to the VFX object
    
    //TODO: create child object just for colliders and rotate it to face the core
    
    void Start()
    {
        initialPosition = transform.position;
        
        /*CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
        foreach (CapsuleCollider collider in colliders)
        {
            if (collider != null && collider.enabled)
            {
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                visual.transform.position = collider.transform.position;
                visual.transform.rotation = collider.transform.rotation;
                visual.transform.localScale = new Vector3(collider.radius * 2, collider.height / 2, collider.radius * 2);
                visual.transform.parent = transform;

                // Make the visual representation red and more visible
                Color color = Color.red;
                color.a = 1f; // Fully opaque
                visual.GetComponent<Renderer>().material.color = color;
            }
        }*/
        
    }
    
    
    /*public void ActivateVFX()
    {
        // Instantiate the VFX object
        GameObject vfxObject = Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        // Activate the VFX object
        vfxObject.SetActive(true);

        // Optionally, destroy the VFX object after some time
        Destroy(vfxObject, 5f); // Destroy after 5 seconds
    }*/
    
    
    
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }


}
