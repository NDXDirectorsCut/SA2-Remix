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
    bool canTrigger;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        canTrigger = true;
        animator = GetComponent<Animator>();
    }
    IEnumerator ReTrigger()
    {
	    canTrigger = false;
	    yield return new WaitForSeconds(.1f);
	    canTrigger = true;
    }

    IEnumerator Spring(Collider col, float force, float time, float holdF, bool lockPos, bool add)
    {
        animator.CrossFadeInFixedTime("Jump",.1f,0,0);
	    if(col.GetComponent<JumpAction>() != null && col.GetComponent<EnigmaPhysics>() != null)
        {
            JumpAction jumpScript = col.GetComponent<JumpAction>();
            EnigmaPhysics enigmaPhysics = col.GetComponent<EnigmaPhysics>();
            enigmaPhysics.canTriggerAction = false;
            if(lockPos == true)
	        {
                enigmaPhysics.rBody.position = transform.position + transform.up * .5f;
		        Debug.Log("lock Position");
	        }
            if(additive == false)
                enigmaPhysics.rBody.velocity = Vector3.zero;
            StartCoroutine(jumpScript.Jump(force,time,holdF,transform.up));
        }
        else
        {
            if(col.GetComponent<Rigidbody>() != null)
            {
		        Rigidbody rBody = col.GetComponent<Rigidbody>();
		        rBody.velocity = add ? rBody.velocity + transform.up * force : transform.up * force;
                //StartCoroutine(Spring(col.GetComponent<Rigidbody>(),springForce));
            }
        }
	    StartCoroutine(ReTrigger());
	    
        yield return null;
    }
    
    void OnTriggerEnter(Collider col)
    {
	    if(canTrigger == true)
            StartCoroutine(Spring(col,springForce,holdTime,holdForce,lockPosition,additive));
    }
}
