using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string _message;
    [SerializeField] private int sound;
    private AudioManager _audioManager;
    
    void Awake()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

    }
    void OnTriggerEnter(Collider other)
    {
        _audioManager.PlaySFX(sound);
        other.SendMessage(_message);
        Destroy(gameObject);
    }
}
