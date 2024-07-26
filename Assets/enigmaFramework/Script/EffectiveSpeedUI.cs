using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectiveSpeedUI : MonoBehaviour
{
    public Rigidbody rBody;
    public Vector3 prev;
    public TMP_Text speedUIElement;
    // Start is called before the first frame update
    void Start()
    {
        prev = rBody.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed = (rBody.position - prev).magnitude * 1/Time.fixedDeltaTime;
        prev = rBody.position;
        speedUIElement.text = speed.ToString();
    }
}
