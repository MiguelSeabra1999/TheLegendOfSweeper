using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Direction _direction;
    private BoardManager BoardManager;
    void Start()
    {
        BoardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        BoardManager.ChangeRoom(_direction);
    }
}
