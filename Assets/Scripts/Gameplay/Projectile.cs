using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private bool _flipSprite = false;
    [SerializeField] private SpriteRenderer _sprite;
    public Vector3 Direction;
    public float Speed;
    public Rigidbody rb;
    protected AudioManager _audioManager;
    void Awake()
    {
        
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Start()
    {
        if(_flipSprite)
        {
            if(Direction == Vector3.right)
                _sprite.flipX = true;
            if(Direction == Vector3.left)
                 _sprite.flipX = false;
        }
    }
    void FixedUpdate() {
        rb.velocity = Direction*Speed;
    }
    void OnTriggerEnter(Collider other)
    {
        if(~_ignoreLayer == (~_ignoreLayer | (1 << other.gameObject.layer)))
            Destroy(gameObject);
    }
    void OnDestroy()
    {
        _audioManager.PlaySFX(6);
    }

}
