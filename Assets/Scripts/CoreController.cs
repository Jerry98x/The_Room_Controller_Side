using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that represents the core of the Room
/// </summary>
/// <remarks>
/// The core of the Room is called "Deathtrap"
/// </remarks>
public class CoreController : MonoBehaviour
{
    
    // Singleton pattern

    [SerializeField] private Transform coreCenter;
    
    public static CoreController Instance { get; private set; }

    
    private Interactable selectedObject;


    #region MonoBehaviourr callbacks

    /// <summary>
    /// Initializes the core instance according to the Singleton pattern
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one core instance!");
        }
        Instance = this;
    }

    /// <summary>
    /// Handles the raycasting at each frame
    /// </summary>
    private void Update()
    {
        HandleRaycasting();
    }

    #endregion



    #region Relevant functions

    //TODO: see if this custom function is really necessary and in case it is not, remove it
    /// <summary>
    /// Handles the raycasting to detect the objects that the player is looking at
    /// </summary>
    private void HandleRaycasting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {

            if (hit.transform.TryGetComponent(out Interactable interactable))
            {
                
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
    
    /// <summary>
    /// Updates the selected object
    /// </summary>
    /// <param name="newObject"> Updated object to select </param>
    public void SelectObject(Interactable newObject)
    {
        if (selectedObject != null)
        {
            selectedObject.OnDeselect();
        }

        selectedObject = newObject;
        selectedObject.OnSelect();
    }

    #endregion



    #region Getters and setters

    /// <summary>
    /// Returns the core center
    /// </summary>
    public Transform GetCoreCenter()
    {
        return coreCenter;
    }

    #endregion

    
    
}
