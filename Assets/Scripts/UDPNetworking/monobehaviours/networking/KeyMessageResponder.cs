
using Oasis.GameEvents;
using UnityEngine;


public abstract class KeyMessageResponder : KeyValueGameEventListener
{
    [Space]

    [Header("key")]
    
        [SerializeField] protected StringSO dofKey;
        [SerializeField] protected string key;

    [Header("responses")]
    
        [SerializeField] protected bool listenToString;

    
    private void Awake()
    {
        if (dofKey != null)
            key = dofKey.runtimeValue;
    }

    public void SetKey(string newKey) => 
        key = newKey;
    
    public void OnKeyValueMsgReceived(KeyValueMsg keyValueMsg)
    {
        // Debug.Log($"[KeyMessageResponder][OnKeyValueMsgReceived] - k: {keyValueMsg.key} - sv: {keyValueMsg.stringValue} / MY key: {dofKey.runtimeValue} / same Key? {dofKey.runtimeValue == keyValueMsg.key}");
        
        if (key == keyValueMsg.key)
        {
            if (keyValueMsg.success)
                MessageResponse(keyValueMsg.value);
            
            if (listenToString)
                StringMessageResponse(keyValueMsg.stringValue);
        }
    }
    
    protected abstract void MessageResponse(float val);

    protected abstract void StringMessageResponse(string val);
}