using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public Animator animator;
    public GameObject effect;
    InputPlayer plInput;
    //InputAI aiInput;
    public float skidDeceleration;
    public float requiredAngle;
	public float minSpeed;
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
	  plInput = GetComponent<InputPlayer>();
	  //aiInput = GetComponent<InputAI>();w
    }
	
    IEnumerator Skid(float deceleration)
    {
		animator.CrossFadeInFixedTime("Skid",.25f,0,0);
		animator.SetBool("Scripted Animation",true);
		GameObject currentEffect = Instantiate(effect,animator.transform.position,Quaternion.identity,animator.transform);
		currentEffect.SetActive(true);
		ParticleSystem particle = currentEffect.GetComponent<ParticleSystem>();
		Destroy(currentEffect, particle.duration + particle.startLifetime);
		while(enigmaPhysics.characterState == 1 && enigmaPhysics.rBody.velocity.magnitude > .1f)
		{
		if(plInput != null)
			plInput.canMove = false;
		//if(aiInput != null)
		//	aiInput.canMove = false;
		float gravityMultiplier = 1-Mathf.Clamp(Vector3.Angle(enigmaPhysics.referenceVector,enigmaPhysics.normal)/90,0,1); 
		enigmaPhysics.rBody.velocity -= enigmaPhysics.rBody.velocity * skidDeceleration * gravityMultiplier * Time.fixedDeltaTime * Mathf.Clamp(enigmaPhysics.rBody.velocity.magnitude,0,1);
		yield return new WaitForFixedUpdate();
		}
		plInput.canMove = true;
		animator.SetBool("Scripted Animation",false);
		yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	 float angle = Vector3.Angle(enigmaPhysics.rBody.velocity,enigmaPhysics.primaryAxis);
       if(angle > requiredAngle && enigmaPhysics.characterState == 1 && enigmaPhysics.rBody.velocity.magnitude > minSpeed)
		StartCoroutine(Skid(skidDeceleration));
    }
}
