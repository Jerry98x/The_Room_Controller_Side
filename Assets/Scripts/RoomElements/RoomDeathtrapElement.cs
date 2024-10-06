using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] Transform deathtrapPortal;
    [SerializeField] DeathtrapTouchFeedbackHandler deathtrapTouchFeedbackHandler;
    [SerializeField] FullScreenEffectsManager fullScreenEffectsManager;
    
    [SerializeField] AudioSource goodTouchSound;
    [SerializeField] AudioSource badTouchSound;

    [ColorUsage(true, true)] private Color currentColor;
    
    private VisualEffect silhouetteEffect;

    private Material deathtrapMaterial;
    private Color deathtrapColor;
    private Vector3 lastSilhouettePosition;
    
    
    private Coroutine colorChangeCoroutine;
    private Coroutine silhouetteMoveCoroutine;
    private Coroutine silhouetteFadeInCoroutine;
    private Coroutine silhouetteFadeOutCoroutine;
    
    

    protected override void Start()
    {
        base.Start();   
        
        Debug.Log("PORCODIO: " + receivingEndPointSO.EndPoint);
        deathtrapMaterial = GetComponentInChildren<Renderer>().material;
        deathtrapColor = deathtrapMaterial.GetColor(Constants.EMISSION_COLOR_ID);
        silhouetteEffect = humanSilhouette.GetComponent<VisualEffect>();

        // Position the hypothetical initial "lastSilhouettePosition" in the middle of the perceivable distance
        lastSilhouettePosition = deathtrapPortal.position + (deathtrapPortal.position - transform.position) *
            (Constants.DEATHTRAP_SONAR_DISTANCE_MAX - Constants.DEATHTRAP_SONAR_DISTANCE_MIN) / (2 * Constants.DEATHTRAP_SONAR_DISTANCE_DIVISOR);
        
        lastMessage = new int[2];
        for (int i = 0; i < lastMessage.Length; i++)
        {
            lastMessage[i] = 0;
        }
    }


    private void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.J))
        {
            PresenceDetected(UnityEngine.Random.Range(0, 14000));
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PresenceDetected(15000);
        }
        

        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleTouchEffect(2);
            PlayAmbientTouchSound(2);
            HandleFullScreenEffect(2);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            HandleTouchEffect(0);
            PlayAmbientTouchSound(0);
            HandleFullScreenEffect(0);
        }
        
    }


    protected override void ExecuteMessageResponse(string message)
    {
        Debug.Log("Deathtrap element received message: " + message);
        
        // Split the message string by the colon separator
        string[] parts = message.Split(separator);
        
        
        
        
        
        
        
        for(int i = 0; i < parts.Length; i++)
        {
            Debug.Log("DEATHTRAP MESSAGE PART " + i + ": " + parts[i]);
        }
        
        
        
        
        
        
        
        

        // Initialize the message array with the same length as parts, excluding the first one
        // (the sender type, which is not needed)
        this.messageContent = new int[parts.Length - 1];

        float[] messageContentFloat = new float[parts.Length - 1];

        // Convert each part to an integer and store it in the message array
        for (int i = 1; i < parts.Length; i++)
        {
            messageContentFloat[i-1] = float.Parse(parts[i]);
            this.messageContent[i-1] = Mathf.RoundToInt(messageContentFloat[i-1]);
            Debug.Log("DEATHTRAP MESSAGE CONTENT " + (i-1) + ": " + messageContent[i-1]);
        }
        
        
        
        if(lastMessage[1] != messageContent[1])
        {
            PresenceDetected(messageContent[1]);
        }
        
        if(lastMessage[0] != messageContent[0])
        {
            HandleTouchEffect(messageContent[0]);
            PlayAmbientTouchSound(messageContent[0]);
            HandleFullScreenEffect(messageContent[0]);
            
        }
        else
        {
            if (deathtrapTouchFeedbackHandler.IsNegativeEffectPlaying() && messageContent[0] != Constants.DEATHTRAP_NO_TOUCH_INTENSITY)
            {
                float deltaTimeToAdd = 0.03f;
                deathtrapTouchFeedbackHandler.IncreaseParticlesLifetime(deltaTimeToAdd);
            }
        }
        
        
        
        lastMessage[0] = messageContent[0];
        lastMessage[1] = messageContent[1];
        
        
        
        /*if(lastMessage[0] == messageContent[0] && lastMessage[1] == messageContent[1])
        {
            return;
        }
        else
        {
            lastMessage[0] = messageContent[0];
            lastMessage[1] = messageContent[1];
            
            
            
            // Apply actions
            if(lastMessage[0] != messageContent[0])
            {
                ChangeDeathtrapEmissionColor(messageContent[0]);
            }

            if (lastMessage[1] != messageContent[1])
            {
                PresenceDetected(messageContent[1]);
            }
            
            
        }*/
        
        
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
    
    
    
    private void HandleTouchEffect(int touchIntensity)
    {
        // In case the touch intensity is > 0, the touch feedback effect should be played
        // However, first we need to check if the human silhouette is active, so if for some reason / error
        // the silhouette is not active, we are going to activate it before playing the touch feedback effect
        if (touchIntensity > Constants.DEATHTRAP_NO_TOUCH_INTENSITY && !humanSilhouette.activeSelf)
        {
            // Play the silhouette effect, positioning it at a reasonable fixed distance in this case
            //PresenceDetected((int)Constants.DEATHTRAP_SONAR_DISTANCE_MIN);
            humanSilhouette.SetActive(true);
            humanSilhouette.transform.position = lastSilhouettePosition;
        }
        
        switch (touchIntensity)
        {
            case Constants.DEATHTRAP_NO_TOUCH_INTENSITY:
                deathtrapTouchFeedbackHandler.StopEffect(false);
                deathtrapTouchFeedbackHandler.StopEffect(true);
                break;
            case Constants.DEATHTRAP_SOFT_TOUCH_INTENSITY:
                
                GeneratePositiveParticlesEffect(touchIntensity);
                break;
            case Constants.DEATHTRAP_MEDIUM_TOUCH_INTENSITY:
            case Constants.DEATHTRAP_HARD_TOUCH_INTENSITY:
                GrowVinesEffect(touchIntensity);
                break;
        }
    }


    private void HandleFullScreenEffect(int touchIntensity)
    {

        int duration = (int)deathtrapTouchFeedbackHandler.GetStripsLifetime();
        fullScreenEffectsManager.DisplayFullScreenEffect(touchIntensity, duration);
    }
    
    
    
    private void GrowVinesEffect(int touchIntensity)
    {
        Debug.Log("Growing vines with touch intensity: " + touchIntensity);
        // Double check
        if(humanSilhouette.activeSelf && touchIntensity > 1)
        {
            // If the positive effect is playing, stop it and smoothly transition to the negative effect
            if(deathtrapTouchFeedbackHandler.IsPositiveEffectPlaying())
            {
                deathtrapTouchFeedbackHandler.StopEffect(true);
            }
            
            
            // Set the spawn position to the current position of the silhouette
            deathtrapTouchFeedbackHandler.SetSpawnPosition(humanSilhouette.transform.position);
            //deathtrapTouchFeedbackHandler.SetAttractorPosition(humanSilhouette.transform.position);
            deathtrapTouchFeedbackHandler.ResetInitialPosition();
            deathtrapTouchFeedbackHandler.VinesEffectStarted();
        }
    }
    
    private void GeneratePositiveParticlesEffect(int touchIntensity)
    {
        // Double check
        if (humanSilhouette.activeSelf && touchIntensity == 0)
        {
            // If the negative effect is playing, stop it and smoothly transition to the positive effect
            if (deathtrapTouchFeedbackHandler.IsNegativeEffectPlaying())
            {
                deathtrapTouchFeedbackHandler.StopEffect(false);
            }
            
            
            // Set the spawn position to the current position of the silhouette
            deathtrapTouchFeedbackHandler.SetSpawnPosition(humanSilhouette.transform.position);
            deathtrapTouchFeedbackHandler.ResetInitialPosition();
            // TODO: call the positive effect
        }
    }



    private void PlayAmbientTouchSound(int touchIntensity)
    {

        switch (touchIntensity)
        {
            case Constants.DEATHTRAP_NO_TOUCH_INTENSITY:
                // Stop existing sound, if any
                if (goodTouchSound.isPlaying)
                {
                    goodTouchSound.Stop();
                }

                if (badTouchSound.isPlaying)
                {
                    badTouchSound.Stop();
                }
                break;
            case Constants.DEATHTRAP_SOFT_TOUCH_INTENSITY:
                // Play the good touch sound
                if (badTouchSound.isPlaying)
                {
                    badTouchSound.Stop();
                }
                if (!goodTouchSound.isPlaying)
                {
                    goodTouchSound.Play();
                }
                break;
            case Constants.DEATHTRAP_MEDIUM_TOUCH_INTENSITY:
            case Constants.DEATHTRAP_HARD_TOUCH_INTENSITY:
                // Play the bad touch sound
                if (goodTouchSound.isPlaying)
                {
                    goodTouchSound.Stop();
                }
                if (!badTouchSound.isPlaying)
                {
                    badTouchSound.Play();
                }
                break;
        }
        
    }
    
    
    
    
    
    
    
    private void ChangeDeathtrapEmissionColor(int touchIntensity)
    {
        StopAllCoroutines();
        Debug.Log("Changing color with touch intensity: " + touchIntensity);
        switch (touchIntensity)
        {
            case 0:
                StartCoroutine(ChangeEmissionColorGradually(initialColor));
                break;
            case 1:
                StartCoroutine(ChangeEmissionColorGradually(softTouchColor));
                break;
            case 2:
                StartCoroutine(ChangeEmissionColorGradually(mediumTouchColor));
                break;
            case 3:
                StartCoroutine(ChangeEmissionColorGradually(hardTouchColor));
                break;
        }
    }

    
    
    private IEnumerator ChangeEmissionColorGradually(Color targetColor)
    {
        
        Debug.Log("Changing color coroutine with color: " + targetColor);
        currentColor = deathtrapColor;
        float duration = 1.0f; // Duration of the color change
        float elapsed = 0.0f;
  
        while (elapsed < duration)
        {
            deathtrapColor = Color.Lerp(currentColor, targetColor, elapsed / duration);
            deathtrapMaterial.SetColor(Constants.EMISSION_COLOR_ID, deathtrapColor);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        //deathtrapMaterial.SetColor(Constants.EMISSION_COLOR_ID, targetColor);
        
    }
    
    
    
    private void PresenceDetected(int detected)
    {
        Debug.Log("Changing silhouette at distance: " + detected);
        if (detected <= Constants.DEATHTRAP_SONAR_DISTANCE_MAX)
        {
            /*// Make the silhouette look at the core center
            humanSilhouette.transform.LookAt(transform.position);*/
            

            // Further check to avoid visually moving away the silhouette while the touch feedback effect is playing.
            // even if the distance might be actually changing. It's better to keep the silhouette in place when it is touching
            // the Deathtrap sphere and the effect is playing.
            if (!deathtrapTouchFeedbackHandler.IsNegativeEffectPlaying() && !deathtrapTouchFeedbackHandler.IsPositiveEffectPlaying())
            {
                Vector3 newPosition;
                if (detected >= Constants.DEATHTRAP_SONAR_DISTANCE_MIN)
                {
                    newPosition = deathtrapPortal.position + new Vector3(0f, 0f, detected / Constants.DEATHTRAP_SONAR_DISTANCE_DIVISOR);
                }
                else
                {
                    // If the distance is less than the minimum, set the silhouette at the minimum distance
                    newPosition = deathtrapPortal.position + new Vector3(0f, 0f, Constants.DEATHTRAP_SONAR_DISTANCE_MIN / Constants.DEATHTRAP_SONAR_DISTANCE_DIVISOR);
                }

                if(humanSilhouette.activeSelf)
                {
                    if (silhouetteMoveCoroutine != null)
                    {
                        StopCoroutine(silhouetteMoveCoroutine);
                    }
                    silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(lastSilhouettePosition, newPosition));
                }
                else
                {
                    float fadeInDuration = 0.4f;
                    humanSilhouette.SetActive(true);
                    
                    if (silhouetteFadeInCoroutine != null)
                    {
                        StopCoroutine(silhouetteFadeInCoroutine);
                    }
                    if (silhouetteMoveCoroutine != null)
                    {
                        StopCoroutine(silhouetteMoveCoroutine);
                    }
                    
                    
                    silhouetteFadeInCoroutine = StartCoroutine(FadeInSilhouette(fadeInDuration));
                    silhouetteMoveCoroutine = StartCoroutine(MoveSilhouette(lastSilhouettePosition, newPosition));
                }

                lastSilhouettePosition = newPosition;
            }
            
            
            
        }
        else
        {
            // Visitor is going away from the Deathtrap
            if(humanSilhouette.activeSelf)
            {
                float fadeOutDuration = 0.3f;
                if (silhouetteFadeOutCoroutine != null)
                {
                    if (silhouetteFadeOutCoroutine != null)
                    {
                        StopCoroutine(silhouetteFadeOutCoroutine);
                    }
                }
                
                silhouetteFadeOutCoroutine = StartCoroutine(FadeOutSilhouette(fadeOutDuration));
            }
        }
        
    }
    
    
    private IEnumerator MoveSilhouette(Vector3 fromPosition, Vector3 toPosition)
    {
        float duration = 0.5f; // Duration of the movement
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            humanSilhouette.transform.position = Vector3.Lerp(fromPosition, toPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        humanSilhouette.transform.position = toPosition;
    }
    
    private IEnumerator MoveAndFadeSilhouette(Vector3 fromPosition, Vector3 toPosition)
    {
        StartCoroutine(MoveSilhouette(fromPosition, toPosition));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(FadeOutSilhouette(0.3f));
    }
    
    private IEnumerator FadeInSilhouette(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            silhouetteEffect.SetFloat("Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        silhouetteEffect.SetFloat("Alpha", 1f);


        /*Renderer silhouetteRenderer = humanSilhouette.GetComponent<Renderer>();
        Color color = silhouetteRenderer.material.color;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(0, 1, elapsed / duration);
            silhouetteRenderer.material.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1;
        silhouetteRenderer.material.color = color;*/
    }
    
    private IEnumerator FadeOutSilhouette(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            silhouetteEffect.SetFloat("Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        silhouetteEffect.SetFloat("Alpha", 0f);
        humanSilhouette.SetActive(false);
        
        
        /*Renderer silhouetteRenderer = humanSilhouette.GetComponent<Renderer>();
        Color color = silhouetteRenderer.material.color;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(1, 0, elapsed / duration);
            silhouetteRenderer.material.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 0;
        silhouetteRenderer.material.color = color;
        humanSilhouette.SetActive(false);*/
    }
    

    
    
    /// <summary>
    /// Returns the position of the Deathtrap element
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    
}
