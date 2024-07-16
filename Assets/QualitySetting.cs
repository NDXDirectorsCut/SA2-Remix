using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QualitySetting : MonoBehaviour
{
    public TMP_Dropdown setting;
    // Start is called before the first frame update
    void Start()
    {
        setting = GetComponentInChildren<TMP_Dropdown>();
        setting.value = PlayerPrefs.GetInt("QualityLevel");
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("QualityLevel",setting.value);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel"));
    }
}
