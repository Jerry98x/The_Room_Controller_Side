using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: fix the class or remove it completely if it's not used
public class EmissiveColorChanger : MonoBehaviour
{
    
    
    private Material originalMaterial; // To store the original material
    private Material instantiatedMaterial; // The new instantiated material
    
    
    void Start()
    {
        // Get the Renderer component from the GameObject
        Renderer renderer = GetComponent<Renderer>();

        // Store the original material
        originalMaterial = renderer.material;

        // Instantiate a new material based on the template
        instantiatedMaterial = new Material(renderer.material);

        // Assign the new material to the renderer
        renderer.material = instantiatedMaterial;
    }
    
    
    
    public void ChangeMaterialColor(Color newColor)
    {
        // Change the color of the instantiated material
        instantiatedMaterial.color = newColor;
    }
    
    
    public void ChangeMaterialAlpha(float newAlpha)
    {
        // Change the alpha of the instantiated material
        Color color = instantiatedMaterial.color;
        color.a = newAlpha;
        ChangeMaterialColor(color);
    }
    
    
    public void ChangeMaterialEmissiveColor(Color newColor)
    {
        // Change the emission color of the instantiated material
        instantiatedMaterial.SetColor(Constants.EMISSIVE_COLOR_ID, newColor);
    }
    
    public void ChangeMaterialEmissionIntensity(float newIntensity)
    {
        // Change the emission intensity of the instantiated material
        instantiatedMaterial.SetFloat(Constants.EMISSIVE_INTENSITY_ID, newIntensity);
    }
    
    
    
    
    void OnDestroy()
    {
        // Revert back to the original material when the object is destroyed
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = originalMaterial;

        // Optionally destroy the instantiated material to free up memory
        Destroy(instantiatedMaterial);
    }
    
    

    
    
    
    
    
    
    /*private MaterialPropertyBlock propBlock;
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
    }*/
}
