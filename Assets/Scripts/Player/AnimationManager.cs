using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    
    [SerializeField] private Animator Animator;
    [SerializeField] private Movement Movement;
    [SerializeField] private Attack Attack;
    [SerializeField] private ParticleSystem ps;
    

    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        Movement._startMoving += StartMoving;
        Movement._stopMoving  += StopMoving;
        Movement._movementChange += MovementChange;
        Attack._attack += StartAttack;
        Attack._grab += StartGrab;
    }
    private void UnsubscribeEvents()
    {
        Movement._startMoving -= StartMoving;
        Movement._stopMoving  -= StopMoving;
        Movement._movementChange -= MovementChange;
        Attack._attack -= StartAttack;
        Attack._grab -= StartGrab;
    }
    private void StartMoving()
    {
        Animator.SetBool("Walking",true);
        Animator.SetTrigger("StartWalking");

        if(PlayerPrefs.HasKey("hasBoots"))
        {
            if(PlayerPrefs.GetInt("hasBoots") == 1)
            {
                ps.Play();
            }
        }
    }
    private void StopMoving()
    {
        Animator.SetBool("Walking",false);
        ps.Stop();
    }
    private void MovementChange(Vector2 movement)
    {
      

        Animator.SetInteger("Direction",(int)Movement._direction);
    }

    private void StartAttack()
    {
        Animator.SetTrigger("Attack");
    }
    private void StartGrab()
    {
        Animator.SetTrigger("Grab");
    }
}
