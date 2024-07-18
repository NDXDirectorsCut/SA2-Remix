using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAction : MonoBehaviour
{
	EnigmaPhysics enigmaPhysics;
	JumpAction jumpScript;
	Animator animator;
	CharacterVoice voice;
	public GameObject trailEffect;
	public GameObject ballEffect;
	public AudioClip dashSound;
	[Range(0,1)]
	public float volume = 0.5f;
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
				//Debug.Log(hitCol.transform + " " + hitAngle);
				target = hitCol.transform;
				maxAngle = hitAngle;
			}
		    }
       	}

		if(target != null)
		{
			Vector3 targetDir = -(transform.position - target.position).normalized; 
			float dist = Vector3.Distance(transform.position,target.position);
			RaycastHit testHit;
			Physics.Raycast(transform.position+transform.up*.5f,targetDir, out testHit, dist,LayerMask.GetMask("Default"));
			if(testHit.transform != null)
			{
				target = null;
			}
		}

		if(target == null)
		{
			//Debug.Log("No targets in range");
			StartCoroutine(AirDash(airDashForce));
			yield return null;
		}
		else
		{
			//target.position += Vector3.up*2;
			enigmaPhysics.rBody.velocity = Vector3.zero;
			StartCoroutine(HomeIn(target,homingForce,homingTurn));
			//Debug.Log(target);
		}

		AudioSource voiceSource = voice.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		AudioClip sound = voice.heavyGruntSounds[Random.Range(0, voice.heavyGruntSounds.Length-1 )];
		voiceSource.clip = sound;
		voiceSource.Play();
		AudioSource effectSource = voice.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		effectSource.clip = dashSound;
		effectSource.volume = volume;
		effectSource.Play();

		Destroy(voiceSource,sound.length+5);
        Destroy(effectSource,dashSound.length+5);

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
			Vector3 hitDir = -(transform.position - target.position).normalized;
			Vector3 targetPos = col.ClosestPoint(transform.position+transform.up*.5f);// + hitDir*.5f;
			float clampedDist = Mathf.Clamp(Vector3.Distance(transform.position,targetPos),0,1);

			Vector3 crossVector = Vector3.Cross(enigmaPhysics.rBody.velocity,hitDir);
			float angle = Vector3.SignedAngle(enigmaPhysics.rBody.velocity,hitDir,crossVector);
			enigmaPhysics.canTriggerAction = false;
			enigmaPhysics.rBody.velocity = enigmaPhysics.rBody.velocity.normalized * force;
			Debug.DrawRay(col.ClosestPoint(transform.position+transform.up*.5f),-hitDir,Color.blue);

			/*
			if(target == null || enigmaPhysics.characterState != 2)
			{
				animator.SetBool("Scripted Animation",false);
				enigmaPhysics.canTriggerAction = true;
				currentEffect.GetComponent<TrailRenderer>().emitting = false;
				particle.Stop();
				Destroy(curBall,particle.startLifetime);
				StopAllCoroutines();
				//enigmaPhysics.rBody.velocity = Vector3.zero;
				yield return null;

			}*/

			if(clampedDist<.8f)
			{
				Debug.Log("Target Hit");
				target = null;

				transform.position = targetPos;//col.ClosestPoint(transform.position+transform.up*.5f);
				animator.SetBool("Scripted Animation",false);
				enigmaPhysics.canTriggerAction = true;
				currentEffect.GetComponent<TrailRenderer>().emitting = false;
				particle.Stop();
				Destroy(curBall,particle.startLifetime);
				enigmaPhysics.rBody.velocity = Vector3.zero;

				if(jumpScript != null && col.transform.root.GetComponentInChildren<SpringObject>() == null)
				{
           	    	Debug.Log("Jump");
					StartCoroutine(jumpScript.Jump(jumpScript.initialJumpForce,jumpScript.jumpTimer,jumpScript.additiveJumpForce,enigmaPhysics.normal,false));
				}

				if(col.GetComponentInChildren<EnemyDamageAction>())
				{
					EnemyDamageAction enmDmgScript = col.GetComponentInChildren<EnemyDamageAction>();
					//enmDmgScript.canTakeDamage = false;
					StartCoroutine(enmDmgScript.EnemyDamage(gameObject));
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
		GameObject currentEffect = Instantiate(trailEffect,animator.transform.position+animator.transform.up * .5f,Quaternion.identity,animator.transform);
		GameObject curBall = Instantiate(ballEffect,animator.transform.position+animator.transform.up * .525f,Quaternion.identity,animator.transform);
		curBall.SetActive(true);
		currentEffect.SetActive(true);
		ParticleSystem particle = curBall.GetComponent<ParticleSystem>();
		var main = particle.main;

		enigmaPhysics.rBody.velocity = new Vector3(enigmaPhysics.rBody.velocity.x/2,enigmaPhysics.rBody.velocity.y,enigmaPhysics.rBody.velocity.z/2) + enigmaPhysics.forwardReference.normalized * force;
		while(enigmaPhysics.grounded == false && enigmaPhysics.characterState == 2)
		{
			enigmaPhysics.canTriggerAction = false;
			yield return new WaitForFixedUpdate();
		}
		currentEffect.GetComponent<TrailRenderer>().emitting = false;
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		curBall.transform.position = transform.position;
		particle.Stop();
		Destroy(curBall,particle.startLifetime);
		animator.SetBool("Scripted Animation",false);
		enigmaPhysics.canTriggerAction = true;
		yield return null;
	}
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
		animator = transform.root.GetComponentInChildren<Animator>();
		voice = transform.root.GetComponentInChildren<CharacterVoice>();
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
			//StopAllCoroutines();
		}
    }
}
