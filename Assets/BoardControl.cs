using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControl : MonoBehaviour
{
    public EnigmaPhysics enigmaPhysics;
    public Animator animator;
    public bool on;
    public bool brake;
    float horizontal;
    [Range(0,20)]
    public float sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(on == true)
            enigmaPhysics.characterState = 3;
        horizontal = Mathf.MoveTowards(horizontal,Input.GetAxisRaw("Horizontal"),sensitivity * Time.fixedDeltaTime);
        enigmaPhysics.primaryAxis = new Vector3(horizontal,0,0);
        brake = Input.GetKey(KeyCode.LeftShift);
        if(horizontal < 0)
        {
            animator.Play("Board L Transition",0 , Mathf.Abs(horizontal));
        }
        if(horizontal > 0)
        {
            animator.Play("Board R Transition",0 , Mathf.Abs(horizontal));
        }
        /*if(horizontal < 0.05f && horizontal > -0.01f)
        {
            animator.Play("Board");
        }*/
        animator.SetBool("Braking",brake);
        animator.SetFloat("Turning",horizontal);
    }
}
