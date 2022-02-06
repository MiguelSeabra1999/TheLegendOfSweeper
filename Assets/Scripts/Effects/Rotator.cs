using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public void StartRotation(Quaternion goal, float duration)
    {
        StartCoroutine(Rotate(goal,duration));
    }
    private IEnumerator Rotate(Quaternion goal, float duration)
    {
        float startTime = Time.time;
        float percent = 0;
        Quaternion startRot = transform.rotation;
        while(percent < 1)
        {
            percent = (Time.time-startTime)/duration;
            transform.rotation = Quaternion.Lerp(startRot,goal,percent);
            yield return null;
        }
    }
}
