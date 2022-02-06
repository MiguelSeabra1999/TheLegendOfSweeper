using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class KeyInfo
{
    public bool state = false;
    public Action actionDown;    
    public Action actionUp;
}
public class KeyReader : MonoBehaviour
{
    [SerializeField] private List<KeyCode> _usedKeys;
    public Dictionary<KeyCode,KeyInfo> _keys = new Dictionary<KeyCode,KeyInfo> ();
    private bool firstFrame = true;
    void Awake()
    {
        foreach(KeyCode key in _usedKeys)
        {
//            Debug.Log("adding key: " + key);
            _keys.Add(key, new KeyInfo());
        }
    }
    void OnEnable()
    {
        if(firstFrame)
        {
            firstFrame = false;
            return;
        }
        ReCheckKeys();
    }
    void OnDisable()
    {
        LetGoAllKeys();
    }
    void Update()
    {
        GetKeys();
    }

    void GetKeys()
    {
        foreach( KeyValuePair<KeyCode, KeyInfo> kvp in _keys )
        {
            if(Input.GetKeyDown(kvp.Key))
            {
                _keys[kvp.Key].actionDown?.Invoke();
                _keys[kvp.Key].state = true;

            }
            else if(Input.GetKeyUp(kvp.Key))
            {
                _keys[kvp.Key].actionUp?.Invoke();
                _keys[kvp.Key].state = false;

            }
        }
    }
    void ReCheckKeys()
    {
        foreach( KeyValuePair<KeyCode, KeyInfo> kvp in _keys )
        {
            if(Input.GetKey(kvp.Key) && !_keys[kvp.Key].state)
            {
                _keys[kvp.Key].actionDown?.Invoke();
                _keys[kvp.Key].state = true;

            }
            else if(!Input.GetKey(kvp.Key)  && _keys[kvp.Key].state)
            {
                _keys[kvp.Key].actionUp?.Invoke();
                _keys[kvp.Key].state = false;

            }
        }
    }
    void LetGoAllKeys()
    {
        foreach( KeyValuePair<KeyCode, KeyInfo> kvp in _keys )
        {
            if(_keys[kvp.Key].state)
            {
                _keys[kvp.Key].actionUp?.Invoke();
                _keys[kvp.Key].state = false;

            }
        }
    }

}
