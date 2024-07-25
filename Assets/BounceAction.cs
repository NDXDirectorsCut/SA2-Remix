using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    Animator animator;
    CharacterVoice voice;

    public GameObject trailEffect;
    public GameObject ballEffect;
    public AudioClip bounceSound;
    [Range(0,1)]
    public float volume;
    public float[] downForce;
    public float[] bounceForce;
    [Range(0,1)]
    public float angleInfluence;
    int bounceNumber; 

    GameObject currentEffect;
    GameObject curBall;


    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        animator = transform.root.GetComponentInChildren<Animator>();
        voice = transform.root.GetComponentInChildren<CharacterVoice>();
        bounceNumber = 0;
    }

    IEnumerator Bounce()
    {
        //enigmaPhysics.rBody.velocity = Vector3.zero;
        int downBounce = Mathf.Clamp(bounceNumber,0,downForce.Length-1);
        int upBounce = Mathf.Clamp(bounceNumber,0,bounceForce.Length-1);
        enigmaPhysics.rBody.velocity = -enigmaPhysics.referenceVector * downForce[downBounce];
        
        animator.CrossFadeInFixedTime("Spin",.25f,0,0);
        animator.SetBool("Scripted Animation",true);

        if( bounceNumber == 0)
        {
            currentEffect = Instantiate(trailEffect,animator.transform.position+animator.transform.up * .5f,Quaternion.identity,animator.transform);
            curBall = Instantiate(ballEffect,animator.transform.position+animator.transform.up * .525f,Quaternion.identity,animator.transform);
            curBall.SetActive(true);
            currentEffect.SetActive(true);
        }
        ParticleSystem particle = curBall.GetComponent<ParticleSystem>();
        bounceNumber += 1;

        AudioSource effectSource = voice.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		effectSource.clip = bounceSound;
		effectSource.volume = volume;
		effectSource.Play();

        Destroy(effectSource,bounceSound.length+5);


        bool check = false;
        while(enigmaPhysics.characterState == 2)
        {
            if(check == false)
            {
                if(Physics.Raycast(enigmaPhysics.rBody.position, -enigmaPhysics.referenceVector, out RaycastHit hit, 1f,enigmaPhysics.raycastLayers))
                {
                    check = true;
                    enigmaPhysics.rBody.position = hit.point+enigmaPhysics.referenceVector*.5f;
                    enigmaPhysics.grounded = false;
                    Vector3 dir = Vector3.Reflect(-enigmaPhysics.referenceVector,hit.normal);
                    float reflectAngle = Vector3.Angle(enigmaPhysics.referenceVector,dir);
                    enigmaPhysics.rBody.velocity = dir.normalized * bounceForce[upBounce] * Mathf.Lerp(1,reflectAngle,angleInfluence);
                }
                yield return new WaitForFixedUpdate();
            }
            if(enigmaPhysics.grounded == false)
            {
                animator.SetBool("Scripted Animation",true);
            }

            yield return new WaitForFixedUpdate();
        }
        
        animator.SetBool("Scripted Animation",false);
        bounceNumber = 0;
    
        currentEffect.GetComponent<TrailRenderer>().emitting = false;
		particle.Stop();
		Destroy(curBall,particle.startLifetime);

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire3") && enigmaPhysics.characterState == 2)
        {
            StartCoroutine(Bounce());
        }
    }
}
