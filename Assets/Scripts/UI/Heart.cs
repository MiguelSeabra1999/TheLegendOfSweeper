using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Heart : MonoBehaviour
{
    [SerializeField] private RawImage Image;
    [SerializeField] private RectTransform RectTransform;
    private Coroutine _routine;
    private Vector2 defaultSize;
    void Start()
    {
        defaultSize = RectTransform.sizeDelta;
    }
    public void Restore()
    {
        if(_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(HeartRestore());
    }
    public void Break()
    {
        if(_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(BreakHeart());
    }

    private IEnumerator BreakHeart()
    {
        float percent = 0;
        float startTime = Time.time;
        float duration = 0.1f;
        float startScale = RectTransform.sizeDelta.x;
        float goalScale = RectTransform.sizeDelta.x * 1.2f;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float scale = Mathf.Lerp(startScale,goalScale,percent);
            RectTransform.sizeDelta = new Vector2(scale,scale);
            yield  return null;
        }
                
        percent = 0;
        startTime = Time.time;
        duration = 0.05f;

        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float scale = Mathf.Lerp(goalScale,startScale,percent);
            RectTransform.sizeDelta = new Vector2(scale,scale);
            yield  return null;
        }
        Image.enabled = false;
    }
    private IEnumerator HeartRestore()
    {
        Image.enabled = true;
        float percent = 0;
        float startTime = Time.time;
        float duration = 0.1f;
        RectTransform.sizeDelta = new Vector2(0,0);
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float scale = Mathf.Lerp(0f,defaultSize.x*1.2f,percent);
            RectTransform.sizeDelta = new Vector2(scale,scale);
            yield  return null;
        }
        percent = 0;
        startTime = Time.time;
        duration = 0.05f;

        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float scale = Mathf.Lerp(defaultSize.x*1.2f,defaultSize.x*1f,percent);
            RectTransform.sizeDelta = new Vector2(scale,scale);
            yield  return null;
        }      
    }
}
