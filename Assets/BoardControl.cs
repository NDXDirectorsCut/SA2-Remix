using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControl : MonoBehaviour
{
    public EnigmaPhysics enigmaPhysics;
    Rigidbody rBody;
    public Animator animator;
    public float turningRate;
    public float airTurnRate;
    public float gravityForce;
    public float recoveryTime;
    public bool on;
    public bool brake;
    [Range(-1,1)]
    public float horizontal;
    [Range(0,3)]
    public float turnTime;
    float curVelo;
    Vector3 brakeVelo;
    Vector3 recoveryVelo;
    float recoveryVeloFl;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            rBody.velocity += enigmaPhysics.forwardReference*5f;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rBody = enigmaPhysics.rBody;

        if(on == true){
            enigmaPhysics.characterState = 3;
            Debug.DrawRay(enigmaPhysics.point,enigmaPhysics.normal,Color.blue);
            Debug.Log(enigmaPhysics.hit.transform != null);
            

            if(enigmaPhysics.hit.transform != null)
            {
                // Grounded
                float normalDeviance = Mathf.Clamp(Vector3.Angle(enigmaPhysics.referenceVector,enigmaPhysics.normal)/90,0,1);
                rBody.velocity += -enigmaPhysics.referenceVector * gravityForce * Time.deltaTime * normalDeviance * Mathf.Clamp(rBody.velocity.magnitude/100,1,100);

                Vector3 normalRight = Vector3.Cross(enigmaPhysics.normal,rBody.velocity.normalized).normalized;
                Vector3 normalForward = Vector3.Cross(normalRight,enigmaPhysics.normal).normalized;

                rBody.velocity = normalForward * rBody.velocity.magnitude;

                if(rBody.velocity.sqrMagnitude != 0)
                        enigmaPhysics.forwardReference = normalForward;

                rBody.transform.up = enigmaPhysics.normal;//rBody.rotation = Quaternion.LookRotation(enigmaPhysics.forwardReference,enigmaPhysics.normal);

                enigmaPhysics.point = Vector3.Lerp(enigmaPhysics.point,enigmaPhysics.hit.point,Mathf.Clamp(1 - Mathf.Pow(1 - enigmaPhysics.pointLerp, Time.deltaTime * 60 ),0,1));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(enigmaPhysics.point); localPoint.x = 0; localPoint.z = 0;
                enigmaPhysics.point = rBody.transform.TransformPoint(localPoint);
                rBody.position = enigmaPhysics.point;
                //rBody.velocity = 
                //enigmaPhysics.primaryAxis = new Vector3(horizontal,0,0);
                brake = Input.GetKey(KeyCode.LeftShift);
                if(brake == false)
                {
                    horizontal = Mathf.SmoothDamp(horizontal,Input.GetAxisRaw("Horizontal"),ref curVelo,turnTime) * Mathf.Clamp(rBody.velocity.magnitude,0,1);

                    float turnAngle = horizontal * turningRate * Time.deltaTime;
                    rBody.velocity = Quaternion.AngleAxis(turnAngle,enigmaPhysics.normal) * rBody.velocity;
                    

                    if(Mathf.Abs(horizontal) < .2f)
                    {
                        horizontal = Mathf.MoveTowards(horizontal,0,.125f * Time.deltaTime);
                        animator.CrossFadeInFixedTime("Board", .05f, 0 , Time.time, Time.time);
                    }/*
                    if(horizontal == 0)
                    {
                        animator.CrossFadeInFixedTime("Board", .125f, 0 , Time.time, 0);
                    }*/

                    if(horizontal > 0.05f)
                    {
                        animator.Play("Board R Transition",0 , Mathf.Abs(horizontal));//animator.CrossFadeInFixedTime("Board R Transition", .05f, 0 , Mathf.Abs(horizontal), 1-Mathf.Abs(horizontal));//
                    }
                    if(horizontal < -0.05f)
                    {
                        animator.Play("Board L Transition",0 , Mathf.Abs(horizontal));//animator.CrossFadeInFixedTime("Board L Transition", .05f, 0 , Mathf.Abs(horizontal), 1-Mathf.Abs(horizontal));
                    }
                }
                else
                {
                    
                    rBody.velocity = Vector3.SmoothDamp(rBody.velocity,Vector3.zero,ref brakeVelo,1);
                    if(horizontal > 0)
                    {
                        animator.CrossFadeInFixedTime("Board R Brake", .08f, 0 , Time.time * 2.5f, 0);
                        horizontal = 1;
                    }
                    else
                    {
                        animator.CrossFadeInFixedTime("Board L Brake", .08f, 0 , Time.time * 2.5f, 0);
                        horizontal = -1;
                    }
                }

            }
            else
            {
                // Airborne
                rBody.velocity += -enigmaPhysics.referenceVector * gravityForce * Time.deltaTime * Mathf.Clamp(rBody.velocity.magnitude/100,1,100);
                
                horizontal = Mathf.SmoothDamp(horizontal,Input.GetAxisRaw("Horizontal"),ref curVelo,turnTime);
                
                
                if(!Input.GetKey(KeyCode.LeftShift))
                {
                    enigmaPhysics.normal = Vector3.RotateTowards(enigmaPhysics.normal,enigmaPhysics.referenceVector,recoveryTime*Time.deltaTime,0).normalized;
                    rBody.transform.up = Vector3.RotateTowards(enigmaPhysics.normal,enigmaPhysics.referenceVector,recoveryTime*Time.deltaTime,0).normalized;
                    horizontal = Mathf.SmoothDamp(horizontal,0,ref recoveryVeloFl,recoveryTime);
                }

                float turnAngle = horizontal * airTurnRate * Time.deltaTime;
                float normalDeviance = Mathf.Clamp(Vector3.Angle(enigmaPhysics.referenceVector,enigmaPhysics.normal)/90,0,1);

                rBody.velocity = Quaternion.AngleAxis(turnAngle,enigmaPhysics.normal) * rBody.velocity;
                
                if(Mathf.Abs(horizontal) < .2f)
                    {
                        horizontal = Mathf.MoveTowards(horizontal,0,.125f * Time.deltaTime);
                        animator.CrossFadeInFixedTime("Board", .05f, 0 , Time.time, Time.time);
                    }/*
                    if(horizontal == 0)
                    {
                        animator.CrossFadeInFixedTime("Board", .125f, 0 , Time.time, 0);
                    }*/

                    if(horizontal > 0.05f)
                    {
                        animator.Play("Board R Transition",0 , Mathf.Abs(horizontal)/1.25f);//animator.CrossFadeInFixedTime("Board R Transition", .05f, 0 , Mathf.Abs(horizontal), 1-Mathf.Abs(horizontal));//
                    }
                    if(horizontal < -0.05f)
                    {
                        animator.Play("Board L Transition",0 , Mathf.Abs(horizontal)/1.25f);//animator.CrossFadeInFixedTime("Board L Transition", .05f, 0 , Mathf.Abs(horizontal), 1-Mathf.Abs(horizontal));
                    }

                Vector3 normalRight = Vector3.Cross(enigmaPhysics.normal,rBody.velocity.normalized).normalized;
                Vector3 normalForward = Vector3.Cross(normalRight,enigmaPhysics.normal).normalized;
                if(rBody.velocity.sqrMagnitude != 0)
                    enigmaPhysics.forwardReference = normalForward;
            }


            
        }
    }
}
