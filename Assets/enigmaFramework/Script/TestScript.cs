using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    Rigidbody physBody;
    public bool useUpdate;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        physBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(useUpdate == true)
        {
            float dl = Time.deltaTime;
            float time = Time.time;
            Vector3 dir = new Vector3(Mathf.Cos(time),0,Mathf.Sin(time));
            Debug.DrawRay(Vector3.zero,dir.normalized,Color.red);
            transform.position += dir * speed * dl;
            //physBody.Move(physBody.position+dir*speed*dl,Quaternion.identity);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(useUpdate == false)
        {
            float dl = Time.fixedDeltaTime;
            float time = Time.time;
            Vector3 dir = new Vector3(Mathf.Cos(time),0,Mathf.Sin(time));
            Debug.DrawRay(Vector3.zero,dir.normalized,Color.green);
            physBody.Move(physBody.position+dir*speed*dl,Quaternion.identity);//transform.position += dir * speed * dl;
        }
    }
}
