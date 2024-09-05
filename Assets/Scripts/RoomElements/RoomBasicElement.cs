using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class RoomBasicElement : MonoBehaviour
{
    
    [SerializeField] protected EndPointSO endPointSO;
    //[SerializeField] protected UDPManager udpManager;

    
    protected virtual void Start()
    {
        Debug.Log("PORCODIO basic: " + endPointSO.EndPoint);;
        UDPManager.Instance.onMessageReceived.AddListener(OnMessageReceived);
    }
    
    private void OnDestroy()
    {
        UDPManager.Instance.onMessageReceived.RemoveListener(OnMessageReceived);
    }
    
    public void OnMessageReceived(string message, IPEndPoint ipAddress)
    {
        Debug.Log("End point: " + ipAddress + " ma mio endpoint: " + endPointSO.EndPoint);
        if (ipAddress.Equals(endPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(char[] message, IPEndPoint ipAddress)
    {
        if (ipAddress.Equals(endPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(byte[] message, IPEndPoint ipAddress)
    {
        if (ipAddress.Equals(endPointSO.EndPoint))
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
