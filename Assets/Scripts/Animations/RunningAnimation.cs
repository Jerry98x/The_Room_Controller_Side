using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningAnimation : MonoBehaviour
{

    private Animator animator;
    
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    
    private void Update()
    {
        if (animator != null && Input.GetKeyDown(KeyCode.A))
        {
            bool isRunning = animator.GetBool("run");
            animator.SetBool("run", !isRunning);
        }
        
    }
    
    
    
}
