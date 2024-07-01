using System.Net;
using UnityEngine;

[CreateAssetMenu(fileName = "UDP Event", menuName = "Scriptable Objects/Events/UDP Event")]
public class UDPEventSO : ScriptableObject
{
    public delegate void UDPEventHandler(string message, IPEndPoint endpoint);
    private event UDPEventHandler OnEventRaised;

    public void RaiseEvent(string message, IPEndPoint endpoint)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(message, endpoint);
    }

    public void RegisterListener(UDPEventHandler listener)
    {
        OnEventRaised += listener;
    }

    public void UnregisterListener(UDPEventHandler listener)
    {
        OnEventRaised -= listener;
    }
}