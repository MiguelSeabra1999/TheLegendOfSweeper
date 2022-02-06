


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(KeyReader))]
[RequireComponent(typeof(Movement))]
public class Attack : MonoBehaviour
{
    public int _damage;
    public float _knockback;
    public float _cooldown = 0.2f;
    public GameObject HitboxPrefab;
    [SerializeField] private KeyReader KeyReader;
    [SerializeField] private Movement Movement;
    private Board Board;
    public Action _attack;
    public Action _grab;
    private bool _canAttack = true;
    private Coroutine _attackCooldownRoutine;
     private AudioManager _audioManager;
    void Awake()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Board = GameObject.Find("Board").GetComponent<Board>();
    }
    void Start()
    {

         SubscribeToEvents();
    }
    void OnEnable()
    {
        
        SubscribeToEvents();
    }
    void OnDisable()
    {

        UnsubscribeToEvents();
    }
    private void SubscribeToEvents()
    {
        if(KeyReader._keys.Count == 0)
            return;

        KeyReader._keys[KeyCode.Mouse0].actionDown   += StartAttack;
        KeyReader._keys[KeyCode.Mouse1].actionDown   += StartFlag;
       
    }
    private void UnsubscribeToEvents()
    {
        if(KeyReader._keys.Count == 0)
            return;
        KeyReader._keys[KeyCode.Mouse0].actionDown   -= StartAttack;
        KeyReader._keys[KeyCode.Mouse1].actionDown   -= StartFlag;
    }

    private void StartFlag()
    {
        Vector3 attackPosWorld = transform.position + CalcOffset();
        Board.CheckClickWorldPos(false, attackPosWorld);
        _grab?.Invoke();
    }
    private void StartAttack()
    {
        if(!_canAttack)
            return;
        Vector3 offset = CalcOffset();
        Vector3 attackPosWorld = transform.position + offset;
        Board.CheckClickWorldPos(true, attackPosWorld);
        _attack?.Invoke();
        SpawnHitbox(attackPosWorld + offset*0.2f + Vector3.forward*-0.2f);
        _canAttack = false;
        if(_attackCooldownRoutine != null)
            StopCoroutine(_attackCooldownRoutine);
        _attackCooldownRoutine = StartCoroutine(AttackCooldown());
         _audioManager.PlaySFX(9);
        
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _canAttack = true;
    }

    private void SpawnHitbox(Vector3 pos)
    {
        GameObject hitbox = Instantiate(HitboxPrefab, pos, Quaternion.identity);
        hitbox.GetComponent<Hitbox>().Setup(Movement._direction,_damage,_knockback,LayerMask.NameToLayer("PlayerHitbox"));
        hitbox.transform.SetParent(transform.root);
    }

    private Vector3 CalcOffset()
    {
        Vector3 offset = Vector3.zero;
        switch(Movement._direction)
        {
            case(Direction.down):
                offset = new Vector3(0,0,-1);
            break;
            case(Direction.left):
                offset = new Vector3(-1,0,0);
            break;
            case(Direction.right):
                offset = new Vector3(1,0,0);
            break;
            case(Direction.up):
                offset = new Vector3(0,0,1);
            break;
            default:
            break;
        }
        return offset;
    }
}
