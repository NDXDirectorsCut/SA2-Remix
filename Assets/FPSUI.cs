using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSUI : MonoBehaviour
{
    public TMP_Text fpsUIElement;
    public int currentFPS;
    public int displayFPS;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FPSUpdate());
    }

    IEnumerator FPSUpdate()
    {
        while(1>0)
        {
           displayFPS = currentFPS;
            yield return new WaitForSeconds(1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        currentFPS = (int) (1/Time.deltaTime);
        fpsUIElement.text = displayFPS.ToString();
    }
}
