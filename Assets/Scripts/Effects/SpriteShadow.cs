using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    void Awake()
    {
        SpriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        Destroy(this);
    }
}
