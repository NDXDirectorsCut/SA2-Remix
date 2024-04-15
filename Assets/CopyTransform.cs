using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public Transform obj;
    public Transform target;
    [Range(0,1)]
    public float posLerp;
    public Vector3 posOffset;
    [Range(0,1)]
    public float rotLerp;
    public Vector3 rotOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        obj.position = Vector3.Lerp(obj.position,target.position + posOffset,posLerp) ;
        obj.rotation = Quaternion.Lerp(obj.rotation,target.rotation * Quaternion.Euler(rotOffset),rotLerp) ;
    }
}
