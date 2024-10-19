using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectsManager : MonoBehaviour
{
    
    [SerializeField] private float positiveEffectDisplayTime = 1f;
    [SerializeField] private float positiveEffectFadeOutDuration = 0.5f;
    
    [SerializeField] private ScriptableRendererFeature negativeEffectRendererFeature;
    [SerializeField] private ScriptableRendererFeature positiveEffectRendererFeature;
    [SerializeField] private Material negativeEffectMaterial;
    [SerializeField] private Material positiveEffectMaterial;
    
    [SerializeField] private float voronoiIntensityStartAmount = 1.25f;
    [SerializeField] private float vignetteIntensityStartAmount = 1.25f;
    
    private float negativeEffectDisplayTime = 1f;
    private float negativeEffectFadeOutDuration = 0.5f;

    private int voronoiIntensity = Shader.PropertyToID("_voronoiIntensity");
    private int vignetteIntensity = Shader.PropertyToID("_vignetteIntensity");
    private int lightAlpha = Shader.PropertyToID("_lightAlpha");
    

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
            DisplayFullScreenEffect(3, 5, false);
        }
    }


    public void DisplayFullScreenEffect(int intensity, int baseDuration, bool isPositive)
    {
        switch (intensity)
        {
            case Constants.DEATHTRAP_NO_TOUCH_INTENSITY:
                StopEffect(isPositive);
                break;
            case Constants.DEATHTRAP_SOFT_TOUCH_INTENSITY:
                StopEffect(false);
                SetPositiveEffectDisplayTime(baseDuration);
                StartCoroutine(DisplayPositiveEffect());
                break;
            case Constants.DEATHTRAP_MEDIUM_TOUCH_INTENSITY:
            case Constants.DEATHTRAP_HARD_TOUCH_INTENSITY:
                StopEffect(true);
                SetNegativeEffectDisplayTime(baseDuration);
                StartCoroutine(DisplayNegativeEffect());
                break;
        }
    }
    
    
    private IEnumerator DisplayNegativeEffect()
    {
        
        yield return new WaitForSeconds(1f);
        
        negativeEffectRendererFeature.SetActive(true);
        negativeEffectMaterial.SetFloat(voronoiIntensity, voronoiIntensityStartAmount);
        negativeEffectMaterial.SetFloat(vignetteIntensity, vignetteIntensityStartAmount);
        
        //yield return new WaitForSeconds(negativeEffectDisplayTime);
        float elapsedTime = 0f;
        while (elapsedTime < negativeEffectDisplayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
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


    private IEnumerator DisplayPositiveEffect()
    {
        
        yield return new WaitForSeconds(1.5f);
        
        
        positiveEffectRendererFeature.SetActive(true);
        
        
        
        float elapsedTime = 0f;
        while (elapsedTime < positiveEffectDisplayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        /*elapsedTime = 0f;
        while (elapsedTime < positiveEffectFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float lerpedVoronoi = Mathf.Lerp(voronoiIntensityStartAmount, 0f, elapsedTime / negativeEffectFadeOutDuration);
            float lerpedVignette = Mathf.Lerp(vignetteIntensityStartAmount, 0f, elapsedTime / negativeEffectFadeOutDuration);
            
            positiveEffectMaterial.SetFloat(voronoiIntensity, lerpedVoronoi);
            positiveEffectMaterial.SetFloat(vignetteIntensity, lerpedVignette);
            
            yield return null;
        }*/

        positiveEffectRendererFeature.SetActive(false);
        
    }


    private void StopEffect(bool isPositive)
    {
        // Stop both effects prematurely with respect to their lifetime, but make them fade out
        StartCoroutine(FadeOutEffect(isPositive, negativeEffectRendererFeature, negativeEffectMaterial, negativeEffectFadeOutDuration));
        StartCoroutine(FadeOutEffect(isPositive, positiveEffectRendererFeature, positiveEffectMaterial, positiveEffectFadeOutDuration));
    }
    
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

}
