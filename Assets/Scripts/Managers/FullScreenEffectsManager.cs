using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Manager class that handles visual effects applied directly to the camera and so to the VR headset
/// </summary>
public class FullScreenEffectsManager : MonoBehaviour
{
    
    [SerializeField] private float positiveEffectDisplayTime = 1f;
    [SerializeField] private float positiveEffectFadeOutDuration = 0.2f;
    
    [SerializeField] private ScriptableRendererFeature negativeEffectRendererFeature;
    [SerializeField] private ScriptableRendererFeature positiveEffectRendererFeature;
    [SerializeField] private Material negativeEffectMaterial;
    [SerializeField] private Material positiveEffectMaterial;
    
    [SerializeField] private float voronoiIntensityStartAmount = 1.25f;
    [SerializeField] private float vignetteIntensityStartAmount = 1.25f;
    
    private float negativeEffectDisplayTime = 1f;
    private float negativeEffectFadeOutDuration = 0.2f;

    private int voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
    private int vignetteIntensity = Shader.PropertyToID("_VignetteIntensity");
    private int lightAlpha = Shader.PropertyToID("_Alpha");


    private bool isTouchCheck = false;
    

    private void Start()
    {
        negativeEffectRendererFeature.SetActive(false);
        positiveEffectRendererFeature.SetActive(false);
    }

    private void Update()
    {
        //HandledEvents();
    }
    
    private void HandledEvents()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DisplayFullScreenEffect(3, false);
        }
    }


    /// <summary>
    /// Activates the correct effect based on the boolean parameter
    /// </summary>
    /// <param name="intensity"> Intensity of the effect </param>
    /// <param name="isPositive"> To distinguish between positive and negative effect </param>
    public void DisplayFullScreenEffect(int intensity, bool isPositive)
    {

        if (isPositive)
        {
            negativeEffectRendererFeature.SetActive(false);
            positiveEffectRendererFeature.SetActive(true);
        }
        else
        {
            positiveEffectRendererFeature.SetActive(false);
            negativeEffectRendererFeature.SetActive(true);
        }
        
    }
    
    /// <summary>
    /// Coroutine for a fade-in effect of the negative effect
    /// </summary>
    private IEnumerator DisplayNegativeEffect()
    {
        
        //yield return new WaitForSeconds(1f);
        
        negativeEffectRendererFeature.SetActive(true);
        negativeEffectMaterial.SetFloat(voronoiIntensity, voronoiIntensityStartAmount);
        negativeEffectMaterial.SetFloat(vignetteIntensity, vignetteIntensityStartAmount);
        
        //yield return new WaitForSeconds(negativeEffectDisplayTime);
        float elapsedTime = 0f;
        /*while (elapsedTime < negativeEffectDisplayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/
        
        elapsedTime = 0f;
        while (elapsedTime < negativeEffectFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float lerpedVoronoi = Mathf.Lerp(voronoiIntensityStartAmount, 0f, elapsedTime / negativeEffectFadeOutDuration);
            float lerpedVignette = Mathf.Lerp(vignetteIntensityStartAmount, 0f, elapsedTime / negativeEffectFadeOutDuration);
            
            negativeEffectMaterial.SetFloat(voronoiIntensity, lerpedVoronoi);
            negativeEffectMaterial.SetFloat(vignetteIntensity, lerpedVignette);
            
            yield return null;
        }
        
        negativeEffectRendererFeature.SetActive(false);
        
    }


    /// <summary>
    /// Coroutine for a fade-in effect of the positive effect
    /// </summary>
    private IEnumerator DisplayPositiveEffect()
    {
        
        //yield return new WaitForSeconds(1.5f);
        
        
        positiveEffectRendererFeature.SetActive(true);
        
        
        
        float elapsedTime = 0f;
        while (elapsedTime < positiveEffectDisplayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        positiveEffectRendererFeature.SetActive(false);
        
    }


    /// <summary>
    /// Stops the correct effect based on the boolean parameter
    /// </summary>
    /// <param name="isPositive"> To distinguish between positive and negative effect </param>
    private void StopEffect(bool isPositive)
    {
        // Stop both effects prematurely with respect to their lifetime, but make them fade out
        if (isPositive)
        {
            StartCoroutine(FadeOutEffect(true, negativeEffectRendererFeature, negativeEffectMaterial, negativeEffectFadeOutDuration));
        }
        else
        {
          StartCoroutine(FadeOutEffect(false, positiveEffectRendererFeature, positiveEffectMaterial, positiveEffectFadeOutDuration));
        }
    }


    /// <summary>
    /// Stops all the effects
    /// </summary>
    public void StopEffects()
    {
        if (negativeEffectRendererFeature.isActive)
        {
            negativeEffectRendererFeature.SetActive(false);
        }

        if (positiveEffectRendererFeature.isActive)
        {
            positiveEffectRendererFeature.SetActive(false);
        }
    }
    
    /// <summary>
    /// Coroutine for a fade-out effect for both the visual effects, based on the boolean parameter
    /// </summary>
    IEnumerator FadeOutEffect(bool isPositive, ScriptableRendererFeature scriptableRendererFeature, Material material, float duration)
    {
        float elapsedTime = 0f;

        if(isPositive)
        {
            float initialLightAlpha = material.GetFloat(lightAlpha);
            
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float lerpedLightAlpha = Mathf.Lerp(initialLightAlpha, 0f, elapsedTime / duration);
                
                material.SetFloat(lightAlpha, lerpedLightAlpha);
                
                yield return null;
            }
        }
        else
        {
            float initialVoronoi = material.GetFloat(voronoiIntensity);
            float initialVignette = material.GetFloat(vignetteIntensity);
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float lerpedVoronoi = Mathf.Lerp(initialVoronoi, 0f, elapsedTime / duration);
                float lerpedVignette = Mathf.Lerp(initialVignette, 0f, elapsedTime / duration);
                
                material.SetFloat(voronoiIntensity, lerpedVoronoi);
                material.SetFloat(vignetteIntensity, lerpedVignette);
                
                yield return null;
            }
        }
        
        
        scriptableRendererFeature.SetActive(false);
    }


    /// <summary>
    /// Increases the duration of the full-screen effect
    /// </summary>
    /// <param name="value"> Increase amount </param>
    /// <param name="isPositiveEffect"> To distinguish between positive and negative effect </param>
    public void IncreaseFullScreenEffectDuration(float value, bool isPositiveEffect)
    {
        if (isPositiveEffect)
        {
            positiveEffectDisplayTime += value;
        }
        else
        {
            negativeEffectDisplayTime += value; }
        
    }
    
    public void SetPositiveEffectDisplayTime(float time)
    {
        positiveEffectDisplayTime = time;
    }
    
    public void SetPositiveEffectFadeOutDuration(float time)
    {
        positiveEffectFadeOutDuration = time;
    }
    
    public void SetNegativeEffectDisplayTime(float time)
    {
        negativeEffectDisplayTime = time;
    }
    
    public void SetNegativeEffectFadeOutDuration(float time)
    {
        negativeEffectFadeOutDuration = time;
    }
    
    public bool IsPositiveFullScreenEffectPlaying()
    {
        return positiveEffectRendererFeature.isActive;
    }
    
    public bool IsNegativeFullScreenEffectPlaying()
    {
        return negativeEffectRendererFeature.isActive;
    }

    public bool IsTouching()
    {
        return isTouchCheck;
    }

    public void SetTouchCheck(bool isTouching)
    {
        isTouchCheck = isTouching;
    }

}
