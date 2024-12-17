using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// NOT USED

/// <summary>
/// Class that handles the raycast interaction with the modules of the Room and updates the rays' characteristics;
/// tries to use a finite state machine to manage the movement of the ray
/// </summary>
/// <remarks>
/// Used for the second version of the Room, where action/movement is on the ray itself
/// (the movement is reflected in the change in the characteristics of the ray) and perception of the Visitor
/// is on the endpoint (visual effects are applied near the endpoint of the ray)
/// </remarks>
public class HandleRayMovement_FSM : MonoBehaviour
{
    //public XRController handController;
    /*private XRRayInteractor rayInteractor;
    private Pointer pointer;
    
    SinewaveRay sinewaveRay;
    SpiralwaveRay spiralwaveRay;

    
    
    // Finite state machine variables
    private enum HandMovementState
    {
        Initial,
        FirstHalfInitialDirection,
        FirstHalfOppositeDirection,
        SecondHalfOppositeDirection,
        SecondHalfInitialDirection,
    }
    
    private HandMovementState currentState = HandMovementState.Initial;
    private bool isTracking = false;
    //private bool isInitial = true;
    private Vector3 currentHandPosition;
    private Vector3 maxPosition;
    private Vector3 previousHandPosition = Vector3.zero;
    private Vector3 initialDirection = Vector3.zero;
    
    private float previousTime;
    
    


    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(HandleHoverEntered);
        rayInteractor.hoverExited.AddListener(HandleHoverExited);
        
        pointer = rayInteractor.GetComponentInChildren<Pointer>();
    }
    
    
    public void HandleHoverEntered(HoverEnterEventArgs args)
    {
        // Set the amplitude of the ray
        SinewaveRay sine = args.interactable.GetComponentInChildren<SinewaveRay>();
        if (sine != null)
        {
            Debug.Log("SinewaveRay");
            sinewaveRay = sine;
        }
        SpiralwaveRay spiral = args.interactable.GetComponentInChildren<SpiralwaveRay>();
        if (spiral != null)
        {
            spiralwaveRay = spiral;
        }
        
        
        /*initialHandYPosition = pointer.transform.position.y;
        initialHandXPosition = pointer.transform.position.x;#1#
        isTracking = true;
    }
    
    public void HandleHoverExited(HoverExitEventArgs args)
    {
        isTracking = false;
        
        // Reset the amplitude of the ray
        SinewaveRay sine = args.interactable.GetComponent<SinewaveRay>();
        if (sine != null)
        {
            sinewaveRay = null;
        }
        SpiralwaveRay spiral = args.interactable.GetComponent<SpiralwaveRay>();
        if (spiral != null)
        {
            spiralwaveRay = null;
        }
    }
    
    
    
    
    
    
    private void Update()
    {
        if (isTracking)
        {
            if(sinewaveRay != null)
            {
                UpdateSinewaveRay();
            }
            if(spiralwaveRay != null)
            {
                UpdateSpiralwaveRay();
            }
        }
        /*else
        {
            isInitial = true;
        }#1#
    }


    


    // Custom version FSM
    private void UpdateSinewaveRay()
    {
        Vector3 endPointPosition = sinewaveRay.GetEndPoint().position;
        Vector3 pointerPosition = pointer.transform.position;
        currentHandPosition = pointerPosition - endPointPosition;
        
        
        float amplitude = CalculateAmplitude(endPointPosition);
        //sinewaveRay.SetAmplitude(amplitude);
        
        

        switch (currentState)
        {
            case HandMovementState.Initial:
                HandleInitialState(currentHandPosition, endPointPosition, amplitude);
                break;
            case HandMovementState.FirstHalfInitialDirection:
                HandleFirstHalfInitialDirection(currentHandPosition, endPointPosition, amplitude);
                break;
            case HandMovementState.FirstHalfOppositeDirection:
                HandleFirstHalfOppositeDirection(currentHandPosition, endPointPosition, amplitude);
                break;
            case HandMovementState.SecondHalfOppositeDirection:
                HandleSecondHalfOppositeDirection(currentHandPosition, endPointPosition, amplitude);
                break;
            case HandMovementState.SecondHalfInitialDirection:
                HandleSecondHalfInitialDirection(currentHandPosition, endPointPosition, amplitude);
                break;
            /*case HandMovementState.MovingInInitialHalf:
                HandleMovingInInitialHalf(currentHandPosition, endPointPosition);
                break;
            case HandMovementState.MovingInOppositeHalf:
                HandleMovingInOppositeHalf(currentHandPosition, endPointPosition);
                break;#1#
            /*case HandMovementState.MovingInInitialDirection:
                HandleMovingInInitialDirection(currentHandPosition, endPointPosition, amplitude);
                break;
            case HandMovementState.MovingInOppositeDirection:
                HandleMovingInOppositeDirection(currentHandPosition, endPointPosition, amplitude);
                break;#1#
        }
        
        previousHandPosition = currentHandPosition;
        
    }

    private void HandleInitialState(Vector3 currHandPosition, Vector3 endPointPosition, float amplitude)
    {
        initialDirection = new Vector3(Mathf.Sign((currHandPosition - endPointPosition).normalized.x), Mathf.Sign((currHandPosition - endPointPosition).normalized.y), 0);
        /*if (sinewaveRay.IsHorizontal())
        {
            initialDirection = new Vector3(Mathf.Sign((currHandPosition - endPointPosition).normalized.x), 0, 0);
        }
        else
        {
            initialDirection = new Vector3(0, Mathf.Sign((currHandPosition - endPointPosition).normalized.y), 0);
        }#1#
        
        maxPosition = endPointPosition;
        currentState = HandMovementState.FirstHalfInitialDirection;
    }


    private void HandleFirstHalfInitialDirection(Vector3 currHandPosition, Vector3 endPointPosition, float amplitude)
    {
        if (sinewaveRay.IsHorizontal())
        {
            if (Mathf.Abs(currHandPosition.x) >= Mathf.Abs(previousHandPosition.x))
            {
                if (Mathf.Abs(currHandPosition.x) >= Mathf.Abs(maxPosition.x))
                {
                    maxPosition.x = Mathf.Max(Mathf.Abs(currHandPosition.x), Mathf.Abs(maxPosition.x));
                }
                
            }
            else
            {
                if (Mathf.Sign(currHandPosition.x - previousHandPosition.x).Equals(Mathf.Sign(initialDirection.x)))
                {
                    maxPosition.x = Mathf.Max(Mathf.Abs(currHandPosition.x), Mathf.Abs(maxPosition.x));
                    currentState = HandMovementState.FirstHalfOppositeDirection;
                }
                
            }
                
        }
        else
        {
            if (Mathf.Abs(currHandPosition.y) >= Mathf.Abs(previousHandPosition.y))
            {
                if (Mathf.Abs(currHandPosition.y) >= Mathf.Abs(maxPosition.y))
                {
                    maxPosition.y = Mathf.Max(Mathf.Abs(currHandPosition.y), Mathf.Abs(maxPosition.y));
                }
                
            }
            else
            {
                if (Mathf.Sign(currHandPosition.y - previousHandPosition.y).Equals(Mathf.Sign(initialDirection.y)))
                {
                    maxPosition.y = Mathf.Max(Mathf.Abs(currHandPosition.y), Mathf.Abs(maxPosition.y));
                    currentState = HandMovementState.FirstHalfOppositeDirection;
                }
                
            }
                
        }

        sinewaveRay.SetAmplitude(amplitude);
        
        previousHandPosition = currHandPosition;
        
    }
    
    private void HandleFirstHalfOppositeDirection(Vector3 currHandPosition, Vector3 endPointPosition, float amplitude)
    {
        if (sinewaveRay.IsHorizontal())
        {
            if (!Mathf.Sign(currHandPosition.x).Equals(Mathf.Sign(previousHandPosition.x)))
            {
                currentState = HandMovementState.SecondHalfOppositeDirection;
            }

            if (Mathf.Abs(currHandPosition.x) > Mathf.Abs(previousHandPosition.x))
            {
                currentState = HandMovementState.FirstHalfInitialDirection;
            }
        }
        else
        {
            if (!Mathf.Sign(currHandPosition.y).Equals(Mathf.Sign(previousHandPosition.y)))
            {
                currentState = HandMovementState.SecondHalfOppositeDirection;
            }
            
            if (Mathf.Abs(currHandPosition.y) > Mathf.Abs(previousHandPosition.y))
            {
                currentState = HandMovementState.FirstHalfInitialDirection;
            }
        }
        
        
        previousHandPosition = currHandPosition;
        
    }
    
    private void HandleSecondHalfOppositeDirection(Vector3 currHandPosition, Vector3 endPointPosition, float amplitude)
    {
        if (sinewaveRay.IsHorizontal())
        {
            if (Mathf.Abs(currHandPosition.x) < Mathf.Abs(previousHandPosition.x))
            {
                // Valutare se cambiare ampiezza anche qua o se farlo solo nella metà iniziale
                // Se si cambia, solo se è più grande di quella salvata o anche se è più piccola?
                currentState = HandMovementState.SecondHalfInitialDirection;
            }
        }
        else
        {
            if (Mathf.Abs(currHandPosition.y) < Mathf.Abs(previousHandPosition.y))
            {
                // Valutare se cambiare ampiezza anche qua o se farlo solo nella metà iniziale
                // Se si cambia, solo se è più grande di quella salvata o anche se è più piccola?
                currentState = HandMovementState.SecondHalfInitialDirection;
            }
        }
        
        previousHandPosition = currHandPosition;
    }
    
    private void HandleSecondHalfInitialDirection(Vector3 currHandPosition, Vector3 endPointPosition, float amplitude)
    {
        if (sinewaveRay.IsHorizontal())
        {
            if (!Mathf.Sign(currHandPosition.x).Equals(Mathf.Sign(previousHandPosition.x)))
            {
                currentState = HandMovementState.FirstHalfInitialDirection;
            }
            
            if (Mathf.Abs(currHandPosition.x) > Mathf.Abs(previousHandPosition.x))
            {
                currentState = HandMovementState.SecondHalfOppositeDirection;
            }
        }
        else
        {
            if (!Mathf.Sign(currHandPosition.y).Equals(Mathf.Sign(previousHandPosition.y)))
            {
                currentState = HandMovementState.FirstHalfInitialDirection;
            }
            
            if (Mathf.Abs(currHandPosition.y) > Mathf.Abs(previousHandPosition.y))
            {
                currentState = HandMovementState.SecondHalfOppositeDirection;
            }
        }
        
        previousHandPosition = currHandPosition;
    }

    
    
        
    /*private void HandleMovingInInitialDirection(Vector3 currentHandPosition, Vector3 endPointPosition, float amplitude)
    {

        if (currentHalf == Half.First)
        {
            /*if (sinewaveRay.IsHorizontal())
            {
                if (Mathf.Abs(currentHandPosition.x) >= Mathf.Abs(previousHandPosition.x))
                {
                   maxPosition.x = Mathf.Abs(currentHandPosition.x); 
                }
                else
                {
                    currentState = HandMovementState.MovingInOppositeDirection;
                }
                
            }
            else
            {
                if (Mathf.Abs(currentHandPosition.y) >= Mathf.Abs(previousHandPosition.y))
                {
                    maxPosition.y = Mathf.Abs(currentHandPosition.y);
                }
                else
                {
                    currentState = HandMovementState.MovingInOppositeDirection;
                }
                
            }
            
            sinewaveRay.SetAmplitude(amplitude);
            #2#
            
        }
        else
        {
            if (sinewaveRay.IsHorizontal())
            {
                if (!Mathf.Sign(currentHandPosition.x).Equals(Mathf.Sign(previousHandPosition.x)))
                {
                    currentHalf = Half.First;
                }
            }
            else
            {
                if (!Mathf.Sign(currentHandPosition.y).Equals(Mathf.Sign(previousHandPosition.y)))
                {
                    currentHalf = Half.First;
                }
            }
            
        }
        
        
        
        /*float dotProduct = Vector3.Dot(currentHandPosition - maxPosition, initialDirection);
        if (dotProduct > 0)
        {
            maxPosition = currentHandPosition;
        }
        else
        {
            currentState = HandMovementState.MovingInOppositeDirection;
        }#2#
    }
    
    

    private void HandleMovingInOppositeDirection(Vector3 currentHandPosition, Vector3 endPointPosition, float amplitude)
    {

        if (currentHalf == Half.Second)
        {
            if(sinewaveRay.IsHorizontal())
            {
                if (Mathf.Abs(currentHandPosition.x) <= Mathf.Abs(maxPosition.x))
                {
                    //maxPosition.x = Mathf.Abs(currentHandPosition.x);
                    if(Mathf.Abs(currentHandPosition.x) <= Mathf.Abs(previousHandPosition.x))
                    {
                        currentState = HandMovementState.MovingInInitialDirection;
                    }
                }
                else
                {
                    currentState = HandMovementState.MovingInInitialDirection;
                }
            }
            else
            {
                if (Mathf.Sign(currentHandPosition.y) <= Mathf.Sign(maxPosition.y))
                {
                    maxPosition.y = Mathf.Abs(currentHandPosition.y);
                }
                else
                {
                    currentState = HandMovementState.MovingInInitialDirection;
                }
            }
        }
        else
        {
            
        }
        
        
        
        /*float dotProduct = Vector3.Dot(currentHandPosition - maxPosition, -initialDirection);
        if (dotProduct > 0)
        {
            maxPosition = currentHandPosition;
        }
        else
        {
            currentState = HandMovementState.MovingInInitialDirection;
        }#2#
    }#1#
    
    /*private float ComputeSinusoidalWaveDirection(Vector3 currentHandPosition, Vector3 endPointPosition)
    {
        Vector3 direction = currentHandPosition - endPointPosition;
        // Distinguish between horizontal and vertical sinusoidal waves
        if (sinewaveRay.IsHorizontal())
        {
            //float x =
            //return 
        }
        else
        {
            //return 
        }
        
        
        float dotProduct = Vector3.Dot(direction, initialDirection);
        return dotProduct;
    }#1#

    private float CalculateAmplitude(Vector3 endPointPosition)
    {
        if (sinewaveRay.IsHorizontal())
        {
            return Mathf.Abs(pointer.transform.position.x - endPointPosition.x);
        }
        else
        {
            return Mathf.Abs(pointer.transform.position.y - endPointPosition.y);
        }
    }
    
    
    
    
    private void UpdateSpiralwaveRay()
    {
       
    }*/
    
}
