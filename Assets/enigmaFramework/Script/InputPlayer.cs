using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class InputPlayer : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public bool canMove;
    public enum InputReference { Transform, TransformGrounded, Spline}
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
            case InputReference.Transform:

                break;
            case InputReference.TransformGrounded:
                primaryAxis = new Vector3(hor,0,ver);
                primaryAxis = Quaternion.FromToRotation(referenceObject.transform.up,enigmaPhysics.normal) * referenceObject.transform.TransformDirection(primaryAxis);
                primaryAxis = Vector3.ClampMagnitude(primaryAxis,1);
                break;
            case InputReference.Spline:
                if(referenceObject.GetComponent<Spline>() != null)
                {
                    Spline moveSpline = referenceObject.GetComponent<Spline>();
                    CurveSample splineSample = moveSpline.GetProjectionSample(moveSpline.transform.InverseTransformPoint(transform.position + primaryAxis * splineExaggeration));
                    
                    primaryAxis = new Vector3(hor,0,ver);
                    primaryAxis = Quaternion.Slerp(Quaternion.identity,Quaternion.FromToRotation(Vector3.forward,splineSample.tangent),splineExaggeration) * primaryAxis;
                    primaryAxis = Vector3.ClampMagnitude(primaryAxis,1);
                }
                break;
        }
        enigmaPhysics.primaryAxis = primaryAxis;
        Debug.DrawRay(transform.position,primaryAxis,Color.yellow);

    }
    
    /*
    void FixedUpdate()
    {
        

        switch(enigmaPhysics.characterState)
        {
            case 0: //Debug
                break;
            case 1: //Grounded
                break;
            case 2: //Airborne
                break;
            case 3: //Scripted
                break;
        }
    }*/

}
