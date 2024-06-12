using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerLogic : MonoBehaviour
{
    public TMP_Text mCount;
    public TMP_Text sCount;
    public TMP_Text msCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	float time = Time.fixedTime;
	sCount.text = TimeSpan.FromSeconds(time).Seconds.ToString("00");
	mCount.text = TimeSpan.FromSeconds(time).Minutes.ToString("00");
	float ms = TimeSpan.FromSeconds(time).Milliseconds/10f;
	msCount.text = ms.ToString("00"); //TimeSpan.FromSeconds(time).Milliseconds.ToString("00");

	//msCount,TimeSpan.FromSeconds(numOfSecs).Minutes
    }
}
