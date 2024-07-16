using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        slider.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("MasterVolume",slider.value);
    }
}
