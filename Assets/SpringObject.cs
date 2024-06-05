using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringObject : MonoBehaviour
{
    public float springForce;
    public float holdTime;
    public float holdForce;
    public float lockInputTime;
    public bool lockPosition;
    public bool additive;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator Spring(Rigidbody rBody,float force)
    {
	    rBody.velocity = additive ? rBody.velocity + transform.up * springForce : transform.up * springForce;
        yield return null;
    }
    
    void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<JumpAction>() != null && col.GetComponent<EnigmaPhysics>() != null)
        {
            JumpAction jumpScript = col.GetComponent<JumpAction>();
            EnigmaPhysics enigmaPhysics = col.GetComponent<EnigmaPhysics>();
            enigmaPhysics.canTriggerAction = false;
            if(lockPosition == true)
                enigmaPhysics.rBody.position = transform.position + transform.up * .5f;
            StartCoroutine(jumpScript.Jump(springForce,holdTime,holdForce));
        }
        else
        {
            if(col.GetComponent<Rigidbody>() != null)
            {
                StartCoroutine(Spring(col.GetComponent<Rigidbody>(),springForce));
            }
        }
    }
}
