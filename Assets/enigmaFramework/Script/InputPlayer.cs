using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public enum InputReference { Transform, TransformGrounded, Spline}

public class InputPlayer : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public bool canMove;
    
    public InputReference inputReference;
    public GameObject referenceObject;
    public float splineExaggeration = 1;
    public Vector3 primaryAxis { get; private set;}
    float hor,ver;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    public IEnumerator InputLock(float lockInputTime)
    {
        canMove = false;
        yield return new WaitForSeconds(lockInputTime);
        canMove = true;
    }

    void Update()
    {
        if(canMove == true) 
        {
            hor = Input.GetAxisRaw("Horizontal");
            ver = Input.GetAxisRaw("Vertical");
        }
        else
        {
            hor = 0; ver = 0;
        }
        switch(inputReference)
        {
            case InputReference.Transform: // make input vector relative to the reference object
                primaryAxis = new Vector3(hor,0,ver);
                primaryAxis = referenceObject.transform.TransformDirection(primaryAxis);
                break;
            case InputReference.TransformGrounded: // make input vector relative to the reference object but align it to the normal
                primaryAxis = new Vector3(hor,0,ver);
                primaryAxis = Quaternion.FromToRotation(referenceObject.transform.up,enigmaPhysics.normal) * referenceObject.transform.TransformDirection(primaryAxis);
                primaryAxis = Vector3.ClampMagnitude(primaryAxis,1);
                break;
            case InputReference.Spline: // make input vector aligned to a spline
                if(referenceObject.GetComponent<Spline>() != null)
                {
                    Spline moveSpline = referenceObject.GetComponent<Spline>();
                    CurveSample splineSample = moveSpline.GetProjectionSample(moveSpline.transform.InverseTransformPoint(transform.position + primaryAxis * splineExaggeration));
                    
                    primaryAxis = new Vector3(-hor,0,-ver);
                    primaryAxis = Quaternion.Slerp(Quaternion.identity,Quaternion.FromToRotation(Vector3.forward,splineSample.tangent),splineExaggeration) * primaryAxis;
                    Vector3 toCenter = ((referenceObject.transform.position + splineSample.location) - transform.position) * primaryAxis.magnitude;
                    primaryAxis = Vector3.Lerp(primaryAxis,toCenter,toCenter.magnitude*.125f);
                    primaryAxis = Vector3.ClampMagnitude(primaryAxis,1);
                }
                break;
        }
        // transfer the primaryAxis over to main physics script
        enigmaPhysics.primaryAxis = primaryAxis;
        Debug.DrawRay(transform.position,primaryAxis,Color.yellow);

    }
    
}
