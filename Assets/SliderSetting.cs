using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : MonoBehaviour
{
    Slider slider;
    public string settingName;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        slider.value = PlayerPrefs.GetFloat(settingName);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat(settingName,slider.value);
    }
}
