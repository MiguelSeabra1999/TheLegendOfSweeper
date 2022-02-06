using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : MonoBehaviour
{
     [SerializeField] protected float _changeDirectionChance = 0.2f;
    
    [SerializeField]protected float _speed;
    [SerializeField]protected float _knockbackSpeed;
    
    [SerializeField]protected Health Health;
    [SerializeField]protected Rigidbody rb;

    protected GameObject _target;
   // protected Board _board;
    protected int _row;
    protected int _column;
    protected Coroutine _moveRoutine;
    protected Direction _movementDir = Direction.left;
    protected Quaternion _goalRot;
    protected AudioManager _audioManager;

    void Awake()
    {
        Health._knockback += ApplyKnockback;
         _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    protected virtual void OnEnable()
    {
        Health._knockback += ApplyKnockback;
    }
    void OnDisable()
    {
        Health._knockback -= ApplyKnockback;
    }

    protected virtual void Start()
    {
        _target = GameObject.Find("Player");
//        _board = GameObject.Find("Board").GetComponent<Board>();
       

    }
    protected virtual void Update()
    {
        if(transform.position.y != 0)
            transform.position = new Vector3(transform.position.x,0,transform.position.z);
    }

    protected virtual void ApplyKnockback(Direction direction, float knockback)
    {

        _audioManager.PlaySFX(2);
        //Move( direction,_knockbackSpeed);
        //FinishedMoving();
        if(_moveRoutine!=null)
            StopCoroutine(_moveRoutine);
        if(_knockbackSpeed >0)
            rb.AddForce(DirectionToVec(direction)*knockback*_knockbackSpeed,ForceMode.Impulse);
        Invoke("FinishedMoving",1f);

    }
    

    protected void Move(Direction direction, float speed)
    {
        Vector3 movement = Vector3.zero;
        switch(direction)
        {
            case Direction.down:
                movement = Vector3.back;
                _column -=1;
            break;
            case Direction.up:
                movement = Vector3.forward;
                _column +=1;
            break;
            case Direction.right:
                movement = Vector3.right;
                _row += 1;
            break;
            case Direction.left:
                movement = Vector3.left;
                _row -= 1;
            break;
        }
        if(_moveRoutine != null)
            StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveTo(transform.position + movement*0.5f, speed));

    }

    private  IEnumerator MoveTo(Vector3 goalPos,float speed)
    {
        float percent = 0;
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/speed;
            rb.MovePosition(Vector3.Lerp(startPos,goalPos,percent));
            yield return null;
        }
        FinishedMoving();
    }
    protected virtual void FinishedMoving()
    {
        transform.rotation = _goalRot;
    }
    protected bool IsAdjacentSquareWalkable(Direction direction)
    {
       /* switch(direction)
        {
            case Direction.down:
                return _board.IsSquareDown(_row, _column-1);
            case Direction.up:
                return _board.IsSquareDown(_row, _column+1);
            case Direction.right:
                return _board.IsSquareDown(_row+1, _column);
            case Direction.left:
                return _board.IsSquareDown(_row-1, _column);
        }
        return false;*/


        return CheckWall(DirectionToVec(direction));

    }

    protected Vector3 DirectionToVec(Direction direction)
    {
        switch(direction)
        {
            case Direction.down:
                return Vector3.back;
         
            case Direction.up:
                return  Vector3.forward;
         
            case Direction.right:
                return Vector3.right;
      
            case Direction.left:
                return Vector3.left;
         
        }
        return Vector3.zero;
    }

    protected bool CheckWall(Vector3 dir)
    {
        RaycastHit hit;
        if( Physics.Raycast(transform.position + Vector3.up*0.1f,dir , out hit, 1,~LayerMask.NameToLayer("Tile")))
        {
            Debug.DrawRay(transform.position + Vector3.up*0.1f, dir, Color.green,0.4f);
         
            return false;
        }
        Debug.DrawRay(transform.position + Vector3.up*0.1f, dir, Color.red,0.4f);
        return true;
       
    }

    protected bool CheckPlayer(Vector3 dir)
    {
        RaycastHit hit;
        if( Physics.Raycast(transform.position + Vector3.up*0.2f,dir , out hit, 6,LayerMask.NameToLayer("Player")))
        {
            Debug.DrawRay(transform.position + Vector3.up*0.2f, dir*6, Color.yellow,0.4f);
         
            return true;
        }
        Debug.DrawRay(transform.position + Vector3.up*0.2f, dir*6, Color.yellow,0.4f);
        return false;
       
    }
    public void SetGridPos((int,int)pos)
    {
        _row = pos.Item1;
        _column = pos.Item2;
    }
    protected Direction TurnRight(Direction direction)
    {
        switch(direction)
        {
            case Direction.down:
                return Direction.left;
            case Direction.up:
                 return Direction.right;
            case Direction.right:
                 return Direction.down;
            case Direction.left:
                 return Direction.up;
        }  
        return Direction.down;
    }
    protected Direction TurnLeft(Direction direction)
    {
        switch(direction)
        {
            case Direction.down:
                return Direction.right;
            case Direction.up:
                 return Direction.left;
            case Direction.right:
                 return Direction.up;
            case Direction.left:
                 return Direction.down;
        }  
        return Direction.down;
    }

    protected void FindGridPos()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + Vector3.up*0.2f, Vector3.down , out hit, 1))
        {
            Debug.DrawRay(transform.position + Vector3.up*0.2f, Vector3.down, Color.yellow);
            Box3D box = hit.collider.gameObject.GetComponent<Box3D>();
            Debug.Log("before"+_row + " " + _column);
            SetGridPos((box.RowIndex,box.ColumnIndex));
            Debug.Log("after" + _row + " " + _column);
           
           // if(_moveRoutine!=null)
             //   StopCoroutine(_moveRoutine);
           //_moveRoutine = StartCoroutine(MoveTo(new Vector3(box.transform.position.x,0,box.transform.position.z)));
        }

    }
    protected void TryGoForward()
    {
        if(_moveRoutine!=null)
            StopCoroutine(_moveRoutine);
        if(IsAdjacentSquareWalkable(_movementDir))
        {
            if(Random.Range(0f,1f) < _changeDirectionChance)
            {
                TurnInRandomDir();
            }
            else
            {
                Move(_movementDir,_speed);
            }
        }
        else
        {
            _moveRoutine = StartCoroutine(TurnRight());
        }
    }
    protected void TurnInRandomDir()
    {
        if(Random.Range(0f,1f)<0.5f)
           _moveRoutine = StartCoroutine(TurnRight());
        else
            _moveRoutine = StartCoroutine(TurnLeft());
    }
    protected IEnumerator TurnRight()
    {
        float percent = 0;
        float startTime = Time.time;
        Quaternion startRot = transform.rotation;
        _goalRot = Quaternion.Euler(0,transform.eulerAngles.y + 90,0);
        _movementDir = TurnRight(_movementDir);

        while(percent < 1)
        {
          
            percent = (Time.time - startTime)/_speed;
            transform.rotation = Quaternion.Lerp(startRot,_goalRot,percent);
            yield return null;
        }
        FinishedMoving();
    }
    protected IEnumerator TurnLeft()
    {
        float percent = 0;
        float startTime = Time.time;
        Quaternion startRot = transform.rotation;
        _goalRot = Quaternion.Euler(0,transform.eulerAngles.y - 90,0);
        _movementDir = TurnLeft(_movementDir);

        while(percent < 1)
        {
            
            percent = (Time.time - startTime)/_speed;
            transform.rotation = Quaternion.Lerp(startRot,_goalRot,percent);
            yield return null;
        }
        FinishedMoving();
    }

}
