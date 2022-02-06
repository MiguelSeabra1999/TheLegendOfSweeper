using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverOnDestroy : MonoBehaviour
{
    [SerializeField] private UImanager _UImanager;
    void OnDestroy()
    {

       if(_UImanager != null)
            _UImanager.SetGameOver();
    }
}
