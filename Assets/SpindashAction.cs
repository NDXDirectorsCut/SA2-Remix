using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindashAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    Rigidbody rBody;
    public Animator animator;
    public float triggerTime;
    public float holdTime;
    public float topSpeed;
    public float rollDeceleration;
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        rBody = enigmaPhysics.rBody;
    }

    IEnumerator Spindash()
    {
        yield return new WaitForFixedUpdate();
    }

    IEnumerator Roll(float rollSpeed,float rollDecel)
    {
        float activeSpeed = rollSpeed;
        rBody.velocity = enigmaPhysics.forwardReference.normalized * activeSpeed;
        animator.CrossFadeInFixedTime("Ground Spin",.25f,0,0);
        animator.SetBool("Scripted Animation",true);
        while(enigmaPhysics.characterState == 1 && rBody.velocity.magnitude > .25f)
        {
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Roll(topSpeed,rollDeceleration));
        }
    }
}
