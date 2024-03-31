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
    void FixedUpdate()
    {
        transform.position = enigmaPhysics.rBody.position;
        transform.rotation = Quaternion.LookRotation(enigmaPhysics.forwardReference,enigmaPhysics.normal);

        animator.SetFloat("Velocity Magnitude",enigmaPhysics.rBody.velocity.magnitude);
        animator.SetInteger("Character State",enigmaPhysics.characterState);
    }
}
