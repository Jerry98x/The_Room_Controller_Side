
using System.Collections;
using System.Net;
using Core;
using Oasis.GameEvents;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class MessageReceivedEvent : UnityEvent<byte[], IPEndPoint> {}

public class UDPManager : Monosingleton<UDPManager>
{
    
    // Event
    public MessageReceivedEvent onMessageReceived;
    
    
    
    
    [Header("Network setup")]
    
    [SerializeField] private EndPointSO _defaultEndpoint;
    public int myPort = 12345; // UDP port to listen on
    
    [Tooltip("Max life of unread udp messages, in MINUTES")]
    [SerializeField] private int maxUdpAge = 2;

    [Tooltip("Max amount of unread UDP messages")]
    [SerializeField] private int bufferSize = 1;
    
    [SerializeField] private KeyValueGameEventSO onKeyValueReceived;
    [SerializeField] private StringGameEventSO onStringReceived;
    
    private UDPMessenger _udpMessenger;

    private byte[] _data;
    public byte[] Data { get => _data; }
    
    private bool _initialized;
    
    
    protected override void Init()
    {
        SetInitialized(true);
        
        // GENERAL
        _udpMessenger = new UDPMessenger();
        _udpMessenger.Init(_defaultEndpoint.EndPoint, myPort, maxMsgAge: maxUdpAge, bufferSize: bufferSize);

        StartCoroutine(CheckOldMessages());
    }

    public bool IsEndpointMyEndpoint(IPEndPoint compareEndpoint)
    {
        Debug.Log($"[UDPManager][IsEndpointMyEndpoint] - local endpoint address: {_udpMessenger.MyEndpoint.Address} - compare endpoint address: {compareEndpoint.Address}");
        
        return _udpMessenger.MyEndpoint.Equals(compareEndpoint);
    }
    
    private void SetInitialized(bool init)
    {
        Debug.Log($"[UDPMANAGER] - SET FLAG DIO CANE PORCO DIO - flag was: '{_initialized}' - now is: '{init}'");
        _initialized = init;
    }
    
    private IEnumerator CheckOldMessages()
    {
        // check old messages every two minutes
        while (true)
        {
            _udpMessenger.TryRemoveOldMessages();
            
            yield return new WaitForSeconds(2000);
        }
    }
    
    public void SendStringUdpToDefaultEndpoint(string message)
    {
        // only works if INITIALIZED, meaning, if game has started
        // Debug.Log($"[UDP MANAGER][SendStringUpdToRasp] - INIT: '{_initialized}'");
        if (_initialized)
        {
            // Debug.Log($"[UDP MANAGER] - sending message: '{message}' to RASP");
            _udpMessenger.SendUdp(message, _defaultEndpoint.EndPoint);
        }
    }
    
    public void SendStringUdp(string message, IPEndPoint endPoint)
    {
        // only works if INITIALIZED, meaning, if game has started
        // Debug.Log($"[UDP MANAGER][SendStringUpdToRasp] - INIT: '{_initialized}'");
        if (_initialized)
        {
            // Debug.Log($"[UDP MANAGER] - sending message: '{message}' to RASP");
            _udpMessenger.SendUdp(message, endPoint);
        }
    }
    
    public void SendByteArrayUdp(byte[] message, IPEndPoint endPoint)
    {
        // only works if INITIALIZED, meaning, if game has started
        // Debug.Log($"[UDP MANAGER][SendStringUpdToRasp] - INIT: '{_initialized}'");
        if (_initialized)
        {
            // Debug.Log($"[UDP MANAGER] - sending message: '{message}' to RASP");
            _udpMessenger.SendUdp(message, endPoint);
        }
    }

    public void SendIntUdp(int[] message, IPEndPoint endPoint)
    {
        // only works if INITIALIZED, meaning, if game has started
        // Debug.Log($"[UDP MANAGER][SendStringUpdToRasp] - INIT: '{_initialized}'");
        if (_initialized)
        {
            // Debug.Log($"[UDP MANAGER] - sending message: '{message}' to RASP");
            _udpMessenger.SendUdp(message, endPoint);
        }
    }
    
