using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oasis.GameEvents
{
    // THE LISTENER
    public class StringGameEventListener : ValueGameEventListener<string>
    {
        
        [SerializeField] private UnityEvent<string> _unityEvent;
        [SerializeField] private ValueGameEventSO<string> _gameEvent;
        
        private void OnEnable() => _gameEvent.Subscribe(this);
        private void OnDisable() => _gameEvent.Unsubscribe(this);
        
        public void RaiseEvent(string value) => _unityEvent?.Invoke(value);
        
        
    }
}