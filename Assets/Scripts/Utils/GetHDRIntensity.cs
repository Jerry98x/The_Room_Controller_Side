using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to get the intensity of a HDR color
/// </summary>
public class GetHDRIntensity : MonoBehaviour
{
    [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
    public Color color;

    private void Start()
    {
        Color color32;
        float exp;
        DecomposeHdrColor(color, out color32, out exp);
        Debug.Log(exp);
    }

    private const byte k_MaxByteForOverexposedColor = 191;
    
    /// <summary>
    /// Separate the HDR color into a base color and an exposure value
    /// </summary>
    /// <param name="linearColorHdr"> HDR color </param>
    /// <param name="baseLinearColor"> Base color (non-HDR) </param>
    /// <param name="exposure"> Exposure </param>
    public static void DecomposeHdrColor(Color linearColorHdr, out Color baseLinearColor, out float exposure)
    {
        baseLinearColor = linearColorHdr;
        var maxColorComponent = linearColorHdr.maxColorComponent;
        // replicate Photoshops's decomposition behaviour
        if (maxColorComponent == 0f || maxColorComponent <= 1f && maxColorComponent >= 1 / 255f)
        {
            exposure = 0f;
            baseLinearColor.r = (byte)Mathf.RoundToInt(linearColorHdr.r * 255f);
            baseLinearColor.g = (byte)Mathf.RoundToInt(linearColorHdr.g * 255f);
            baseLinearColor.b = (byte)Mathf.RoundToInt(linearColorHdr.b * 255f);
        }
        else
        {
            // calibrate exposure to the max float color component
            var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
            exposure = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);
            // maintain maximal integrity of byte values to prevent off-by-one errors when scaling up a color one component at a time
            baseLinearColor.r = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.r));
            baseLinearColor.g = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.g));
            baseLinearColor.b = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.b));
        }
    }
    
    
    
    /// <summary>
    /// Change the intensity of a HDR color
    /// </summary>
    /// <param name="hdrColor"> Color to be changed </param>
    /// <param name="newIntensity"> New intensity level </param>
    public static Color AdjustEmissiveIntensity(Color hdrColor, float newIntensity)
    {
        DecomposeHdrColor(hdrColor, out Color baseColor, out float exposure);

        float factor = Mathf.Pow(2, newIntensity) / 255f;

        // Recompose the HDR color with the adjusted exposure
        Color adjustedHdrColor = new Color(
            baseColor.r * factor,
            baseColor.g * factor,
            baseColor.b * factor,
            hdrColor.a
        );

        return adjustedHdrColor;
    }
    
}