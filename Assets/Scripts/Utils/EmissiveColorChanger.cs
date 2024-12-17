using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT USED

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
}
