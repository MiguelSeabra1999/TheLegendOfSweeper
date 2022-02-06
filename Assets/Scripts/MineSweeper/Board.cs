using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BoardMode
{
    _3D,
    _2D
}
public class Board : MonoBehaviour
{
    public Texture2D map;
    public LayerMask hitLayers;
    public enum Event { ClickedBlank, ClickedNearDanger, ClickedDanger, Win };
    public BoardMode Mode;
    public bool _checkClicks = true;
    [SerializeField] protected  Camera Camera;
    [SerializeField] private Box2D BoxPrefab;
    [SerializeField] private Box3D Box3DPrefab;
    
    [SerializeField] private int Width = 10;
    [SerializeField] private int Height = 10;
    [SerializeField] private float _bounceDuration = 0.2f;
    [SerializeField] private float _bounceOffset = 1;
    [SerializeField] private int NumberOfDangerousBoxes = 10;
    [SerializeField] private float ClearPropagationDelay = 0.2f;
    [SerializeField] private int _trackIndex = 0;
   public bool ForceRandom = false;
    private Bounce BounceEffect;
    private bool _isFirstClick = true;
    private Box[] _grid;
    private Vector2Int[] _neighbours;
    private RectTransform _rect;
    private Action<Event> _clickEvent;
    private bool _playing =false;
    private bool _firstEnable = true;
     protected AudioManager _audioManager;
    
    private void Awake()
    {
        BounceEffect = GetComponent<Bounce>();
        Build();

    }
  
