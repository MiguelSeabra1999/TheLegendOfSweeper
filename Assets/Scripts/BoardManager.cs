using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [System.Serializable]
    public class BoardRow
    {
        public List<Board> rows;
    }
     [SerializeField] private List<BoardRow> Boards = new List<BoardRow>();
    [SerializeField] private GameObject _player;
    private Movement _playerMovement;

    public Vector2 _currentPos = Vector2.zero;

    private bool _changingRooms = false;

    void Start()
    {
        _playerMovement = _player.GetComponent<Movement>();
        Invoke("HideOtherRooms",0.5f);
        //HideOtherRooms();
    }
    private void HideOtherRooms()
    {
        int column = 0, row = 0;
        foreach(BoardRow boardRow in Boards)
        {
            foreach(Board board in boardRow.rows)
            {
                if(board == null)
                {
                    row++;
                    continue;
                }
                 board.RechargeBoxes();
//                 Debug.Log(row + " " + column);
                if(row != _currentPos.x || column != _currentPos.y)
                {
                    board.gameObject.SetActive(false);
                }
                row++;
            }
            row = 0;
            column++;
        }

    }
    public void ChangeRoom(Direction direction)
    {
        switch(direction)
        {
            case Direction.down:
                ChangeRoom(new Vector2(_currentPos.x,_currentPos.y - 1));
            break;
            case Direction.left:
                ChangeRoom(new Vector2(_currentPos.x - 1,_currentPos.y));
            break;
            case Direction.right:
                ChangeRoom(new Vector2(_currentPos.x +1,_currentPos.y));
            break;
            case Direction.up:
                ChangeRoom(new Vector2(_currentPos.x,_currentPos.y + 1));
            break;
        }
    }

    public void ChangeRoom(Vector2 coordinates)
    {
//        Debug.Log("Changing room to: " + coordinates.x + "," + coordinates.y);
        if(!_changingRooms)
            StartCoroutine(ChangeRoomRoutine(coordinates));
    }

    private IEnumerator ChangeRoomRoutine(Vector2 coordinates)
    {
        _changingRooms = true;
        Vector2 startPos = _currentPos;
        _currentPos = coordinates;
        SetBoardActive(coordinates, true);
       // Debug.Log("Activate: " + coordinates.x + " " + coordinates.y);
        _playerMovement.MoveAndSleep(2f);
        yield return new WaitForSeconds(2f);
        _player.transform.SetParent(GetBoard(coordinates).transform);
        SetBoardActive(startPos, false);
      //  Debug.Log("Deactivate: " + _currentPos.x + " " + _currentPos.y);

        _changingRooms = false;
    }

    private void SetBoardActive(Vector2 coordinates, bool state)
    {
        
     //   GetBoard(coordinates).RechargeBoxes();
        GetBoard(coordinates).gameObject.SetActive(state);
    }
    private Board GetBoard(Vector2 coordinates)
    {
        return Boards[(int)coordinates.y].rows[(int)coordinates.x];
    }
}
