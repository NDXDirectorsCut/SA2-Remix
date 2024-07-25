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
    AudioSource sound;

    IEnumerator InputLock(InputPlayer inputPlayer)
    {
        inputPlayer.canMove = false;
        yield return new WaitForSeconds(lockInputTime);
        inputPlayer.canMove = true;
    }

    IEnumerator DashPanel(EnigmaPhysics enigmaPhysics, InputPlayer inputPlayer)
    {
        sound.Play();
        enigmaPhysics.grounded = true;
        enigmaPhysics.characterState = 1;
        enigmaPhysics.forwardReference = transform.forward;
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
            StartCoroutine(inputPlayer.InputLock(lockInputTime));
            while(inputPlayer.canMove == false)
            {
                enigmaPhysics.primaryAxis = Quaternion.FromToRotation(transform.up,enigmaPhysics.normal) * transform.forward;
                yield return new WaitForFixedUpdate();
            }
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
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
