using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public List<Heart> hearts;
    private int index;
    void Start()
    {   
        index = hearts.Count-1;
    }

    [ContextMenu("Damage")]
    public void Damage()
    {
        if(index <0)return;
        hearts[index].Break();
        index--;

    }
 [ContextMenu("Heal")]
    public void Heal()
    {
        if(index >= hearts.Count-1)return;
        index++;
        hearts[index].Restore();
    }
     [ContextMenu("HealAll")]
    public void HealAll()
    {
        foreach(Heart heart in hearts)
            heart.Restore();
        index = hearts.Count-1;
    }

    
}
