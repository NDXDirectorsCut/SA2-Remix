using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaCamera : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public Vector3 referenceVector;
    [Space(5f)]
    public Transform lookTarget;
    public Vector3 offset;
    [Space(2.5f)]
    public Transform orbitTarget;
    [Space(5f)]
    public Vector2 angleCutoff;
    [Range(0,15f)]
    public float distance;
    public float collisionBuffer;
    public LayerMask layers;
    [Range(0,1)]
    public float bufferLerp;
    float currentBuffer;
    public float speed = 1;
    [Range(0,1)]
    public float inputSmoothness;

    float mouseX, mouseY;
    float deltaDeviance = 1;
    [Range(0,120)]
    public int targetFramerate;

    Vector3 verticalV3,horizontalV3;
    Quaternion prevRot;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = (lookTarget.position - transform.position).normalized * distance;
        enigmaPhysics = transform.parent.Find("EnigmaCharacter").GetComponent<EnigmaPhysics>();
        Cursor.lockState = CursorLockMode.Locked;
        //Application.targetFrameRate = 120;
    }

    void Update()
    {
        deltaDeviance = Time.deltaTime / Time.fixedDeltaTime;
        //Application.targetFrameRate = 120;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Application.targetFrameRate = targetFramerate;
        referenceVector = enigmaPhysics.normal;
        float dl = Time.fixedDeltaTime;
        if(mouseX != mouseX || mouseY != mouseY)
        {
            mouseX = Input.GetAxisRaw("Mouse Y"); mouseY = Input.GetAxisRaw("Mouse X");
        }

        mouseX = Mathf.Lerp(mouseX,Input.GetAxisRaw("Mouse X")/ deltaDeviance,1-inputSmoothness); mouseY = Mathf.Lerp(mouseY,Input.GetAxisRaw("Mouse Y")/ deltaDeviance,1-inputSmoothness) ;
        Vector3 orbitPos = orbitTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;
        Vector3 lookPos = lookTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;

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
        //verticalRot = Quaternion.FromToRotation(Vector3.up,referenceVector) * verticalRot;
        horizontalV3 += new Vector3(0,mouseX*speed,0);
        // /Debug.Log(mouseX);
        Quaternion horizontalQ = Quaternion.FromToRotation(Vector3.up,referenceVector) * Quaternion.Euler(horizontalV3); Quaternion verticalQ = Quaternion.Euler(verticalV3);
        Quaternion finalRot = horizontalQ * verticalQ;
        transform.rotation = finalRot;

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
        transform.rotation = Quaternion.LookRotation((lookPos - transform.position).normalized,referenceVector);
        //transform.position = (diff * (transform.position - orbitPos)) + orbitPos;
        //transform.forward = (lookPos - transform.position).normalized;
        //transform.rotation = finalRot;
        
        /*Quaternion rot = Quaternion.AngleAxis(mouseX*speed,Vector3.up) * Quaternion.AngleAxis(-mouseY*speed,transform.right);
        //rot = Quaternion.Euler(rot.eulerAngles);
        //Debug.Log(mouseX + " " + Input.GetAxisRaw("Mouse X")/ deltaDeviance + " " + (1-inputSmoothness));
        Quaternion hor = Quaternion.AngleAxis(mouseX*(.2f/dl)*speed,Vector3.up) * Quaternion.identity;
        

        //Debug.Log(rot);
        transform.forward = (lookPos - transform.position).normalized;
        
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
        
        transform.position = (rot * (transform.position - orbitPos)) + orbitPos;
        transform.forward = (lookPos - transform.position).normalized;
        */
    }
}
