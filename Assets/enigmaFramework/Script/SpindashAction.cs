using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindashAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    Rigidbody rBody;
    Animator animator;
    public GameObject trailEffect;
    public GameObject ballEffect;
    public float slopeForce = 1;
    public float triggerTime;
    public float holdTime;
    public float topSpeed;
    public float minSpeed;
    public float rollDeceleration;
    bool holding; float startHold;
    [Header("Sounds")]
    public AudioClip chargeStart;
    public AudioClip chargeLoop;
    public AudioClip release;
    [Range(0,1)]
    public float volume = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        animator = transform.root.GetComponentInChildren<Animator>();
        rBody = enigmaPhysics.rBody;
    }

    IEnumerator SpindashSound(AudioSource sound)
    {
        yield return new WaitForSeconds(chargeStart.length - 0.05f);
        if(Input.GetButton("Fire3"))
        {
            sound.clip = chargeLoop;
            sound.loop = true;
            sound.Play();
        }
    }

    IEnumerator Spindash()
    {
	    Vector3 pos = transform.position;
        float velocity = rBody.velocity.magnitude;
	    float startTime = Time.time;
	    animator.CrossFadeInFixedTime("Spindash",.25f,0,0);
        enigmaPhysics.canTriggerAction = false;
	    float time = 0;

        AudioSource chargeSound = animator.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        chargeSound.clip = chargeStart;
        chargeSound.volume = volume;
        chargeSound.Play();
        StartCoroutine(SpindashSound(chargeSound));

	    while(Input.GetButton("Fire3"))
	    {
            animator.SetBool("Scripted Animation",true);
            time = Mathf.Clamp((Time.time-startTime)/holdTime,0,1);

            //transform.position = Vector3.Lerp(transform.position,pos,1/3f);
            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,.1f);
            if(enigmaPhysics.primaryAxis.magnitude > .1f)
                enigmaPhysics.forwardReference = enigmaPhysics.primaryAxis;

            if(enigmaPhysics.characterState != 1)
            {
                animator.SetBool("Scripted Animation",false);
                enigmaPhysics.canTriggerAction = true;
                Destroy(chargeSound);
                StopAllCoroutines();
            }
            yield return new WaitForFixedUpdate();
	    }
        float speed = velocity > .2f ? Mathf.Lerp(velocity,topSpeed,time) : Mathf.Lerp(minSpeed,topSpeed,time);
        chargeSound.clip = release;
        chargeSound.loop = false;
        chargeSound.Play();
        Destroy(chargeSound,release.length + 2.5f);
	    //if(Input.GetKeyUp(KeyCode.LeftShift))
	    //{
	        StartCoroutine(Roll(speed,rollDeceleration));
	    //}
        //else
        //{
        //    animator.SetBool("Scripted Animation",false);
        //    enigmaPhysics.canTriggerAction = true;
        //}
        
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
            if(Input.GetButtonDown("Fire3"))
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
            Debug.DrawRay(transform.position,slopeVector * enigmaPhysics.linearSlopeForce.magnitude * slopeForce * veloRatio,Color.red);
            activeSpeed -= enigmaPhysics.linearSlopeForce.magnitude * slopeForce * veloRatio;
            //Debug.Log(veloRatio);

            rBody.velocity = enigmaPhysics.forwardReference * activeSpeed;
            yield return new WaitForFixedUpdate();
    	}
        Debug.Log("Spindash Stop");
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
        if(enigmaPhysics.characterState == 1)
        {
            if(holding == false)
                startHold = Time.time;
        
            if(Input.GetButtonDown("Fire3"))
            {
                holding = true;
                startHold = Time.time;
            }
            
            if(Input.GetButtonUp("Fire3"))
            {
                holding = false;
            }

            if(Time.time - startHold >= triggerTime && enigmaPhysics.characterState == 1 && holding == true && enigmaPhysics.canTriggerAction == true)
            {
                StartCoroutine(Spindash());
                holding = false;
            }
      }
      //if(enigmaPhysics.characterState != 1 )
      //  StopAllCoroutines();
    }
}