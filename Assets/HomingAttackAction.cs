using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAction : MonoBehaviour
{
	EnigmaPhysics enigmaPhysics;
	JumpAction jumpScript;
	public Animator animator;
	public GameObject trailEffect;
	public GameObject ballEffect;
	public float airDashForce;
	public float homingForce;
	public float homingTurn;
	public float homingRange;

	IEnumerator HomingCheck(float range)
	{
		Debug.Log("Homing Check");
		float angle = Vector3.SignedAngle(enigmaPhysics.forwardReference,enigmaPhysics.primaryAxis,enigmaPhysics.referenceVector);
		Vector3 checkDir = Quaternion.AngleAxis(angle,enigmaPhysics.referenceVector) * enigmaPhysics.forwardReference;

		Collider[] colliderList = Physics.OverlapSphere(transform.position,range);
		Transform target = null; float maxAngle = 360;
		int i = 0;
		foreach (var hitCol in colliderList)
        	{
		   
        	    if(hitCol.tag == "Homing")
		    {
			Vector3 hitDir = Vector3.ProjectOnPlane(-(transform.position - hitCol.transform.position).normalized,enigmaPhysics.referenceVector);
			float hitAngle = Vector3.Angle(hitDir,enigmaPhysics.primaryAxis);
			if(hitAngle<maxAngle)
			{
				Debug.Log(hitCol.transform + " " + hitAngle);
				target = hitCol.transform;
				maxAngle = hitAngle;
			}
		    }
       	}
		if(target == null)
		{
			Debug.Log("No targets in range");
			StartCoroutine(AirDash(airDashForce));
			yield return null;
		}
		else
		{
			//target.position += Vector3.up*2;
			enigmaPhysics.rBody.velocity = Vector3.zero;
			StartCoroutine(HomeIn(target,homingForce,homingTurn));
			Debug.Log(target);
		}
		yield return null;
	}

	IEnumerator HomeIn(Transform target, float force, float turn)
	{
		animator.CrossFadeInFixedTime("Spin",.25f,0,0);
	  	animator.SetBool("Scripted Animation",true);

		GameObject currentEffect = Instantiate(trailEffect,animator.transform.position+animator.transform.up * .5f,Quaternion.identity,animator.transform);
		GameObject curBall = Instantiate(ballEffect,animator.transform.position+animator.transform.up * .525f,Quaternion.identity,animator.transform);
		curBall.SetActive(true);
		currentEffect.SetActive(true);
		ParticleSystem particle = curBall.GetComponent<ParticleSystem>();
		enigmaPhysics.rBody.velocity = enigmaPhysics.forwardReference.normalized * force;
		while(target != null)
		{
			Collider col = target.GetComponent<Collider>();
			float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position,col.ClosestPoint(transform.position+transform.up*.5f)),0,1);
			Vector3 hitDir = -(transform.position - target.position).normalized;
			Vector3 crossVector = Vector3.Cross(enigmaPhysics.rBody.velocity,hitDir);
			float angle = Vector3.SignedAngle(enigmaPhysics.rBody.velocity,hitDir,crossVector);
			enigmaPhysics.canTriggerAction = false;
			Debug.DrawRay(col.ClosestPoint(transform.position+transform.up*.5f),-hitDir,Color.blue);
			if(clampedDist<.8f)
			{
				transform.position = col.ClosestPoint(transform.position+transform.up*.5f);
				target = null;
				animator.SetBool("Scripted Animation",false);
				enigmaPhysics.canTriggerAction = true;
				currentEffect.GetComponent<TrailRenderer>().emitting = false;
				particle.Stop();
				Destroy(curBall,particle.startLifetime);
				enigmaPhysics.rBody.velocity = Vector3.zero;
				if(jumpScript != null)
				{
					StartCoroutine(jumpScript.Jump(jumpScript.initialJumpForce,jumpScript.jumpTimer,0));
				}
				//StopAllCoroutines();
				
				yield return null;
			}
			
			//float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position,target.position),0,1);
			enigmaPhysics.rBody.velocity = Quaternion.AngleAxis(angle*turn*Time.fixedDeltaTime * clampedDist,crossVector) * enigmaPhysics.rBody.velocity * clampedDist;
			
			yield return new WaitForFixedUpdate();
		}
		yield return null;
	}

	IEnumerator AirDash(float force)
	{
		animator.CrossFadeInFixedTime("Spin",.25f,0,0);
	  	animator.SetBool("Scripted Animation",true);
		enigmaPhysics.rBody.velocity = new Vector3(0,enigmaPhysics.rBody.velocity.y,0)	 + enigmaPhysics.forwardReference.normalized * force;
		yield return null;
	}
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
		jumpScript = GetComponent<JumpAction>();
    }

    // Update is called once per frame
    void Update()
    {
		if(enigmaPhysics.characterState == 2 && enigmaPhysics.grounded == false)
		{
			if(Input.GetButtonDown("Jump") && enigmaPhysics.canTriggerAction == true)
			{
				StartCoroutine(HomingCheck(homingRange));
			}

			float angle = Vector3.SignedAngle(enigmaPhysics.forwardReference,enigmaPhysics.primaryAxis,enigmaPhysics.referenceVector);
			Vector3 checkDir = Quaternion.AngleAxis(angle,enigmaPhysics.referenceVector) * enigmaPhysics.forwardReference;
			Debug.DrawRay(transform.position,checkDir,Color.cyan);
		}
		else
		{
			StopAllCoroutines();
		}
    }
}
