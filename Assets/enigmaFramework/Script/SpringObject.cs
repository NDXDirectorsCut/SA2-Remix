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
    InputPlayer inputPlayer;

    [Header("Debug")]
    public int arcPoints = 5;
    public float gizmoRadius = .125f; 
    public float previewWeight = 20;
    public float previewDelta;

    // Start is called before the first frame update
    void Start()
    {
        canTrigger = true;
        animator = GetComponent<Animator>();
    }

    void OnDrawGizmos()
    {
        Vector3 velocity = transform.up * springForce;
        Vector3 position = transform.position + transform.up * .5f;
        for(int i = 0; i<arcPoints; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(position,gizmoRadius);
            position += velocity * previewDelta;
            velocity -= Vector3.up * previewWeight * previewDelta;
        }
    }


    IEnumerator ReTrigger()
    {
	    canTrigger = false;
	    yield return new WaitForSeconds(.1f);
	    canTrigger = true;
    }

    IEnumerator Spring(Collider col, float force, float time, float holdF, bool lockPos, bool add)
    {
        yield return new WaitForFixedUpdate();

        animator.CrossFadeInFixedTime("Jump",.1f,0,0);
        StartCoroutine(inputPlayer.InputLock(lockInputTime));
	    if(col.GetComponent<JumpAction>() != null && col.GetComponent<EnigmaPhysics>() != null)
        {
            JumpAction jumpScript = col.GetComponent<JumpAction>();
            EnigmaPhysics enigmaPhysics = col.GetComponent<EnigmaPhysics>();
            enigmaPhysics.canTriggerAction = false;
            
            float corrector = 1;//enigmaPhysics.characterState == 1 ? 1/enigmaPhysics.airSpeedPreservation : 1 ;
            enigmaPhysics.characterState = 2;

            if(lockPos == true)
	        {
                enigmaPhysics.rBody.position = transform.position + transform.up * .5f;
		        //Debug.Log("lock Position");
	        }
            if(additive == false)
                enigmaPhysics.rBody.velocity = Vector3.zero;
            jumpScript.StopAllCoroutines();
            //enigmaPhysics.normal = transform.up;
            GetComponent<AudioSource>().Play();
	        StartCoroutine(jumpScript.Jump(force * corrector,time,holdF,transform.up,false));
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
        {
            inputPlayer = col.GetComponent<InputPlayer>();
            StartCoroutine(Spring(col,springForce,holdTime,holdForce,lockPosition,additive));
        }
    }
}
