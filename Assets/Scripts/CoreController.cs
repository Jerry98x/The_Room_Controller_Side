using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoreController : MonoBehaviour
{

    [SerializeField] private Transform coreCenter;
    
    public static CoreController Instance { get; private set; }

    
    private Interactable selectedObject;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one core instance!");
        }
        Instance = this;
    }

    private void Update()
    {
        HandleRaycasting();
    }


    private void HandleRaycasting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {

            if (hit.transform.TryGetComponent(out Interactable interactable))
            {
                // 
                
                
                // Hit object is interactable and it's either a Neto module or a Sauron module
                SelectObject(interactable);
            }
            
            /*Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                SelectObject(interactable);
            }*/
        }
    }
    
    public void SelectObject(Interactable newObject)
    {
        if (selectedObject != null)
        {
            selectedObject.OnDeselect();
        }

        selectedObject = newObject;
        selectedObject.OnSelect();
    }
    
    public Transform GetCoreCenter()
    {
        return coreCenter;
    }
    
    
}
