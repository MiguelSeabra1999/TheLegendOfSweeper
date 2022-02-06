
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum Direction{down,left,right,up};
[RequireComponent(typeof(KeyReader))]
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] private KeyReader KeyReader;
    [SerializeField] private Rigidbody rb;
    public float _speed = 0.07f;
    private Vector2 _movement = Vector2.zero;
    public Vector2 _normalizedMovement = Vector2.zero;
    public Action _startMoving;
    public Action _stopMoving;
    public Action<Vector2> _movementChange;
    private bool _moving;
    public Direction _direction;
    private bool _sleeping;
    private bool _maintainMovement;
    private Vector2 _bufferedMovement;

    void Start()
    {
       // SubscribeToEvents();
    }

    void OnEnable()
    {
       // SubscribeToEvents();
    }
    void OnDisable()
    {

        //UnsubscribeToEvents();
    }
    void Update()
    {
        CheckKeys();
    }
    void FixedUpdate() {
        if(!_sleeping)
            Move();
    }
    private void Move()
    {
        _normalizedMovement = _movement.normalized;
        Vector3 movement;
        if(!_maintainMovement)
        {
            movement = new Vector3(_normalizedMovement.x,0,_normalizedMovement.y) * _speed;
            rb.MovePosition(rb.position + movement);
            UpdateMovingEvents(movement.magnitude);
        }else
        {
            movement = new Vector3(_bufferedMovement.x,0,_bufferedMovement.y) * _speed;
            rb.MovePosition(rb.position + movement);
            UpdateMovingEvents(_bufferedMovement.magnitude);
        }
    }

    private void UpdateMovingEvents(float movementMagnitude)
    {
        if(movementMagnitude > 0.01f)
        {
            if(!_moving)
            {
                _startMoving?.Invoke();
                _moving = true;
            }
        }
        else
        {
            if(_moving)
            {
                _stopMoving?.Invoke();
                _moving = false; 
            }
        }
    }
    private void MovementChange()
    {
        
        if(_movement.x == 0 && _movement.y != 0)
        {
            if(_movement.y > 0)
                _direction = Direction.up;
            else
                _direction = Direction.down;
        }
        else if(_movement.y == 0 && _movement.x != 0)
        {
            if(_movement.x > 0)
                _direction = Direction.right;
            else
                _direction = Direction.left;
        }
        _movementChange?.Invoke(_movement);
    }
    private void OnApplicationFocus (bool paused)
    {

            _movement = Vector3.zero;
    }
    private void CheckKeys()
    {
        //_movement
        Vector2 movement = Vector2.zero;
        if(KeyReader._keys[KeyCode.W].state)
        {
            movement.y += 1;
        }
        if(KeyReader._keys[KeyCode.S].state)
        {
            movement.y -= 1;
        }
        if(KeyReader._keys[KeyCode.A].state)
        {
            movement.x -= 1;
        }
        if(KeyReader._keys[KeyCode.D].state)
        {
            movement.x += 1;
        }
        if(movement != _movement)
        {
            _movement = movement;
            MovementChange();
        }
    }
    
    /*
    #region EventHandling

    private void SubscribeToEvents()
    {
        if(KeyReader._keys.Count == 0)
            return;

        KeyReader._keys[KeyCode.W].actionDown   += PressUp;
        KeyReader._keys[KeyCode.W].actionUp     += ReleaseUp;
        KeyReader._keys[KeyCode.A].actionDown   += PressLeft;
        KeyReader._keys[KeyCode.A].actionUp     += ReleaseLeft;
        KeyReader._keys[KeyCode.S].actionDown   += PressDown;
        KeyReader._keys[KeyCode.S].actionUp     += ReleaseDown;
        KeyReader._keys[KeyCode.D].actionDown   += PressRight;
        KeyReader._keys[KeyCode.D].actionUp     += ReleaseRight;  
    }
    private void UnsubscribeToEvents()
    {
        if(KeyReader._keys.Count == 0)
            return;
        KeyReader._keys[KeyCode.W].actionDown   -= PressUp;
        KeyReader._keys[KeyCode.W].actionUp     -= ReleaseUp;
        KeyReader._keys[KeyCode.A].actionDown   -= PressLeft;
        KeyReader._keys[KeyCode.A].actionUp     -= ReleaseLeft;
        KeyReader._keys[KeyCode.S].actionDown   -= PressDown;
        KeyReader._keys[KeyCode.S].actionUp     -= ReleaseDown;
        KeyReader._keys[KeyCode.D].actionDown   -= PressRight;
        KeyReader._keys[KeyCode.D].actionUp     -= ReleaseRight;
    }

    private void PressUp()
    {
        if(_movement.y != 1)
            _movement.y += 1;
        MovementChange();
    }

    private void ReleaseUp()
    {
        _movement.y -= 1;
        MovementChange();
    }

    private void PressLeft()
    {
        if(_movement.x != -1)
            _movement.x -= 1;
        MovementChange();
    }

    private void ReleaseLeft()
    {
        _movement.x += 1;
        MovementChange();
    }

    private void PressDown()
    {
        if(_movement.y != -1)
            _movement.y -= 1;
        MovementChange();
    }

    private void ReleaseDown()
    {
        _movement.y += 1;
        MovementChange();
    }

    private void PressRight()
    {
        if(_movement.x != 1)
            _movement.x += 1;
        MovementChange();
    }

    private void ReleaseRight()
    {
        _movement.x -= 1;
        MovementChange();
    }
    #endregion
*/
    public void MoveAndSleep(float duration)
    {
        _maintainMovement = true;
        _bufferedMovement = _movement.normalized*0.5f;
        Invoke("Sleep",duration/2);
        Invoke("StopSleep",duration);
//        Debug.Log(_bufferedMovement);
    }   
    public void Sleep()
    {
        _maintainMovement = false;
        _sleeping = true;

    }   
    public void StopSleep()
    {
        _maintainMovement = false;
        _sleeping = false;
    } 
    
}
