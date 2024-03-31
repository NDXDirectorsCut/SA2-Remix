using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RbodySpeedUI : MonoBehaviour
{
    public Rigidbody rBody;
    public TMP_Text speedUIElement;
    public bool div6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = div6? rBody.velocity.magnitude / 6 : (float)rBody.velocity.magnitude;
        
        speedUIElement.text = speed.ToString();
    }
}
