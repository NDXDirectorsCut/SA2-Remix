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
        // load settings
        speed = PlayerPrefs.GetFloat("CameraSensitivity");

        enigmaPhysics = transform.parent.Find("EnigmaCharacter").GetComponent<EnigmaPhysics>();
        Cursor.lockState = CursorLockMode.Locked;
        referenceVector = Vector3.up;
    }
    
    void Update()
    {
        deltaDeviance = Time.deltaTime / Time.fixedDeltaTime;
        speed = PlayerPrefs.GetFloat("CameraSensitivity");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Application.targetFrameRate = targetFramerate;
        if(scripted == false) // use normal behavior
        {
            Vector3 prevReference = referenceVector;
            referenceVector = Vector3.Slerp(referenceVector,enigmaPhysics.normal,lerpCurve.Evaluate(Vector3.Angle(referenceVector,enigmaPhysics.normal)) * Time.deltaTime * lerpForce);

            // mouse Input
            float dl = Time.fixedDeltaTime;
            mouseX = Mathf.Lerp(mouseX,Input.GetAxisRaw("Mouse X")/1 ,1-inputSmoothness); mouseY = Mathf.Lerp(mouseY,Input.GetAxisRaw("Mouse Y")/1 ,1-inputSmoothness) ;
            
            // offset positions of targets
            Vector3 orbitPos = orbitTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;
            Vector3 lookPos = lookTarget.position + lookTarget.right * offset.x + lookTarget.up * offset.y + lookTarget.forward * offset.z;

            //rotate camera around a pivot
            transform.position = (Quaternion.AngleAxis(mouseX*speed*Time.fixedDeltaTime,referenceVector) * (transform.position-lookPos)) + lookPos;
            transform.forward = Quaternion.AngleAxis(mouseX*speed*Time.fixedDeltaTime,referenceVector) * transform.forward;

            // normal handling
            Vector3 planeForward = Vector3.ProjectOnPlane(transform.forward,referenceVector).normalized;

            float yAngle = Vector3.SignedAngle(transform.forward,planeForward,transform.right);
            //clamp camera angle
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

            // turn camera to where the player is facing
            Vector3 projForward = Vector3.ProjectOnPlane(transform.forward,referenceVector);
            Vector3 projForwardRef = Vector3.ProjectOnPlane(enigmaPhysics.forwardReference,referenceVector);

            float xAngle = useTransformForward ? Vector3.SignedAngle(transform.forward,lookTarget.forward,referenceVector)
                            : Vector3.SignedAngle(projForward,projForwardRef,referenceVector);
            if(mouseX < 0.01f && mouseX > -0.01f && enigmaPhysics.rBody.velocity.magnitude > 0.5f)
            {
                float finalTurnAngle = xAngle * Time.fixedDeltaTime * facingSpeed;
                finalTurnAngle = Mathf.Abs(finalTurnAngle) > Mathf.Abs(xAngle) ? xAngle : finalTurnAngle;

                transform.position = (Quaternion.AngleAxis(finalTurnAngle,referenceVector) * (transform.position-lookPos)) + lookPos;
                transform.forward = Quaternion.AngleAxis(finalTurnAngle,referenceVector) * transform.forward;
            }

            // camera collision
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

            // final camera rotation
            transform.rotation = Quaternion.LookRotation( Quaternion.FromToRotation(prevReference,referenceVector) * (lookPos - transform.position).normalized,referenceVector);
        }
    }
}
