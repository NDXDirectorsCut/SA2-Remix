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
        while(enigmaPhysics.characterState == 1)
        {
            activeSpeed -= rollDecel * Time.fixedDeltaTime;
            rBody.velocity = enigmaPhysics.forwardReference * activeSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Roll(30,rollDeceleration));
        }
    }
}
