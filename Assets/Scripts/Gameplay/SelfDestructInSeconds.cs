using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructInSeconds : MonoBehaviour
{
    public float _timeToLive;
    void Start()
    {
        Invoke("SelfDestruct",_timeToLive);
    }
    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
