using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Material))]
public class Fade : MonoBehaviour
{
    [SerializeField] private bool _automatic;
    [SerializeField] private Material _material;
    void Awake()
    {
     /*   if(_automatic)
            _material = GetComponent<Renderer>().material;*/
    }
    
    void OnEnable()
    {
        if(!_automatic)
            return;
        StartFade(0,transform.position.y -0.2f, 0.3f);
    }
    void OnDisable()
    {
        if(!_automatic)
            return;
        StartFade(1,transform.position.y -0.2f, 0.3f);
    }
    public void StartFade(float goalOpacity,float goalY, float duration)
    {
        StartCoroutine(FadeEffect(goalOpacity,goalY,duration));
    }
    public void FadeIn(float duration, float yOffset)
    {
        _material.color = new Color(_material.color.r,_material.color.g,_material.color.b,0);
        float originalY = transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y - yOffset ,transform.position.z);
       
        StartFade(1,originalY,duration);
    }
    private IEnumerator FadeEffect(float goalOpacity, float goalY, float duration)
    {
        float startOpacity = _material.color.a;
        float startY = transform.position.y;
        float startTime = Time.time;
        float percent = 0;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float alpha = Mathf.Lerp(startOpacity,goalOpacity,percent);
           
            _material.color = new Color(_material.color.r,_material.color.g,_material.color.b, alpha);
            float y = Mathf.Lerp(startY,goalY,percent);
            transform.position = new Vector3(transform.position.x, y ,transform.position.z);
            yield return null;
        }
    }
}
