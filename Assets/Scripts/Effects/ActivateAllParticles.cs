using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAllParticles : MonoBehaviour
{
    private List<ParticleSystem> _particles;
    public int sound = -1;
    protected AudioManager _audioManager;
    void Awake()
    {
         GameObject obj = GameObject.Find("AudioManager");
         if(obj == null)
         {
            Destroy(this);
            return;
         }
         _audioManager = obj.GetComponent<AudioManager>();
        _particles = GetComponentsInChildren<ParticleSystem>().ToList();
    }
    [ContextMenu("ActivateAll")]
    public void Activate()
    {
        foreach(ParticleSystem ps in _particles)
        {
            ps.Play();
        }
        if(_audioManager != null && sound != -1)
            _audioManager.PlaySFX(sound);
    }
}
