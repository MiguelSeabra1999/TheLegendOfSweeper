using UnityEngine;
 using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ScriptableObjects/ObjectPrefab", menuName = "ScriptableObjects/ObjectPrefab")]
public class ObjectPrefabs : ScriptableObject
{
    public GameObject TurtlePrefab;
    public List<GameObject> RandomDrops = new List<GameObject>();
    public float DropChance = 0.01f;
    public GameObject GetRandomDrop()
    {
        return RandomDrops[Random.Range(0,RandomDrops.Count)];
    }
    public bool RollDropChance()
    {
        return Random.Range(0f,1f) <= DropChance;
    }
}