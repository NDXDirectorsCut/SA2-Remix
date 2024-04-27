using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad1))
        {
            QualitySettings.SetQualityLevel(0,true);
        }
        if(Input.GetKey(KeyCode.Keypad2))
        {
            QualitySettings.SetQualityLevel(1,true);
        }
    }
}
