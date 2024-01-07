using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionMovement : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public enum InputReference { Transform, TransformGrounded, Spline}
    public InputReference inputReference;
    public GameObject referenceObject;
    public Vector3 axisInput { get; private set;}
    float hor,ver;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    void Update()
    {
        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");
        
        switch(inputReference)
        {
            case InputReference.Transform:

                break;
            case InputReference.TransformGrounded:
                axisInput = new Vector3(hor,0,ver);
                axisInput = Quaternion.FromToRotation(referenceObject.transform.up,enigmaPhysics.normal) * referenceObject.transform.TransformDirection(axisInput);
                axisInput = Vector3.ClampMagnitude(axisInput,1);
                break;
            case InputReference.Spline:

                break;
        }
        Debug.DrawRay(transform.position,axisInput,Color.yellow);

    }

    // Update is called once per frame
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
    }

}
