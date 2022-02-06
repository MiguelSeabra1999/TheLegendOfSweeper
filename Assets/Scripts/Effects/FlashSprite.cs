using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSprite : MonoBehaviour
{
    [SerializeField] private Color _baseColor;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private float _defaultDuration;
    [SerializeField] private Color _defaultFlashColor;
    private Coroutine _flashRoutine;
    
    public void StartFlashEffect(Color goalColor,float duration)
    {
        if(_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashEffect(goalColor, duration));
    }
    public void StartFlashEffect(Color goalColor)
    {
        if(_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashEffect(goalColor, _defaultDuration));
    }
    public void StartFlashEffect(float duration)
    {
        if(_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashEffect(_defaultFlashColor, duration));
    }
    public void StartFlashEffect()
    {
        if(_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashEffect(_defaultFlashColor, _defaultDuration));
    }

    private IEnumerator FlashEffect(Color goalColor, float duration)
    {
        float startTime = Time.time;
        float percent = 0;
        Color startColor = SpriteRenderer.color;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            SpriteRenderer.color = Color.Lerp(startColor,goalColor,percent);
            yield return null;
        }
        startTime = Time.time;
        percent = 0;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            SpriteRenderer.color = Color.Lerp(goalColor,_baseColor,percent);
            yield return null;
        }
    }
}
