using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
}
