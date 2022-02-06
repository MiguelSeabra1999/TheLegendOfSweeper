using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Health : MonoBehaviour
{
    [SerializeField] private FlashSprite FlashSprite;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private bool _useFlashEffect = false;
    public LayerMask _layerMask;
    public int _maxHP;
    [HideInInspector] public int _currentHP;
    [SerializeField] private PickupReceiver _pickupReceiver;
    [SerializeField] private Movement _movement;


    [HideInInspector] public Action<Direction,float> _knockback;

    void Start()
    {
        _currentHP = _maxHP;
    }
    void OnTriggerEnter(Collider other)
    {
        if(_layerMask == (_layerMask | (1 << other.gameObject.layer)))
        {
            Hitbox hitbox = other.gameObject.GetComponent<Hitbox>();
            if(hitbox == null)
                return;

            if(hitbox.calculateDirection)
                hitbox._direction = GetDirBetween(transform.position, hitbox.transform.position);
            _knockback?.Invoke(hitbox._direction,hitbox._knockback);
            

            if(_pickupReceiver != null)
            {
                if(_pickupReceiver.HasShield == 1)
                {
                    if(GetOppositeDirection(_movement._direction) == hitbox._direction)
                        return;

                }
            }
            Damage(hitbox._damage);

        }
    }

    public void Damage(int damage)
    {
        _currentHP -= damage;
        FlashSprite?.StartFlashEffect();
        if(_useFlashEffect)
            PostProcessorInterface.DamageEffect(0.2f);
//        Debug.Log("ouvh");
        if(healthBar != null)
        {
            healthBar.Damage();
        }
        if(_currentHP < 0)
            Destroy(gameObject);
    }
    private Direction GetDirBetween(Vector3 a, Vector3 b)
    {
        if(Mathf.Abs(a.x-b.x) < Mathf.Abs(a.z-b.z))
        {
            if(b.z > a.z)
                return Direction.down;
            else
                return Direction.up;
        }
        else
        {
            if(b.x > a.x)
                return Direction.right;
            else
                return Direction.left;
        }
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.down:
            return Direction.up;
            case Direction.up:
            return Direction.down;
            case Direction.left:
            return Direction.right;
            case Direction.right:
            return Direction.left;
        }
        return Direction.down;
    }

    public void Heal()
    {
        if(_currentHP < _maxHP)
        {
            _currentHP++;
            if(healthBar != null)
            {
                healthBar.Heal();
            }
        }
    }
}
