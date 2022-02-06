using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using UnityEngine.EventSystems;

public class Box2D : Box, IPointerClickHandler 
{
    [SerializeField] protected Image Danger;
    [SerializeField] protected Image Flag;
    private Button _button;
    private TMP_Text _textDisplay;


   
    public override bool IsActive()
    {
        return _button != null && _button.interactable;
    }

  

    public override void Reveal()
    {
        base.Reveal();
        SetFlag(false);
        if (_button != null)
        {
            _button.interactable = false;
        }
        if(IsDangerous && Danger != null)
        {
            Danger.enabled = true;
        }
        else if (_textDisplay != null)
        {
            _textDisplay.enabled = true;
        }

    }

    public override void StandDown()
    {
        base.StandDown();
        if (_button != null)
        {
            _button.interactable = false;
        }

        if (_textDisplay != null)
        {
            _textDisplay.enabled = false;
        }
    }


    protected override void Awake()
    {
        _textDisplay = GetComponentInChildren<TMP_Text>(true);
        _button = GetComponent<Button>();

        base.Awake();
    }

    protected override void ResetState()
    {
        base.ResetState();
        if (_textDisplay != null)
        {
            if (DangerNearby > 0)
            {
                _textDisplay.text = DangerNearby.ToString("D");
                _textDisplay.color = DangerColors[DangerNearby-1];
            }
            else
            {
                _textDisplay.text = string.Empty;
            }

            _textDisplay.enabled = false;
        }

        if (_button != null)
        {
            _button.interactable = true;
        }
    }
    protected override void SetDangerImage(bool state)
    {
        if (Danger != null)
        {
            Danger.enabled = false;
        }
    }
    protected override void SetFlagImage(bool state)
    {
        if(Flag!=null)
            Flag.enabled = state;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
 
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        /* else if (eventData.button == PointerEventData.InputButton.Middle)
             middleClick.Invoke ();*/
         else if (eventData.button == PointerEventData.InputButton.Right)
             OnRightClick();
    }

}
