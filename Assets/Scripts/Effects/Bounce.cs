using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] private AnimationCurve Curve;
    private Coroutine _bounceRoutine;

    public void StartBounce(float yOffset, AnimationCurve curve, float duration)
    {
        if(_bounceRoutine != null)
            StopCoroutine(_bounceRoutine);
        _bounceRoutine = StartCoroutine(BounceEffect(yOffset,curve,duration));
    }
    public void StartBounce(float yOffset, float duration)
    {
        StartBounce(yOffset, Curve, duration);
    }
    private IEnumerator BounceEffect(float yOffset, AnimationCurve curve, float duration)
    {
        float startY = transform.position.y;
        float startTime = Time.time;
        float percent = 0;
        
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float y = startY + -1*curve.Evaluate(percent)*yOffset;
            transform.position = new Vector3(transform.position.x,y,transform.position.z);
            yield return null;
        }
    }


}