    void Update()
    {
        if(_checkClicks && Mode == BoardMode._3D && _playing)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
                CheckClick(true,Input.mousePosition);
            else if(Input.GetKeyDown(KeyCode.Mouse1))
                CheckClick(false,Input.mousePosition);

        }
    }
    void OnEnable()
    {
        GameObject obj = GameObject.Find("AudioManager");
        if(obj == null)
        {
            return;
        }
         _audioManager =  obj.GetComponent<AudioManager>();
        if(!_firstEnable)
        {
          
           _audioManager.PlayMusic(_trackIndex);
        }

        _firstEnable = false;
    }
    [ContextMenu("Build")]
    private void Build()
    {
        if(map!=null)
        {
            Height = map.width;
            Width = map.height;
            NumberOfDangerousBoxes = Mathf.RoundToInt(((float)Height*(float)Width)/5f);
//            Debug.Log(Height +" " + Width);
        }
        _grid = new Box[Width * Height];
        


        _neighbours = new Vector2Int[8]
        {
            new Vector2Int(-Height - 1, -1),
            new Vector2Int(-Height, -1),
            new Vector2Int(-Height + 1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(Height - 1, 1),
            new Vector2Int(Height, 1 ),
            new Vector2Int(Height + 1, 1)
        };
        if(Mode == BoardMode._2D)
        {
            CreateBoard2D();
        }
        else if(Mode == BoardMode._3D)
        {
            CreateBoard3D(transform.position + new Vector3(-Width/2,0,-Height/2));
        }
    }
    [ContextMenu("DestroyGrid")]
    private void DestroyGrid()
    {
        for (int row = 0; row < Width; ++row)
        {


            for (int column = 0; column < Height; ++column)
            {
                int index = row * Width + column;
                DestroyImmediate(_grid[index].gameObject);
                
            }
        }
    }
    public void CheckClickWorldPos(bool leftClick, Vector3 worldPos)
    {
        CheckClick(leftClick, Camera.WorldToScreenPoint(worldPos));
    }
    public void CheckClick(bool leftClick, Vector3 screenPos)
    {
      
        GameObject hit = ScreenCast(screenPos);
        if(hit!=null)
        {
            Box3D box3D = hit.GetComponent<Box3D>();
            if(box3D == null)
                return;

            if(leftClick)
            {
              /*  if(BoardMode._3D == Mode)
                    _audioManager.PlaySFX(1);*/
                box3D.OnLeftClick();
            }
            else
                box3D.OnRightClick();
        }
    }
    private GameObject ScreenCast(Vector3 screenPos)
    {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out hit, 10000, hitLayers)) {
            Transform objectHit = hit.transform;
            
            return objectHit.gameObject;
        }
        return null;
    }
    public void Setup(Action<Event> onClickEvent)
    {
        _clickEvent = onClickEvent;
        Clear();
    }

    public void Clear()
    {
        for (int row = 0; row < Width; ++row)
        {
            for (int column = 0; column < Height; ++column)
            {
                int index = row * Width + column;
                _grid[index].StandDown();
            }
        }
    }

    public void RechargeBoxes()
    { 
        _playing = true;
        int numberOfItems = Width * Height;
        List<bool> dangerList = new List<bool>(numberOfItems);

        for (int count = 0; count < numberOfItems; ++count)
        {
            dangerList.Add(count < NumberOfDangerousBoxes);
        }

        dangerList.RandomShuffle();
        if(!ForceRandom )
            for (int i = 0; i < numberOfItems; ++i)
            {
                if(!_grid[i].AllowBomb)
                    dangerList[i] = false;
                if(_grid[i].MustHaveBomb)
                    dangerList[i] = true;
            }
        for (int i = 0; i < numberOfItems; ++i)
        {
            if(_grid[i].ForbidBomb)
                dangerList[i] = false;
        }

        for (int i = 0; i < numberOfItems; ++i)
        {
            _grid[i].Charge(CountDangerNearby(dangerList, i), dangerList[i], OnClickedBox);
            
        }
    }


    private void CreateBoard2D()
    {
        _rect = transform as RectTransform;
        RectTransform boxRect = BoxPrefab.transform as RectTransform;

        _rect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y * Height);
        Vector2 startPosition = _rect.anchoredPosition - (_rect.sizeDelta * 0.5f) + (boxRect.sizeDelta * 0.5f);
        startPosition.y *= -1.0f;

        for (int row = 0; row < Width; ++row)
        {
            GameObject rowObj = new GameObject(string.Format("Row{0}", row), typeof(RectTransform));
            RectTransform rowRect = rowObj.transform as RectTransform;
            rowRect.SetParent(transform);
            rowRect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, startPosition.y - (boxRect.sizeDelta.y * row));
            rowRect.sizeDelta = new Vector2(boxRect.sizeDelta.x * Width, boxRect.sizeDelta.y);
            rowRect.localScale = Vector2.one;

            for (int column = 0; column < Height; ++column)
            {
                int index = row * Width + column;
                _grid[index] = Instantiate(BoxPrefab, rowObj.transform);
                _grid[index].Setup(index, row, column);
                RectTransform gridBoxTransform = _grid[index].transform as RectTransform;
                _grid[index].name = string.Format("ID{0}, Row{1}, Column{2}", index, row, column);
                gridBoxTransform.anchoredPosition = new Vector2( startPosition.x + (boxRect.sizeDelta.x * column), 0.0f);
            }
        }
    }
    private void CreateBoard3D(Vector3 startPosition)
    {
       // Debug.Log("W; " + Width + " H: " + Height);
        for (int row = 0; row < Width; ++row)
        {
            GameObject rowObj = new GameObject(string.Format("Row{0}", row));
            rowObj.transform.SetParent(transform);
            rowObj.transform.position = new Vector3(startPosition.x,startPosition.y ,startPosition.z + 0.5f + (Width - row-1));

            for (int column = 0; column < Height; ++column)
            {
                int index = row * Height + column;
                //Debug.Log("r: " + row + " c: " + column + " = " + index);
                _grid[index] = Instantiate(Box3DPrefab, rowObj.transform);
                _grid[index].name = string.Format("ID{0}, Row{1}, Column{2}", index, row, column);
                _grid[index].transform.position = new Vector3( startPosition.x + 0.5f + column, 0,  rowObj.transform.position.z);

                if(map != null && Application.isPlaying)
                    _grid[index].Setup(index, row, column, map.GetPixel(column,Width - row - 1));
                else
                    _grid[index].Setup(index, row, column);
               
            }
        }
    }

    public bool IsSquareDown(int row, int column)
    {
        int index = row * Width + column;
        if(index >= _grid.Length || index<0)
            return false;
        return !_grid[index].IsActive();
    }


    private int CountDangerNearby(List<bool> danger, int index)
    {
        int result = 0;
        int boxRow = index / Height;

        if (!danger[index])
        {
            for (int count = 0; count < _neighbours.Length; ++count)
            {
                int neighbourIndex = index + _neighbours[count].x;
                int expectedRow = boxRow + _neighbours[count].y;
                int neighbourRow = neighbourIndex / Height;
                result += (expectedRow == neighbourRow && neighbourIndex >= 0 && neighbourIndex < danger.Count && danger[neighbourIndex]) ? 1 : 0;
            }
        }

        return result;
    }

    private void OnClickedBox(Box box)
    {
        Event clickEvent = Event.ClickedBlank;

        if(box.IsDangerous)
        {
            clickEvent = Event.ClickedDanger;
            if(_isFirstClick)
            {

                RechargeBoxes();
                box.Reveal();
                OnClickedBox(box);
                return;
            }
        }
        else if(box.DangerNearby > 0)
        {
            if(box.IsActive())
            {
                clickEvent = Event.ClickedNearDanger;
            }
            else if(CountNeighbourFlags(box) >= box.DangerNearby)
            {
                if(ClearNonFlagNeighbours(box))
                    clickEvent = Event.ClickedDanger;
            }
        }
        else
        {
           
            ClearNearbyBlanks(box);
        }

        if(CheckForWin())
        {
            clickEvent = Event.Win;
        }
        if(clickEvent == Event.ClickedDanger)
            _playing = false;

        _clickEvent?.Invoke(clickEvent);
        _isFirstClick = false;
    }

    private bool CheckForWin()
    {
        
        bool Result = true;

        for( int count = 0; Result && count < _grid.Length; ++count)
        {   
            
            if(!_grid[count].IsDangerous && _grid[count].IsActive())
            {
                Result = false;
            }
        }
        if(Result)
            _playing = false;
        return Result;
    }

    private void ClearNearbyBlanks(Box box)
    {
        if (BounceEffect != null)
        {
            BounceEffect.StartBounce(_bounceOffset,_bounceDuration);
        }
        RecursiveClearBlanks(box);
    }

    private void RecursiveClearBlanks(Box box)
    {
      
        if (!box.IsDangerous)
        {
            box.Reveal();

            if (box.DangerNearby == 0)
            {           
                for (int count = 0; count < _neighbours.Length; ++count)
                {
                    int neighbourIndex = box.ID + _neighbours[count].x;
                  //  Debug.Log("neighbourIndex: "  + neighbourIndex);
                    int expectedRow = box.RowIndex + _neighbours[count].y;
                    int neighbourRow = neighbourIndex / Height;
                    bool correctRow = expectedRow == neighbourRow;
                    bool active = neighbourIndex >= 0 && neighbourIndex < _grid.Length && _grid[neighbourIndex].IsActive();

                    //Debug.Log("neighbourIndex: "  + neighbourIndex + " nRow" + neighbourRow + " eRow " + expectedRow + " neighbourIndex " + neighbourIndex + " myIndex " + box.RowIndex);
                    if (correctRow && active)
                    {
                        //Debug.Log("a");
                        _grid[neighbourIndex].Interactable = false;
                        StartCoroutine(DelayClearRecursiveBlanks(neighbourIndex,ClearPropagationDelay));
                    }
                }
            }
        }
        if(CheckForWin())
        {
           
            _clickEvent?.Invoke(Event.Win);
        }


        
       // Invoke("Unlock",clearDelay);
    }

    private IEnumerator DelayClearRecursiveBlanks(int neighbourIndex, float delay)
    {
        if(delay > 0)
            yield return new WaitForSeconds(delay);
        RecursiveClearBlanks(_grid[neighbourIndex]);
    }

    private bool ClearNonFlagNeighbours(Box box)
    {
        bool clickedDanger = false;
        for (int count = 0; count < _neighbours.Length; ++count)
        {
            int neighbourIndex = box.ID + _neighbours[count].x;
            int expectedRow = box.RowIndex + _neighbours[count].y;
            int neighbourRow = neighbourIndex / Width;
            bool correctRow = expectedRow == neighbourRow;
            if (correctRow && neighbourIndex < _grid.Length)
            {
                Box neighbourBox = _grid[neighbourIndex]; 
                bool active = neighbourIndex >= 0 && neighbourIndex < _grid.Length && neighbourBox.IsActive();
                bool notFlagged = !neighbourBox.IsFlagged;
                if(active && notFlagged)
                {
                    if(neighbourBox.IsDangerous)
                    {
                        clickedDanger = true;
                    }
                    neighbourBox.Reveal();
                    if(neighbourBox.DangerNearby == 0)
                        RecursiveClearBlanks(neighbourBox);
                    
                }
            }
        }  
        return clickedDanger;        
    }

    private int CountNeighbourFlags(Box box)
    {
        int flagCount = 0;
        for (int count = 0; count < _neighbours.Length; ++count)
        {
            int neighbourIndex = box.ID + _neighbours[count].x;
            int expectedRow = box.RowIndex + _neighbours[count].y;
            int neighbourRow = neighbourIndex / Width;
            bool correctRow = expectedRow == neighbourRow;
            if (correctRow && neighbourIndex < _grid.Length && neighbourIndex < _grid.Length)
            {
                Box neighbourBox = _grid[neighbourIndex]; 
                bool active = neighbourIndex >= 0 && neighbourIndex < _grid.Length && neighbourBox.IsActive();
                bool flagged = neighbourBox.IsFlagged;
                if(active && flagged)
                {
                    flagCount++;
                }
            }
        }  
        return flagCount;
    }
}
