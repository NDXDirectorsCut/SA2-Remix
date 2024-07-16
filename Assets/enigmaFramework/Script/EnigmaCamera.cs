using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaCamera : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public bool scripted;
    public Vector3 referenceVector;
    
    [Space(5f)]
    public Transform lookTarget;
    public Vector3 offset;
    [Space(2.5f)]
    public Transform orbitTarget;
    [Space(5f)]
    public float angleCutoff;
    [Range(0,15f)]
    public float distance;
    public float collisionBuffer;
    public LayerMask layers;
    [Range(0,1)]
    public float bufferLerp;
    float currentBuffer;
    public float speed = 1;
    public float facingSpeed;
    public float faceTime;
    public bool useTransformForward;
    public AnimationCurve lerpCurve;
    public float lerpForce = 1;
    [Range(0,1)]
    public float inputSmoothness;

    float mouseX, mouseY;
    float deltaDeviance = 1;
    [Range(0,120)]
    public int targetFramerate;

    Vector3 verticalV3,horizontalV3;
    Quaternion prevRot;
    float stopTime;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = (lookTarget.position - transform.position).normalized * distance;
        enigmaPhysics = transform.parent.Find("EnigmaCharacter").GetComponent<EnigmaPhysics>();
        Cursor.lockState = CursorLockMode.Locked;
        referenceVector = Vector3.up;
        //Application.targetFrameRate = 120;
    }
    /*
    void Update()
    {
        deltaDeviance = Time.deltaTime / Time.fixedDeltaTime;
        //Application.targetFrameRate = 120;
    }*/
    // Update is called once per frame
    void FixedUpdate()
    {
        Application.targetFrameRate = targetFramerate;
        if(scripted == false)
        {
            Vector3 prevReference = referenceVector;
            referenceVector = Vector3.Slerp(referenceVector,enigmaPhysics.normal,lerpCurve.Evaluate(Vector3.Angle(referenceVector,enigmaPhysics.normal)) * Time.deltaTime * lerpForce);

            //Turn Camera Up to Normal
            //transform.up = Quaternion.FromToRotation(prevReference,referenceVector) * transform.up;

            //Mouse Input
            float dl = Time.fixedDeltaTime;
            if(mouseX != mouseX || mouseY != mouseY)
            {
                mouseX = Input.GetAxisRaw("Mouse Y"); mouseY = Input.GetAxisRaw("Mouse X");
            }

            mouseX = Mathf.Lerp(mouseX,Input.GetAxisRaw("Mouse X")/ deltaDeviance,1-inputSmoothness); mouseY = Mathf.Lerp(mouseY,Input.GetAxisRaw("Mouse Y")/ deltaDeviance,1-inputSmoothness) ;
            Vector3 orbitPos = orbitTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;
            Vector3 lookPos = lookTarget.position + lookTarget.right * offset.x + lookTarget.up * offset.y + lookTarget.forward * offset.z;

            transform.position = (Quaternion.AngleAxis(mouseX*speed*Time.deltaTime,referenceVector) * (transform.position-lookPos)) + lookPos;
            transform.forward = Quaternion.AngleAxis(mouseX*speed*Time.deltaTime,referenceVector) * transform.forward;
            //Debug.DrawRay(transform.position,transform.forward,Color.magenta);
            //Debug.DrawRay(transform.position,Vector3.ProjectOnPlane(transform.forward,referenceVector),Color.blue);

            //Vertical Handling
            Vector3 planeForward = Vector3.ProjectOnPlane(transform.forward,referenceVector).normalized;
            Debug.DrawRay(transform.position,planeForward,Color.blue);
            Debug.DrawRay(transform.position,Quaternion.AngleAxis(angleCutoff,transform.right) * planeForward,Color.red);
            Debug.DrawRay(transform.position,Quaternion.AngleAxis(-angleCutoff,transform.right) * planeForward,Color.green);
            float yAngle = Vector3.SignedAngle(transform.forward,planeForward,transform.right);
            if( Mathf.Abs(yAngle) <= angleCutoff)
            {
                transform.position = (Quaternion.AngleAxis(mouseY*speed*Time.deltaTime,transform.right) * (transform.position-lookPos)) + lookPos;
                transform.forward = Quaternion.AngleAxis(mouseY*speed*Time.deltaTime,transform.right) * transform.forward;
            }
            else
            {
                    if(yAngle > 0)
                {
                    transform.forward = Vector3.RotateTowards(transform.forward,Quaternion.AngleAxis(angleCutoff,transform.right) * planeForward,.2f*Time.deltaTime,0);
                }
                else
                {
                    transform.forward = Vector3.RotateTowards(transform.forward,Quaternion.AngleAxis(-angleCutoff,transform.right) * planeForward,.2f*Time.deltaTime,0);
                }
            }
            //transform.up =transform.up;
            //Turn camera to where the player is facing
            float xAngle = useTransformForward ? Vector3.SignedAngle(transform.forward,lookTarget.forward,referenceVector)
                            : Vector3.SignedAngle(transform.forward,enigmaPhysics.forwardReference,referenceVector);
            if(mouseX < 0.01f && mouseX > -0.01f)
            {
                float clampedSpeed = Mathf.Lerp(0,facingSpeed,Mathf.Clamp(Mathf.Abs(xAngle)/15,0,1));
                //Debug.Log(clampedSpeed + " " + Mathf.Clamp(Mathf.Abs(xAngle)/15,0,1));
                transform.position = (Quaternion.AngleAxis(xAngle*clampedSpeed*Time.deltaTime,referenceVector) * (transform.position-lookPos)) + lookPos;
                transform.forward = Quaternion.AngleAxis(xAngle*clampedSpeed*Time.deltaTime,referenceVector) * transform.forward;
            }

            /*
            verticalV3 += new Vector3(-mouseY*speed,0,0);
            if(verticalV3.x >= angleCutoff.y)
            {
                verticalV3.x = Mathf.Lerp(verticalV3.x,angleCutoff.y,.5f);
            }
            if(verticalV3.x <= angleCutoff.x)
            {
                verticalV3.x = Mathf.Lerp(verticalV3.x,angleCutoff.x,.5f);
            }
            verticalV3.x = Mathf.Clamp(verticalV3.x,-90,90);
            horizontalV3 += new Vector3(0,mouseX*speed,0);

            Quaternion horizontalQ = Quaternion.Euler(horizontalV3); Quaternion verticalQ = Quaternion.Euler(verticalV3);
            Quaternion finalRot = horizontalQ * verticalQ;
            transform.rotation = finalRot;
            */
            RaycastHit hit;

            if(Physics.Raycast(orbitPos,-transform.forward,out hit,distance,layers))
            {
                transform.position = hit.point+ transform.forward*currentBuffer;
                currentBuffer = Mathf.Lerp(currentBuffer,collisionBuffer,bufferLerp / (Time.deltaTime/0.01666666f));
            }
            else
            {
                transform.position = orbitPos - transform.forward * distance;
                currentBuffer = 0;
            }
            transform.rotation = Quaternion.LookRotation( Quaternion.FromToRotation(prevReference,referenceVector) * (lookPos - transform.position).normalized,referenceVector);
        }
    }
}
