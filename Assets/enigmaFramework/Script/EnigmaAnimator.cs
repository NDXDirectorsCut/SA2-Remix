using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaAnimator : MonoBehaviour
{
    Transform enigmaCharacter;
    EnigmaPhysics enigmaPhysics;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        enigmaCharacter = transform.parent.Find("EnigmaCharacter");
        enigmaPhysics = enigmaCharacter.GetComponent<EnigmaPhysics>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = enigmaPhysics.rBody.position;
        transform.rotation = Quaternion.LookRotation(enigmaPhysics.forwardReference,enigmaPhysics.normal);
        Vector3 localVelo = enigmaCharacter.InverseTransformDirection(enigmaPhysics.rBody.velocity);

        animator.SetFloat("Velocity Magnitude",enigmaPhysics.rBody.velocity.magnitude);
        animator.SetFloat("VeloX",localVelo.x);
        animator.SetFloat("VeloY",localVelo.y);
        animator.SetFloat("VeloZ",localVelo.z);
        animator.SetInteger("Character State",enigmaPhysics.characterState);
        
        

    }
}
