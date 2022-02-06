using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessorInterface : MonoBehaviour
{
    public float vignetteSway = 1f;
    public float vignetteSwaySpeed = 1f;

    public float vignetteMax = 0.5f;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private float vignetteIntensity;
    private  float duration;
    private  float startTime;
    public float setHpPercent = 1;
    public static PostProcessorInterface  instance;

    void Start()
    {
        this.GetComponent<PostProcessVolume>().profile.TryGetSettings(out chromaticAberration);
        this.GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);
        instance = this;
    }

    public static void SetHpVignette(float HpPercent)
    {
      ///  Debug.Log("SettingVignete");
        instance.vignetteIntensity =  (1-HpPercent) * instance.vignetteMax;
    }

    public static void DamageEffect(float d)
    {
        instance.startTime = Time.time;
        instance.duration = d;
        instance.StartCoroutine(instance.ChromaticAberrationEffect());
    }
    private  IEnumerator ChromaticAberrationEffect()
    {
        while (startTime + duration > Time.time)
        {
            chromaticAberration.intensity.value = Mathf.SmoothStep(0, 1, (startTime + duration - Time.time)/duration);
            yield return null;
        }

    }

    private  void UpdateVignette()
    {
        instance.vignette.intensity.value =  vignetteIntensity + (1-Mathf.Abs(Mathf.Sin(Time.time*vignetteSwaySpeed)))*vignetteSway*vignetteIntensity ;

    }


    void Update()
    {
       // UpdateVignette();

    }
}
