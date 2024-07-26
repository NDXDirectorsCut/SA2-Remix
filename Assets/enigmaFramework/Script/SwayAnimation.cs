using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayAnimation : MonoBehaviour
{
    public Vector3 sinPos;
    public bool additivePos;
    public float posSpeed;
    public Vector3 swayRot;
    public float swaySpeed;
    public float randomRange;
    float offset;
    Vector3 basePos;
    // Start is called before the first frame update
    void Start()
    {
        basePos = transform.position;
	offset = Random.Range(-randomRange,randomRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	float time = Time.time + offset;
	basePos = additivePos ? transform.position : basePos;
        transform.position = basePos + sinPos * Mathf.Sin(time*posSpeed);//transform.TransformPoint();
	transform.rotation = Quaternion.identity * Quaternion.Euler(swayRot * time * swaySpeed);
    }
}
