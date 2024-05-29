using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles the growth of vines
/// </summary>
/// <remarks>
/// Used for the ShaderGraph + Blender vine growth effect
/// </remarks>
public class GrowVines : MonoBehaviour
{

    [SerializeField] private List<MeshRenderer> growVinesMeshes;
    [SerializeField] private float timeToGrow = 5.0f;
    [SerializeField] private float refreshRate = 0.05f;
    [Range(0,1)]
    [SerializeField] private float minGrow = 0.2f;
    [Range(0,1)]
    [SerializeField] private float maxGrow = 0.97f;

    private List<Material> growVinesMaterials = new List<Material>();
    private bool fullyGrown = false;



    #region MonoBehaviour callbacks

    /// <summary>
    /// Adds the materials to the list and sets the initial growth value
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < growVinesMeshes.Count; i++)
        {
            for (int j = 0; j < growVinesMeshes[i].materials.Length; j++)
            {
                if(growVinesMeshes[i].materials[j].HasProperty("_Growth"))
                {
                    growVinesMeshes[i].materials[j].SetFloat("_Growth", minGrow);
                    growVinesMaterials.Add(growVinesMeshes[i].materials[j]);
                }   
            }
        }
    }


    /// <summary>
    /// Handles a key press to grow the vines
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < growVinesMaterials.Count; i++)
            {
                StartCoroutine(GrowVinesCoroutine(growVinesMaterials[i]));
            }
        }
    }

    #endregion


    #region Relevant functions

    /// <summary>
    /// Coroutine that grows the vines by modifying the material with the vertex shader that handles the growth
    /// </summary>
    /// <param name="material"> Material to update </param>

    IEnumerator GrowVinesCoroutine(Material material)
    {
        float growValue = material.GetFloat("_Growth");
        
        if(!fullyGrown)
        {
            while (growValue < maxGrow)
            {
                growValue +=1 / (timeToGrow / refreshRate);
                material.SetFloat("_Growth", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                growValue -= Time.deltaTime / timeToGrow;
                material.SetFloat("_Growth", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        
        if(growValue >= maxGrow)
        {
            fullyGrown = true;
        }
        else if(growValue <= minGrow)
        {
            fullyGrown = false;
        }
        
    }

    #endregion
    

    
}
