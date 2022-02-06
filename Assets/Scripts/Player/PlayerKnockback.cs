using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    
    [SerializeField] private float _controlCooloffTime = 0.3f;
    [SerializeField] private Health Health;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private KeyReader KeyReader;
    private Coroutine _enableKeysRoutine;
    private AudioManager _audioManager;
    void Awake()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Health._knockback += ApplyKnockback;
    }
    void OnEnable()
    {
        Health._knockback += ApplyKnockback;
    }
    void OnDisable()
    {
        Health._knockback -= ApplyKnockback;
    }
    private void ApplyKnockback(Direction direction, float knockback)
    {
        _audioManager.PlaySFX(5);
        if(KeyReader != null)
        {
            KeyReader.enabled = false;
        }

        Vector3 force = Vector2.zero;
        switch(direction)
        {
            case Direction.down:
                force = Vector3.back;
            break;
            case Direction.up:
                force = Vector3.forward;
            break;
            case Direction.right:
                force = Vector3.right;
            break;
            case Direction.left:
                force = Vector3.left;
            break;
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(force*knockback,ForceMode.Impulse);
       
        if(_enableKeysRoutine !=null)
            StopCoroutine(_enableKeysRoutine);
        _enableKeysRoutine = StartCoroutine(EnableKeysWithDelay(_controlCooloffTime));
        
    }
    private IEnumerator EnableKeysWithDelay(float delay)
    {
        if(KeyReader == null)
            yield break;

        yield return new WaitForSeconds(delay);

        KeyReader.enabled = true;
    }
}
