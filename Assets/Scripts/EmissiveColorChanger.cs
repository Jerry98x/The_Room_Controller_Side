using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveColorChanger : MonoBehaviour
{
    private MaterialPropertyBlock propBlock;
    private Renderer renderer;
    private Color originalColor;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        renderer = GetComponent<Renderer>();

        // Save the original color
        originalColor = renderer.material.GetColor("_EmissionColor");
    }

    public void ChangeColor(Color newColor)
    {
        // Set the new color
        propBlock.SetColor("_EmissionColor", newColor);
        renderer.SetPropertyBlock(propBlock);
    }

    private void OnDisable()
    {
        // Reset to the original color when the object is disabled
        propBlock.SetColor("_EmissionColor", originalColor);
        renderer.SetPropertyBlock(propBlock);
    }
}
