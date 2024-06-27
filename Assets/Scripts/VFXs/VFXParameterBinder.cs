using UnityEngine;
using UnityEngine.VFX;

public class VFXParameterBinder : MonoBehaviour
{
    private VisualEffect visualEffect;

    void Start()
    {
        // Get the VisualEffect component on this GameObject
        visualEffect = GetComponent<VisualEffect>();
        if (visualEffect == null)
        {
            Debug.LogError("No VisualEffect component found on this GameObject.");
        }
    }

    // Method to bind a GraphicsBuffer to a named property in the VFX graph
    public void BindGraphicsBuffer(string propertyName, GraphicsBuffer buffer)
    {
        if (visualEffect != null)
        {
            // Ensure the property exists and is of the correct type
            if (visualEffect.HasGraphicsBuffer(propertyName))
            {
                // Bind the GraphicsBuffer to the property
                visualEffect.SetGraphicsBuffer(propertyName, buffer);
            }
            else
            {
                Debug.LogError($"Property '{propertyName}' does not exist or is not of type GraphicsBuffer.");
            }
        }
    }
}