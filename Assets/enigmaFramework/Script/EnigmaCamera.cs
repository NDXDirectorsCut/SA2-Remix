using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaCamera : MonoBehaviour
{
    public Transform lookTarget;
    public Vector3 offset;
    [Space(2.5f)]
    public Transform orbitTarget;
    [Range(0,15f)]
    public float distance;
    public float speed = 1;
    [Range(0,1)]
    public float inputSmoothness;

    float mouseX, mouseY;
    float deltaDeviance;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = (lookTarget.position - transform.position).normalized * distance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        deltaDeviance = Time.deltaTime / Time.fixedDeltaTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        float dl = Time.fixedDeltaTime;
        if(mouseX != mouseX || mouseY != mouseY)
        {
            mouseX = Input.GetAxisRaw("Mouse Y"); mouseY = Input.GetAxisRaw("Mouse X");
        }

        mouseX = Mathf.Lerp(mouseX,Input.GetAxisRaw("Mouse X")/ deltaDeviance,1-inputSmoothness); mouseY = Mathf.Lerp(mouseY,Input.GetAxisRaw("Mouse Y")/ deltaDeviance,1-inputSmoothness) ;
        Vector3 orbitPos = orbitTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;
        Vector3 lookPos = lookTarget.position + orbitTarget.right * offset.x + orbitTarget.up * offset.y + orbitTarget.forward * offset.z;

        Quaternion rot = Quaternion.AngleAxis(mouseX*speed,Vector3.up) * Quaternion.AngleAxis(-mouseY*speed,transform.right);
        //Debug.Log(mouseX + " " + Input.GetAxisRaw("Mouse X")/ deltaDeviance + " " + (1-inputSmoothness));
        Quaternion hor = Quaternion.AngleAxis(mouseX*(.2f/dl)*speed,Vector3.up) * Quaternion.identity;
        

        //Debug.Log(rot);
        transform.position = orbitPos - transform.forward * distance;
        transform.position = (rot * (transform.position - orbitPos)) + orbitPos;
        transform.forward = (lookPos - transform.position).normalized;

    }
}
