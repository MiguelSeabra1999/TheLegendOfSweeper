using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupReceiver : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private RuntimeAnimatorController _shieldAnimator;
    [SerializeField] private Movement _movement;
    public int HasShield = 0;
    public int HasBoots = 0;
    void Awake()
    {
        if (PlayerPrefs.HasKey("hasShield"))
        {
            HasShield = PlayerPrefs.GetInt("hasShield");
        }else
        {
            PlayerPrefs.SetInt("hasShield", HasShield);
        }
        if (PlayerPrefs.HasKey("hasBoots"))
        {
            HasBoots = PlayerPrefs.GetInt("hasBoots");
            if(HasBoots == 1)
                EnableBoots();
        }else
        {
            PlayerPrefs.SetInt("hasBoots", HasBoots);
            
        }

        if(HasShield != 0)
            EnableShield();
    }
    [ContextMenu("GrantShield")]
    public void GrantShield()
    {
        HasShield = 1;
        PlayerPrefs.SetInt("hasShield", 1);
        EnableShield();
    }
    [ContextMenu("GrantBoots")]
    public void GrantBoots()
    {
        HasBoots = 1;
        PlayerPrefs.SetInt("hasBoots", 1);
        EnableBoots();
    
    }
    private void EnableShield()
    {
        _animator.runtimeAnimatorController = _shieldAnimator;
    }
    private void EnableBoots()
    {
        _movement._speed = 0.12f;
    }
        [ContextMenu("RemoveShield")]
    public void RemoveShield()
    {
        PlayerPrefs.SetInt("hasShield", 0);

    }

        [ContextMenu("RemoveBoots")]
    public void RemoveBoots()
    {
        PlayerPrefs.SetInt("hasBoots", 0);

    }

}
