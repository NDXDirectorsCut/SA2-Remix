using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindashAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    Rigidbody rBody;
    public Animator animator;
    public GameObject trailEffect;
    public GameObject ballEffect;
    public float triggerTime;
    public float holdTime;
    public float topSpeed;
    public float minSpeed;
    public float rollDeceleration;
    bool holding; float startHold;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        rBody = enigmaPhysics.rBody;
    }

    IEnumerator Spindash()
    {
	    Vector3 pos = transform.position;
        float velocity = rBody.velocity.magnitude;
	    float startTime = Time.time;
	    animator.CrossFadeInFixedTime("Spindash",.25f,0,0);
        enigmaPhysics.canTriggerAction = false;
	    float time = 0;
	    while(Input.GetKey(KeyCode.LeftShift))
	    {
            animator.SetBool("Scripted Animation",true);
            time = Mathf.Clamp((Time.time-startTime)/holdTime,0,1);

            //transform.position = Vector3.Lerp(transform.position,pos,1/3f);
            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,.1f);
            if(enigmaPhysics.primaryAxis.magnitude > .1f)
                enigmaPhysics.forwardReference = enigmaPhysics.primaryAxis;

            if(enigmaPhysics.grounded == false)
            {
                animator.SetBool("Scripted Animation",false);
                enigmaPhysics.canTriggerAction = true;
            }
            yield return new WaitForFixedUpdate();
	    }
          float speed = velocity > .2f ? Mathf.Lerp(velocity,topSpeed,time) : Mathf.Lerp(minSpeed,topSpeed,time) ;
	    if(Input.GetKeyUp(KeyCode.LeftShift))
	    {
	        StartCoroutine(Roll(speed,rollDeceleration));
	    }

        
    }

    IEnumerator Roll(float rollSpeed,float rollDecel)
    {
        float activeSpeed = rollSpeed;
        rBody.velocity = enigmaPhysics.forwardReference.normalized * activeSpeed;
        animator.CrossFadeInFixedTime("Ground Spin",.25f,0,0);
        animator.SetBool("Scripted Animation",true);
	GameObject currentEffect = Instantiate(trailEffect,animator.transform.position+animator.transform.up * .25f,Quaternion.identity,animator.transform);
	GameObject curBall = Instantiate(ballEffect,animator.transform.position+animator.transform.up * .45f,Quaternion.identity,animator.transform);
	curBall.SetActive(true);
	currentEffect.SetActive(true);
	ParticleSystem particle = curBall.GetComponent<ParticleSystem>();
	var main = particle.main;

        while(enigmaPhysics.characterState == 1 && rBody.velocity.magnitude > .25f)
        {
	    enigmaPhysics.canTriggerAction = false;
	    if(Input.GetKeyDown(KeyCode.LeftShift))
	    {
			StopAllCoroutines();
			animator.SetBool("Scripted Animation",false);
			enigmaPhysics.canTriggerAction = true;
			currentEffect.GetComponent<TrailRenderer>().emitting = false;
        	main.simulationSpace = ParticleSystemSimulationSpace.World;
            curBall.transform.position = transform.InverseTransformPoint(transform.position);
			particle.Stop();
			Destroy(curBall,particle.duration + particle.startLifetime);

	    }
            activeSpeed -= rollDecel * Time.fixedDeltaTime;
            //Add gravity
            Vector3 slopeVector = -Vector3.ProjectOnPlane(enigmaPhysics.referenceVector,enigmaPhysics.normal).normalized;
            Vector3 rightVector = Vector3.Cross(enigmaPhysics.normal,enigmaPhysics.forwardReference);
            float veloRatio = Mathf.Clamp(Vector3.SignedAngle(rightVector,slopeVector,enigmaPhysics.normal)/90,-1,1);
            //Debug.Log(veloRatio);
            Debug.DrawRay(transform.position,rightVector,Color.blue);
            Debug.DrawRay(transform.position,slopeVector * enigmaPhysics.slopeForce.magnitude * veloRatio,Color.red);
            activeSpeed -= enigmaPhysics.slopeForce.magnitude * veloRatio;
            //Debug.Log(veloRatio);

            rBody.velocity = enigmaPhysics.forwardReference * activeSpeed;
            yield return new WaitForFixedUpdate();
    	  }
	    animator.SetBool("Scripted Animation", false);
	    enigmaPhysics.canTriggerAction = true;
	    currentEffect.GetComponent<TrailRenderer>().emitting = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        curBall.transform.position = transform.InverseTransformPoint(transform.position);
	    particle.Stop();
	    Destroy(curBall,particle.duration + particle.startLifetime);

    }

    // Update is called once per frame
    void Update()
    {
	  if(holding == false)
		startHold = Time.time;
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            holding = true;
		startHold = Time.time;
        }
	  if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            holding = false;
        }
	  if(Time.time - startHold >= triggerTime && enigmaPhysics.characterState == 1 && holding == true && enigmaPhysics.canTriggerAction == true)
	  {
		StartCoroutine(Spindash());
		holding = false;
	  }
      if(enigmaPhysics.characterState != 1 )
        StopAllCoroutines();
    }
}