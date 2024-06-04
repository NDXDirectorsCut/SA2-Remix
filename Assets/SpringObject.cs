using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringObject : MonoBehaviour
{
    public float springForce;
    public float holdTime;
    public float holdForce;
    public bool lockPosition;
    public bool additive;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator Spring(Rigidbody rBody,float force)
    {
	rBody.velocity = additive ? rBody.velocity + transform.up * springForce : transform.up * springForce;
    }
    
    void OnTriggerEnter(Collider col)
    {
	if(col.GetComponent<JumpAction>() != null)
	{
	    JumpAction jumpScript = col.GetComponent<JumpAction>();
	    StartCoroutine(jumpScript.Jump(springForce,holdTime,additiveForce));
	}
	else
	{
	    if(col.GetComponent<Rigidbody>())
	    {
		StartCoroutine(Spring(springForce));
	    }
	}
    }
}
