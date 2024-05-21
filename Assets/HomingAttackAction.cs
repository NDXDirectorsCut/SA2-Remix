using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAction : MonoBehaviour
{
	EnigmaPhysics enigmaPhysics;
	public Animator animator;
	public float airDashForce;
	public float homingForce;
	public float homingTurn;
	public float homingRange;
	Vector3[] directions = new Vector3[100];

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
			directions[i] = hitDir;
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
			StartCoroutine(HomeIn(target,homingForce,homingTurn));
			Debug.Log(target);
		}
		yield return null;
	}

	IEnumerator HomeIn(Transform target, float force, float turn)
	{
		enigmaPhysics.rBody.velocity = enigmaPhysics.forwardReference * force;
		while(target != null)
		{
			Collider col = target.GetComponent<Collider>();
			if(Vector3.Distance(transform.position,col.ClosestPoint(transform.position))<.2f)
			{
				target = null;
				yield return null;
			}
			Vector3 hitDir = -(transform.position - target.position).normalized;
			Vector3 crossVector = Vector3.Cross(enigmaPhysics.rBody.velocity,hitDir);
			float angle = Vector3.SignedAngle(enigmaPhysics.rBody.velocity,hitDir,crossVector);
			enigmaPhysics.rBody.velocity = Quaternion.AngleAxis(angle*turn*Time.fixedDeltaTime,crossVector) * enigmaPhysics.rBody.velocity;
			
			yield return new WaitForFixedUpdate();
		}
		yield return null;
	}

	IEnumerator AirDash(float force)
	{
		animator.CrossFadeInFixedTime("Spin",.25f,0,0);
	  	animator.SetBool("Scripted Animation",true);
		enigmaPhysics.rBody.velocity = enigmaPhysics.forwardReference.normalized * force;
		yield return null;
	}
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(enigmaPhysics.characterState == 2)
	{
		if(Input.GetButtonDown("Jump"))
		{
			StartCoroutine(HomingCheck(homingRange));
			for(int i = 0;i<directions.Length;i++)
			{
				directions[i] = Vector3.zero;
			}
		}
		for(int i = 0;i<directions.Length;i++)
		{
			Debug.DrawRay(transform.position,directions[i].normalized * 2, Color.magenta);
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
