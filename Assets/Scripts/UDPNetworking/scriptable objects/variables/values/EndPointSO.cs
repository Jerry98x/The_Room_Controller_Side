
using System.Net;
using UnityEngine;


[CreateAssetMenu(fileName = "EndPoint SO", menuName = "Scriptable Objects/Variables/Values/EndPoint")]
public class EndPointSO : ScriptableObject
{
    [SerializeField] private string stringIP;
    [SerializeField] private int port;
    [SerializeField] private string uiName;
    
    private IPEndPoint _endPoint;
    private IPAddress _ip;

    public string UIName => uiName;
    
    public IPEndPoint EndPoint
    {
        get
        {
            if (_endPoint == null)
                // if not initialized, initialize it
                Debug.Log("Indirizzo IP da sottoclasse IP: " + IP);
                _endPoint = UDPMessenger.GetIPEndPoint(IP, Port);
                Debug.Log("Indirizzo IP dopo costruttore da variabile _endPoint: " + _endPoint);
            return _endPoint;
        }
    }
        
    public IPAddress IP
    {
        get
        {
            if (_ip == null)
                // if not initialized, initialize it
                Debug.Log("Indirizzo IP da SerializeField: " + stringIP);
                _ip = System.Net.IPAddress.Parse(stringIP);
                Debug.Log("Indirizzo IP da variabile _ip: " + _ip);
            return _ip;
        }
    }

    public int Port => port;
}
