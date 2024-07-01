using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomBasicElement : MonoBehaviour
{
    
    [SerializeField] protected EndPointSO endPointSO;
    [SerializeField] protected UDPManager udpManager;

    
    private void Start()
    {
        udpManager.onMessageReceived.AddListener(OnMessageReceived);
    }
    
    private void OnDestroy()
    {
        udpManager.onMessageReceived.RemoveListener(OnMessageReceived);
    }
    
    public void OnMessageReceived(string message, EndPointSO endPointSO)
    {
        if (endPointSO == this.endPointSO)
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(char[] message, EndPointSO endPointSO)
    {
        if (endPointSO == this.endPointSO)
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(byte[] message, EndPointSO endPointSO)
    {
        if (endPointSO == this.endPointSO)
        {
            ExecuteMessageResponse(message);
        }
    }
    
    
    protected abstract void ExecuteMessageResponse(string message);
    
    protected abstract void ExecuteMessageResponse(byte[] message);
    
    protected abstract void ExecuteMessageResponse(char[] message);
    
    
    public EndPointSO GetEndPointSO()
    {
        return endPointSO;
    }
    
    public UDPManager GetUDPManager()
    {
        return udpManager;
    }

}
