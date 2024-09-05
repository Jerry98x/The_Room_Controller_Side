using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a virtual Deathtrap element in the room
/// </summary>
/// <remarks>
/// Applied to the more general objects that represent a Deathtrap element in the room (the core).
/// Used mainly for handling the events related to the Deathtrap element after receiving messages from the physical Deathtrap module.
/// </remarks>
public class RoomDeathtrapElement : RoomBasicElement
{
    [SerializeField] [ColorUsage(true, true)] private Color initialColor;
    [SerializeField] [ColorUsage(true, true)] private Color softTouchColor;
    [SerializeField] [ColorUsage(true, true)] private Color mediumTouchColor;
    [SerializeField] [ColorUsage(true, true)] private Color hardTouchColor;
    [SerializeField] GameObject humanSilhouette;

    private Material deathtrapMaterial;
    private Color deathtrapColor;
    

    private int[] messageContent;
    private int[] lastMessage;
    private string separator = ":";


    protected override void Start()
    {
        base.Start();   
        
        Debug.Log("PORCODIO: " + endPointSO.EndPoint);
        deathtrapMaterial = GetComponentInChildren<Renderer>().material;
        deathtrapColor = deathtrapMaterial.GetColor(Constants.EMISSION_COLOR_ID);
        
        lastMessage = new int[2];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
    }
    
    
    protected override void ExecuteMessageResponse(string message)
    {
        Debug.Log("Deathtrap element received message: " + message);
        
        // Split the message string by the colon separator
        string[] parts = message.Split(separator);

        // Initialize the message array with the same length as parts
        this.messageContent = new int[parts.Length];

        // Convert each part to an integer and store it in the message array
        for (int i = 0; i < parts.Length; i++)
        {
            this.messageContent[i] = int.Parse(parts[i]);
        }
        
        
        
        if(lastMessage[0] == message[0] && lastMessage[1] == message[1])
        {
            return;
        }
        
        lastMessage[0] = message[0];
        lastMessage[1] = message[1];
        
        
        // Apply actions
        ChangeDeathtrapColor(message[0]);
        PresenceDetected(message[1]);
        
    }
    
    protected override void ExecuteMessageResponse(byte[] message)
    {
        Debug.Log("Deathtrap element received message: " + message);
        Debug.Log("message[0]: " + message[0]);
    }
    
    protected override void ExecuteMessageResponse(char[] message)
    {
        Debug.Log("Deathtrap element received message: " + message);
        Debug.Log("message[0]: " + message[0]);
    }
    
    
    
    private void ChangeDeathtrapColor(int touchIntensity)
    {
        StopAllCoroutines();
        switch (touchIntensity)
        {
            case 0:
                StartCoroutine(ChangeColorGradually(initialColor));
                break;
            case 1:
                StartCoroutine(ChangeColorGradually(softTouchColor));
                break;
            case 2:
                StartCoroutine(ChangeColorGradually(mediumTouchColor));
                break;
            case 3:
                StartCoroutine(ChangeColorGradually(hardTouchColor));
                break;
        }
    }
    
    
    private IEnumerator ChangeColorGradually(Color targetColor)
    {
        Color currentColor = deathtrapColor;
        float duration = 2.0f; // Duration of the color change
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            deathtrapColor = Color.Lerp(currentColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        deathtrapColor = targetColor;
    }
    
    
    
    private void PresenceDetected(int detected)
    {
        humanSilhouette.gameObject.transform.position = this.transform.position + new Vector3(0f, 0f, 1.3f);
        
        if (detected == 0)
        {
            humanSilhouette.SetActive(false);
        }
        else
        {
            humanSilhouette.SetActive(true);
        }
    }
    
    

    
    
    /// <summary>
    /// Returns the position of the Deathtrap element
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    
}
