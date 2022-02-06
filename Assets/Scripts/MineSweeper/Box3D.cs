
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Box3D : Box
{

    [SerializeField] private GameObject Wall3DPrefab;
    public ObjectPrefabs ObjectPrefabs;
    private enum BoxType{Basic,Wall,Door,Wolf,Jumper,Turtle,Shield,Pick,Key,Bomb};
    [SerializeField] private Sprite[] NumberSprites = new Sprite[8];
    [SerializeField] private SpriteRenderer Danger;
    [SerializeField] private SpriteRenderer Flag;
    [SerializeField] private SpriteRenderer Number;
    [SerializeField] private BoxMesh BoxMesh;
    [SerializeField] private GameObject HiddenObject;
    [SerializeField] private MeshRenderer MeshRenderer;
    
    [SerializeField] private float _fadeInDuration = 0.2f;
    [SerializeField] private float _fadeYoffset = 0.2f;
    [SerializeField] private float _rotationDuration = 0.2f;

    private Fade _fadeEffect;
    private Rotator _rotateEffect;
    private BoxType _boxType;
    private GameObject _wallReplacement;
    private Health _playerHealth;
    protected AudioManager _audioManager;
    public override bool IsActive()
    {
        return Interactable;
    }
    protected override void Awake()
    {
        base.Awake();
        _fadeEffect = GetComponent<Fade>();
        _rotateEffect = GetComponent<Rotator>();
        _fadeEffect.FadeIn(_fadeInDuration,_fadeYoffset);
        GameObject obj =  GameObject.Find("AudioManager");
        if(obj == null)
        {
            Destroy(this);
            return;
        }
        _playerHealth = GameObject.Find("Player").GetComponent<Health>();
        _audioManager = obj.GetComponent<AudioManager>();
    }
    void OnEnable()
    {
        BoxMesh._unclickCallback += Unclick;
    }
    void OnDisable()
    {
        BoxMesh._unclickCallback -= Unclick;
        
    }
    public override void Setup(int id, int row, int column, Color type)
    {
        base.Setup(id,row,column,type);
         Color a =   new Color(1f, 0.2f ,0f);
        _boxType = ConvertColorToBoxType(type);
        AllowBomb = false;

        switch(_boxType)
        {
            case BoxType.Basic:
                if(ObjectPrefabs.RollDropChance())
                   // Instantiate(ObjectPrefabs.GetRandomDrop(),transform.position,transform.rotation);
                    HiddenObject = ObjectPrefabs.GetRandomDrop();
            break;
            case  BoxType.Wall:
                _wallReplacement = Instantiate(Wall3DPrefab,transform.position,transform.rotation);
                _wallReplacement.transform.SetParent(transform.parent);
                _wallReplacement.name = gameObject.name;
                MeshRenderer.enabled = false;
              //  Destroy(gameObject);
                Interactable = false;
                ForbidBomb = true;
            break;
            case BoxType.Bomb:
                MustHaveBomb = true;
            break;
            case BoxType.Door:
                ForbidBomb = true;
                Reveal();
                
            break;
            case BoxType.Turtle:
                HiddenObject = ObjectPrefabs.TurtlePrefab;
            break;
            default:
            break;
        }

    }

    private BoxType ConvertColorToBoxType(Color color)
    {
        if((color.r == 1f || color.r == 0.2f) && color.g == 0.2f && color.b == 0f)
            return BoxType.Wall;
        if(color.r == 1f && color.g == 0f && color.b == 0f)
            return BoxType.Bomb;
        if(color.r == 0f && color.g == 0f && color.b == 0f && color.a != 0)
            return BoxType.Door;
        if(color.r == 0f && color.g == 1f && color.b == 0f)
            return BoxType.Turtle;
      ///  Debug.Log(color);
        return BoxType.Basic;
    }

    public override void StandDown()
    {
        if(BoxType.Door == _boxType)
            return;
        base.StandDown();
        Interactable = false;

        _rotateEffect.StartRotation(Quaternion.identity,_rotationDuration);
        SetNumberImage(false);
    }

    public override void Reveal()
    {
        if(_audioManager == null)
        {
            Destroy(this);
            return;
        }
        _audioManager.PlaySFX(4);
        _audioManager.PlaySFX(1);
        if(_boxType == BoxType.Wall)
            return;

        if(HiddenObject != null)
        {
            HiddenObject = Instantiate(HiddenObject, new Vector3(transform.position.x,0,transform.position.z), Quaternion.Euler(0,0,0));
            HiddenObject.transform.SetParent(transform.root);
            HiddenObject.transform.position = new Vector3(transform.position.x,0,transform.position.z);
//s            HiddenObject.SendMessage("SetGridPos",(RowIndex,ColumnIndex));
          

            
        }

        base.Reveal();
        SetFlag(false);

        Interactable = false;
  
        _rotateEffect.StartRotation(Quaternion.Euler(180,0,0),_rotationDuration);
     
        if(IsDangerous && Danger != null)
        {
            SetDangerImage(true);
            Invoke("SetOffExplosionParticles",0.2f);
            
        
        }
        else if (Number != null && DangerNearby >0)
        {
            
            SetNumberImage(true);
        }
    }
    protected void SetOffExplosionParticles()
    {
        GetComponent<ActivateAllParticles>().Activate();
//        Debug.Log("damging");
        _playerHealth.Damage(1);
    }
    protected override void SetDangerImage(bool state)
    {
        if (Danger != null)
        {
            Danger.enabled = state;
        }
    }
    protected override void SetFlagImage(bool state)
    {
        if(Flag!=null)
            Flag.enabled = state;
  
    }

    private void SetNumberImage(bool state)
    {
    
        if(Number!=null)
        {
            Number.enabled = state;
            if(state && DangerNearby > 0)
            {
                Number.sprite = NumberSprites[DangerNearby-1];
                Number.color = DangerColors[DangerNearby-1];
            }
        }
    }
    protected override void ResetState()
    {
     /*   if(_wallReplacement != null)
        {
            MeshRenderer.enabled = true;
            Destroy(_wallReplacement);
        }*/
        base.ResetState();
        SetNumberImage(true);
        if(DangerNearby == 0)
            SetNumberImage(false);
        if(_boxType != BoxType.Door)
            Interactable = true;
    }
    public override void OnLeftClick()
    {
        if(IsActive()|| DangerNearby != 0)
        {
            BoxMesh.Click();
            base.OnLeftClick();
        }
        //base.OnLeftClick will be called in Unclick(), which is a callback from BoxMesh. This is so the consequences of pressing a button only manifest after it is let go of.
    }
    private void Unclick()
    {

        //base.OnLeftClick();

    }

}
