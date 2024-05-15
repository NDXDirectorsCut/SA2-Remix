using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction : MonoBehaviour
{
    public float initialJumpForce;
    public float jumpTimer;
    public float additiveJumpForce;
    EnigmaPhysics enigmaPhysics;
    public Animator animator;
    Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        rBody = enigmaPhysics.rBody;
    }

    void Update()
    {
        //Input Handling in Update because I dont wanna input buffer
        if(Input.GetButtonDown("Jump") && enigmaPhysics.grounded == true)
        {
            StartCoroutine(Jump(initialJumpForce,jumpTimer,additiveJumpForce));
        }
    }

    IEnumerator Jump(float iJumpForce, float jTimer,float aJumpForce)
    {
        float initialJumpTime;
        float origRL;
        bool jumping;

        Debug.Log("Initial Jump");

        initialJumpTime = Time.time;
        jumping = true;

        enigmaPhysics.characterState = 2; enigmaPhysics.grounded = false;
        origRL = enigmaPhysics.raycastLength; enigmaPhysics.raycastLength = 0;
        Debug.Log(origRL);
        rBody.velocity += enigmaPhysics.normal * iJumpForce;
        
        animator.CrossFadeInFixedTime("Spin",.25f,0,0);
	  animator.SetBool("Scripted Animation",true);

        int i = 0;

        while(Time.time - initialJumpTime < jTimer && jumping == true)
        {
            Debug.Log(origRL);
            if(Input.GetButtonUp("Jump"))
            {
                jumping = false;
                enigmaPhysics.raycastLength = origRL;
                break;
            }
            if(Time.time - initialJumpTime > 0.1f)
            {
                enigmaPhysics.raycastLength = origRL;
                rBody.velocity += enigmaPhysics.normal * additiveJumpForce * Time.deltaTime;
                i++;
                Debug.Log("Additive Jump " + i);
            }
            yield return new WaitForFixedUpdate();
        }
	  animator.SetBool("Scripted Animation", false);
        //yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
