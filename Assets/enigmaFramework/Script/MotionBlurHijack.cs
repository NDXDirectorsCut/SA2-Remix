using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MotionBlurHijack : MonoBehaviour
{
    public PostProcessVolume ppVolume;
    public MotionBlur mbEffect;
    public float intensity;
    public int sampleCount;
    // Start is called before the first frame update
    void Start()
    {
        ppVolume.profile.TryGetSettings(out mbEffect);
    }

    // Update is called once per frame
    void Update()
    {
        mbEffect.shutterAngle.value = intensity;
        mbEffect.sampleCount.value = sampleCount;
    }
}
