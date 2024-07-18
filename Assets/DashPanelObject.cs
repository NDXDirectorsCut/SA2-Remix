using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPanelObject : MonoBehaviour
{
    public float speed = 5;
    public bool additive = false;
    public bool setPosition = true;
    public bool lockInput = true;
    public float lockInputTime = 0.5f;
    EnigmaPhysics enigmaPhysics;
    InputPlayer inputPlayer;

    IEnumerator InputLock(InputPlayer inputPlayer)
    {
        inputPlayer.canMove = false;
        yield return new WaitForSeconds(lockInputTime);
        inputPlayer.canMove = true;
    }

    IEnumerator DashPanel(EnigmaPhysics enigmaPhysics, InputPlayer inputPlayer)
    {
        if(additive == false)
        {
            enigmaPhysics.rBody.velocity = transform.forward*speed;
        }
        else
        {
            enigmaPhysics.rBody.velocity = transform.forward * (speed + enigmaPhysics.rBody.velocity.magnitude);
        }

        if(setPosition == true)
        {
            enigmaPhysics.transform.position = transform.position;
        }

        if(lockInput == true && inputPlayer.canMove == true)
        {
            StartCoroutine(InputLock(inputPlayer));
        }

        yield return null;
    }

    void OnTriggerEnter(Collider touch)
    {
        if(touch.GetComponent<EnigmaPhysics>())
        {
            enigmaPhysics = touch.GetComponent<EnigmaPhysics>();
            inputPlayer = touch.GetComponent<InputPlayer>();

            StartCoroutine(DashPanel(enigmaPhysics, inputPlayer));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
