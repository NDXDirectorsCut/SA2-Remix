using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccelUI : MonoBehaviour
{
    public Rigidbody rBody;
    Vector3 prev;
    public TMP_Text accelUIElement;
    // Start is called before the first frame update
    void Start()
    {
        prev = rBody.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float accel = (rBody.velocity - prev).magnitude * 1/Time.fixedDeltaTime;
        prev = rBody.velocity;
        accelUIElement.text = accel.ToString();
    }
}
