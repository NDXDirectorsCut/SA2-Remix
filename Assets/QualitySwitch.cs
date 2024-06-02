using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySwitch : MonoBehaviour
{
    EnigmaCamera enigmaCamera;
    // Start is called before the first frame update
    void Start()
    {
        enigmaCamera = GetComponent<EnigmaCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad0))
        {
            enigmaCamera.targetFramerate = 0;
        }
        if(Input.GetKey(KeyCode.Keypad1))
        {
            QualitySettings.SetQualityLevel(0,true);
        }
        if(Input.GetKey(KeyCode.Keypad2))
        {
            QualitySettings.SetQualityLevel(1,true);
        }
        if(Input.GetKey(KeyCode.Keypad4))
        {
            Time.fixedDeltaTime = 1/120f;
        }
        if(Input.GetKey(KeyCode.Keypad5))
        {
            Time.fixedDeltaTime = 1/60f;
        }
        if(Input.GetKey(KeyCode.Keypad6))
        {
            Time.fixedDeltaTime = 1/15f;
        }
        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            Time.fixedDeltaTime = 1/480f;
        }

    }
}
