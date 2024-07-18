using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAnimation : MonoBehaviour
{
    public Renderer eyeRender;
    [Range(0,.5f)]
    public float eyeSensitivity;

    public Transform leftEyeBone;
    public Transform rightEyeBone;
    Vector3 leftBaseForward; 
    Vector3 leftBaseUp;
    Vector3 leftBaseRight;

    Vector3 rightBaseForward;
    Vector3 rightBaseUp;
    Vector3 rightBaseRight;

    public Material leftEye;
    public Material rightEye;


    // Start is called before the first frame update
    void Start()
    {
        leftBaseForward = leftEyeBone.parent.InverseTransformDirection(leftEyeBone.up);
        leftBaseUp = leftEyeBone.parent.InverseTransformDirection(leftEyeBone.forward);
        leftBaseRight = leftEyeBone.parent.InverseTransformDirection(leftEyeBone.right);

        rightBaseForward = rightEyeBone.parent.InverseTransformDirection(rightEyeBone.up);
        rightBaseUp = rightEyeBone.parent.InverseTransformDirection(rightEyeBone.forward);
        rightBaseRight = rightEyeBone.parent.InverseTransformDirection(rightEyeBone.right);

        leftEye = eyeRender.materials[1];
        rightEye = eyeRender.materials[0];
    }

    // Update is called once per frame
    void Update()
    {
       
        Vector3 leftForward = leftEyeBone.parent.InverseTransformDirection(leftEyeBone.up);
        Vector3 rightForward = rightEyeBone.parent.InverseTransformDirection(rightEyeBone.up);

        Debug.DrawRay(leftEyeBone.position,leftEyeBone.parent.TransformDirection(leftBaseForward), Color.blue);
        Debug.DrawRay(leftEyeBone.position,leftEyeBone.parent.TransformDirection(leftForward),Color.red);

        Debug.DrawRay(rightEyeBone.position,rightEyeBone.parent.TransformDirection(rightBaseForward), Color.blue);
        Debug.DrawRay(rightEyeBone.position,rightEyeBone.parent.TransformDirection(rightForward),Color.red);

        float leftXRot = Vector3.SignedAngle( Vector3.ProjectOnPlane(leftBaseForward,leftBaseUp) , Vector3.ProjectOnPlane(leftForward,leftBaseUp),leftBaseUp ); 
        float leftYRot = Vector3.SignedAngle( Vector3.ProjectOnPlane(leftBaseForward,leftBaseRight), Vector3.ProjectOnPlane(leftForward,leftBaseRight),leftBaseRight);

        float rightXRot = Vector3.SignedAngle( Vector3.ProjectOnPlane(rightBaseForward,rightBaseUp) , Vector3.ProjectOnPlane(rightForward,rightBaseUp),rightBaseUp ); 
        float rightYRot = Vector3.SignedAngle( Vector3.ProjectOnPlane(rightBaseForward,rightBaseRight), Vector3.ProjectOnPlane(rightForward,rightBaseRight),rightBaseRight);
        //Quaternion rightRot = leftBaseRot - leftEyeBone.localEulerAngles;
        
        leftEye.SetFloat("_Eye_X", leftXRot * eyeSensitivity);
        leftEye.SetFloat("_Eye_Y", -leftYRot * eyeSensitivity);
        rightEye.SetFloat("_Eye_X", rightXRot * eyeSensitivity);
        rightEye.SetFloat("_Eye_Y", -rightYRot * eyeSensitivity);

        //Debug.Log(leftXRot);
        /*
        leftEye.SetFloat("_Eye_X",Mathf.Sin(Time.time)*eyeSensitivity);
        leftEye.SetFloat("_Eye_Y",Mathf.Cos(Time.time)*eyeSensitivity);
        rightEye.SetFloat("_Eye_Y",Mathf.Sin(Time.time)*eyeSensitivity);
        rightEye.SetFloat("_Eye_X",Mathf.Cos(Time.time)*eyeSensitivity);
        */
    }
}
