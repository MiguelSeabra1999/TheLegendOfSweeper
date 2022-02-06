using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private Board _board;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _player;
    [SerializeField] private Movement _playerMovement;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private float _rebuildDuration;
    [SerializeField] private Transform  _goalPos;
    [SerializeField] private float  _circleSpeed = 1;
    [SerializeField] private float  _minCircleRadius = 10;
    [SerializeField] private float  _maxCircleRadius = 3;
    [SerializeField] private float  _radDuration = 3;
    [SerializeField] private float  _shootThreshold = 0.1f;
    [SerializeField] private float  _shootChance = 0.8f;
    [SerializeField] private float  _restTime = 1f;
    [SerializeField] private GameObject  _wolfPrefab;
    [SerializeField] private UImanager  UImanager;
    private bool _canShoot = true;
    private bool _firstEnable = true;
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnEnable() 
    {
        base.OnEnable();
        if(!_firstEnable)
            BeginBattle();
        _firstEnable = false;
    }
    private void BeginBattle()
    {
        Invoke("Rebuild",1f);
    }

    [ContextMenu("Rebuild")]
    private void Rebuild()
    {
        StartCoroutine(RebuildRoutine());
    }
      [ContextMenu("CirclePlayer")]
    private void CirclePlayer()
    {
        
        StartCoroutine(CirclePlayerRoutine());
    }
    private IEnumerator RebuildRoutine()
    {
        _animator.Play("Idle");
        _playerMovement.Sleep();
        Vector3 playerStartPos = _player.transform.position;
        Vector3 startPos = transform.position;
        float startTime = Time.time;
        float percent = 0;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_rebuildDuration;

            Vector3 playerPos = Vector3.Lerp(playerStartPos,_goalPos.position,percent);
           // playerPos = new Vector3(playerPos.x, playerStartPos.y + _jumpCurve.Evaluate(percent),playerPos.z);
            _player.transform.position = playerPos;

            Vector3 myPos = Vector3.Lerp(startPos,_goalPos.position+Vector3.back*4,percent);
            myPos = new Vector3(myPos.x, myPos.y + _jumpCurve.Evaluate(percent),myPos.z);
            transform.position = myPos;
            yield return null;
        }
        _player.transform.position = _goalPos.position;
        yield return new WaitForSeconds(0.1f);

        _board.ForceRandom = true;
        _animator.Play("Hide");
        _board.RechargeBoxes();
        _board.Clear();
        _board.RechargeBoxes();
        yield return new WaitForSeconds(0.5f);

        _playerMovement.StopSleep();
        StartCoroutine(CirclePlayerRoutine());
    }

    private IEnumerator CirclePlayerRoutine()
    {
         _animator.Play("Idle");
        float startTime = Time.time;
        float percent = 0;
        while(percent < 1)
        {
            percent = (Time.time-startTime)/_radDuration;
            float angle = Time.time*_circleSpeed;
            float circleRadius = Mathf.Lerp(_maxCircleRadius,_minCircleRadius,percent);
            float offsetX = circleRadius * Mathf.Cos(angle);
            float offsetZ = circleRadius * Mathf.Sin(angle);
            Vector3 playerOffset = new Vector3(offsetX,transform.position.y, offsetZ);
            transform.position = _player.transform.position + playerOffset;

            //Shoot
            if(_canShoot)
            {
                if(Mathf.Abs(transform.position.z - _player.transform.position.z) < _shootThreshold)
                {
                    if( Random.Range(0f,1f) < _shootChance)
                    {
                        if(transform.position.x > _player.transform.position.x) 
                            Shoot(Direction.left);
                        else
                            Shoot(Direction.right);
                    }
                    _canShoot = false;
                    Invoke("RestoreShot",0.5f);
                }
            }
            yield return null;
        }
        StartCoroutine(Rest());
    }
    private IEnumerator Rest()
    {
     
        Vector3 startPos = transform.position;
        float startTime = Time.time;
        float percent = 0;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/2f;

            Vector3 myPos = Vector3.Lerp(startPos,_goalPos.position,percent);
            myPos = new Vector3(myPos.x, myPos.y + _jumpCurve.Evaluate(percent),myPos.z);
            transform.position = myPos;

            yield return null;
        }
        _animator.Play("Tired");
        yield return new WaitForSeconds(_restTime);
        StartCoroutine(RebuildRoutine());
    }
    private void RestoreShot()
    {
        _canShoot = true;
    }
    private void Shoot(Direction direction)
    {
        _audioManager.PlaySFX(0);
        GameObject wolf = Instantiate(_wolfPrefab,transform.position + DirectionToVec(direction)*0.5f, Quaternion.identity);
        Hitbox wolfHitbox = wolf.GetComponent<Hitbox>();
        Projectile wolfProjectile = wolf.GetComponent<Projectile>();
        wolfProjectile.Direction = DirectionToVec(direction);
        wolfHitbox._direction = direction;
    }
    void OnDestroy()
    {
        if(UImanager == null)
            return;
       UImanager.SetWin();
        if(_audioManager == null)
            return;
        _audioManager.PlaySFX(7);
        _audioManager.PlayMusic(0);
    }
    
}
