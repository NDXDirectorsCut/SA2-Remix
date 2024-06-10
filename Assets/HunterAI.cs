using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAI : MonoBehaviour
{
    Transform target;
    public float range;
    RaycastHit hit;
    Vector3 normal;
    bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        normal = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
        float minDist = range+1;
        foreach(GameObject potentialTarget in potentialTargets)
        {
            if(Vector3.Distance(transform.position,potentialTarget.transform.position) < range && Vector3.Distance(transform.position,potentialTarget.transform.position) < minDist)
            {
                minDist = Vector3.Distance(transform.position,potentialTarget.transform.position);
                target = potentialTarget.transform;
            }
        }
        if(Physics.Raycast(transform.position + transform.up * 1.5f,-transform.up,out hit,2f))
        {
            normal = hit.normal;
            transform.position = hit.point;
            if(target != null)
            {
                transform.forward = Vector3.ProjectOnPlane( (transform.position-target.position), normal ); 
            }

            transform.rotation = Quaternion.LookRotation(transform.forward,normal);
            Debug.Log("Hit");
            Debug.DrawRay(hit.point,normal,Color.blue);
        }
        grounded = hit.transform == null ? false : true;
    }
}