    void Update()
    {
        if (!_initialized)
            return;
        
        ReceiveMessages();
    }

    public byte[] TryGetFrame()
    {
        if (_udpMessenger.UnreadMsgsPresent)
        {
            var messages = _udpMessenger.UnreadUdpMessages;

            // return the last message
            return messages[messages.Count - 1].RawMsg;
        }
        
        return null;
    }
    
    private void ReceiveMessages()
    {
        Debug.Log("RICEZIONE: inizio");
        if (_udpMessenger.UnreadMsgsPresent)
        {
            Debug.Log("RICEZIONE: ci sono messaggi non letti");
            var messages = _udpMessenger.UnreadUdpMessages;

            foreach (var message in messages)
            {
                
                /*
                // Create an instance of EndPointSO using ScriptableObject.CreateInstance
                var endPointSO = ScriptableObject.CreateInstance<EndPointSO>();
                endPointSO.EndPoint = message.Sender;
                Debug.Log("RICEZIONE: istanziato un nuovo EndPointSO con IP: " + endPointSO.EndPoint.Address);
                */

                // Invoke the event with the message's raw data and the EndPointSO instance
                onMessageReceived.Invoke(message.RawMsg, message.Sender);

                
                
                //TODO: not using key-value messages anymore
                // Upon receiving a message, call an event passing message and endpoint
                //onMessageReceived.Invoke(message.RawMsg, _defaultEndpoint);
                //onMessageReceived.Invoke(message.Msg.ToCharArray(), _defaultEndpoint);
                
                
                //onMessageReceived.Invoke(message.Msg, _defaultEndpoint);

                
                
                
                // check if it's a key-value message
                // otherwise, store the raw data in the input buffer, to be used by the UdpCameraViewer
                /*if (!CheckKeyValueMessage(message.Msg))
                {
                    // Debug.Log($"[UDP MANAGER] - RECEIVED RAW MESSAGE: {message.RawMsg} - {message.RawMsg.Length}");
                    _data = message.RawMsg;
                }*/
            }
        }
    }

    
    //TODO: write a method that doesn't use a key-value message, but just a raw message, because we 
    //TODO: need to use as less space as possible on the ESP32 side
    
    private bool CheckStringMessage(string msg)
    {
        // Check if it's a string message and if the string is in the right format and number of bytes
        if (msg.Length < 1)
            return false;
        
        // ON STRING MSG GE
        onStringReceived.Invoke(msg);
        return true;
    }
    
    
    private bool CheckKeyValueMessage(string msg)
    {
        var keyValMsg = KeyValueMsg.ParseKeyValueMsg(msg);

        // Debug.Log($"  ---[CheckKeyValueMessage] - string msg: '{msg}' - KEY VALUE MESSAGE: {keyValMsg} - is it NONE? {keyValMsg == null} - {keyValMsg?.key} - val: {keyValMsg?.value} - string val: {keyValMsg?.stringValue}");
        //
        // NB it expects messages one by one, no aggregation; 
        // TODO to add aggregation, "parsekeyvalue" needs to be a method from a helper, that tries to gen N K:V messages from one msg
        // TODO and returns a list of K:V messages; if empty, return false; otherwise, parse one by one. 
        
        if (keyValMsg != null)
        {
            Debug.Log($"  ---[CheckKeyValueMessage] - string msg: '{msg}' - KEY VALUE MESSAGE: {keyValMsg} - key: {keyValMsg.key} - val: {keyValMsg.value} - string val: {keyValMsg.stringValue}");

            // ON KEY VALUE MSG GE
            onKeyValueReceived.Invoke(keyValMsg);
            return true;
        }

        return false;
    }
    
    public void OnDisable()
    {
        _udpMessenger.ClosePorts();
    }

    public void OnApplicationQuit()
    {
        _udpMessenger.ClosePorts();
        
        //end thread
        _initialized = false;
    }
}