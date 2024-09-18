using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class RoomBasicElement : MonoBehaviour
{
    
    [SerializeField] protected EndPointSO sendingEndPointSO;
    [SerializeField] protected EndPointSO receivingEndPointSO;
    //[SerializeField] protected UDPManager udpManager;
    
    
    protected int[] messageContent;
    protected int[] lastMessage;
    protected string separator = ":";
    
    protected virtual void Start()
    {
        Debug.Log("PORCODIO basic: " + receivingEndPointSO.EndPoint);;
        UDPManager.Instance.onMessageReceived.AddListener(OnMessageReceived);
    }
    
    private void OnDestroy()
    {
        UDPManager.Instance.onMessageReceived.RemoveListener(OnMessageReceived);
    }
    
    public void OnMessageReceived(string message, IPEndPoint ipAddress)
    {
        Debug.Log("End point: " + ipAddress + " ma mio endpoint: " + receivingEndPointSO.EndPoint);
        if (ipAddress.Equals(receivingEndPointSO.EndPoint))
        {
            Debug.Log("Eseguiamo azione dopo messaggio da " + ipAddress);
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(char[] message, IPEndPoint ipAddress)
    {
        if (ipAddress.Equals(receivingEndPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    public void OnMessageReceived(byte[] message, IPEndPoint ipAddress)
    {
        if (ipAddress.Equals(receivingEndPointSO.EndPoint))
        {
            ExecuteMessageResponse(message);
        }
    }
    
    
    protected abstract void ExecuteMessageResponse(string message);
    
    protected abstract void ExecuteMessageResponse(byte[] message);
    
    protected abstract void ExecuteMessageResponse(char[] message);
    
    
    public EndPointSO GetEndPointSO()
    {
        return sendingEndPointSO;
    }
    
    /*public UDPManager GetUDPManager()
    {
        return udpManager;
    }*/

}
