using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction : MonoBehaviour
{
    public float initialJumpForce;
    public float jumpTimer;
    public float additiveJumpForce;
    EnigmaPhysics enigmaPhysics;
    Animator animator;
    CharacterVoice voice;
    Rigidbody rBody;
    public AudioClip jumpSound;
    [Range(0,1)]
    public float volume = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
        animator = transform.root.GetComponentInChildren<Animator>();
        voice = transform.root.GetComponentInChildren<CharacterVoice>();
        rBody = enigmaPhysics.rBody;
    }

    void LateUpdate()
    {
        //Input Handling in Update because I dont wanna input buffer
        if(Input.GetButtonDown("Jump") && enigmaPhysics.grounded == true)
        {
            StartCoroutine(Jump(initialJumpForce,jumpTimer,additiveJumpForce,enigmaPhysics.normal,true));
        }
    }

    public IEnumerator Jump(float iJumpForce, float jTimer,float aJumpForce, Vector3 direction,bool playSound)
    {
        yield return new WaitForFixedUpdate();
        float initialJumpTime;
        float origRL;
        bool jumping;

        Debug.Log("Initial Jump");

        initialJumpTime = Time.time;
        jumping = true;

        enigmaPhysics.characterState = 2; enigmaPhysics.grounded = false;
        origRL = enigmaPhysics.activeRayLen; enigmaPhysics.activeRayLen = 0.7f;
        //Debug.Log(origRL);
        rBody.velocity += direction * iJumpForce;
        
        animator.CrossFadeInFixedTime("Spin",.25f,0,0);
	    animator.SetBool("Scripted Animation",true);

        if(playSound == true)
        {
            AudioSource voiceSource = voice.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		    AudioClip sound = voice.smallGruntSounds[Random.Range(0, voice.smallGruntSounds.Length-1 )];
		    voiceSource.clip = sound;
            voiceSource.Play();
            AudioSource effectSource = voice.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            effectSource.clip = jumpSound;
            effectSource.volume = volume;
            effectSource.Play();
		    Destroy(voiceSource,sound.length+20);
            Destroy(effectSource,jumpSound.length+20);
        }

        enigmaPhysics.canTriggerAction = false;

        int i = 0;

        while(Time.time - initialJumpTime < jTimer && jumping == true && enigmaPhysics.characterState == 2)
        {
            
            //Debug.Log(origRL);
            if(!Input.GetButton("Jump"))
            {
                jumping = false;
                Debug.Log("Stop Jump");
                enigmaPhysics.activeRayLen = origRL;
                break;
            }
            if(Time.time - initialJumpTime > 0.1f)
            {
                animator.SetBool("Scripted Animation",true);
                //enigmaPhysics.activeRayLen = origRL;
                rBody.velocity += enigmaPhysics.normal * additiveJumpForce * Time.deltaTime;
                enigmaPhysics.canTriggerAction = true;
                i++;
                //Debug.Log("Additive Jump " + i);
            }
            yield return new WaitForFixedUpdate();
        }
	    animator.SetBool("Scripted Animation", false);
        enigmaPhysics.canTriggerAction = true;
        //yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
