using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class RoomBasicElement : MonoBehaviour
{
    
    [SerializeField] protected EndPointSO endPointSO;
    //[SerializeField] protected UDPManager udpManager;

    
    private void Start()
    {
        UDPManager.Instance.onMessageReceived.AddListener(OnMessageReceived);
    }
    
    private void OnDestroy()
    {
        UDPManager.Instance.onMessageReceived.RemoveListener(OnMessageReceived);
    }
    
    public void OnMessageReceived(string message, IPEndPoint endPointSo)
    {
        if (endPointSo.Equals(endPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(char[] message, IPEndPoint endPointSo)
    {
        if (endPointSo.Equals(endPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(byte[] message, IPEndPoint endPointSo)
    {
        if (endPointSo.Equals(endPointSO.EndPoint))
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
    
    /*public UDPManager GetUDPManager()
    {
        return udpManager;
    }*/

}
