using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using UnityEngine.EventSystems;
 using UnityEngine.Events;
public abstract class Box : MonoBehaviour
{
    [SerializeField] protected Color[] DangerColors = new Color[8];


    protected Action<Box> _changeCallback;

    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }
    public int ID { get; private set; }
    public int DangerNearby { get; private set; }
    public bool IsDangerous { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool Interactable = false;
    public bool AllowBomb = true;
    public bool MustHaveBomb = false;
    public bool ForbidBomb = false;

    public virtual bool IsActive()
    {
        return false;
    }
    protected virtual void SetDangerImage(bool state){}
    protected virtual void SetFlagImage(bool state){}
    public void Setup(int id, int row, int column)
    {
        ID = id;
        RowIndex = row;
        ColumnIndex = column;
    }

    public virtual void Setup(int id, int row, int column, Color type)
    {
       Setup(id,row,column);
    }


    public void Charge(int dangerNearby, bool danger, Action<Box> onChange)
    {
        _changeCallback = onChange;
        DangerNearby = dangerNearby;
        IsDangerous = danger;
        ResetState();
    }

    public virtual void Reveal()
    {

    }

    public virtual void StandDown()
    {
        SetDangerImage(false);
        SetFlagImage(false);
    }

    public virtual void OnLeftClick()
    {
        
        Reveal();
        _changeCallback?.Invoke(this);
        
    }
    public virtual void OnRightClick()
    {
        if(IsActive())
        {
            SetFlag(!IsFlagged);
        }
   
    }



    protected virtual void Awake()
    {
        ResetState();
    }

    protected virtual void ResetState()
    {
        SetFlag(false);
        SetDangerImage(false);

  

        
    }

    protected void SetFlag(bool state)
    {
        IsFlagged = state;
        SetFlagImage(state);
    }
}
