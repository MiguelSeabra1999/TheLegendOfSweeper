using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public LayerMask _possibleTargets;
    public Direction _direction;
    public int _damage;
    public float _knockback;
    public bool calculateDirection = false;
    public bool destroy = true;
    public void Setup(Direction direction, int damage, float knockback, int layer)
    {
        _direction = direction;
        _damage = damage;
        _knockback = knockback;
        gameObject.layer = layer;
    }
    void OnTriggerEnter(Collider other)
    {
        if(!destroy)
            return;
        if(_possibleTargets == (_possibleTargets | (1 << other.gameObject.layer)))
            if(other.gameObject.layer != gameObject.layer)
                Destroy(gameObject);
    }
}