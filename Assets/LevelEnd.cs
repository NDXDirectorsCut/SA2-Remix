using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public Transform LevelEndPos;
    public Transform CameraPos;
    public float delay;
    public bool endOnTouch;
    public bool triggered = false;
    Transform player;
    EnigmaPhysics enigmaPhysics;
    EnigmaCamera camScript;
    GameObject resultsScreen;

    IEnumerator EndSequence()
    {
        triggered = true;
        enigmaPhysics.characterState = 3;

        player.GetComponent<InputPlayer>().enabled = false;
        enigmaPhysics.primaryAxis = Vector3.zero;


        if(player.root.GetComponentInChildren<EnigmaCamera>())
        {
            camScript = player.root.GetComponentInChildren<EnigmaCamera>();
            camScript.scripted = true;
        }

        yield return new WaitForSeconds(delay);

        enigmaPhysics.rBody.isKinematic = true;
        enigmaPhysics.enabled = false;

        Transform camObject = camScript.transform;
        camObject.parent = CameraPos;
        camObject.position = CameraPos.position;
        camObject.forward = CameraPos.forward;

        foreach (Transform child in camObject.GetComponentsInChildren<Transform>()) 
        {
            if(child.Find("ResultScreen") != null)
            {
                resultsScreen = child.Find("ResultScreen").gameObject;
                break;
            }
        }

        if(CameraPos.GetComponent<Animator>() != null)
        {
            CameraPos.GetComponent<Animator>().CrossFadeInFixedTime("Victory",0,0,0);
        }

        enigmaPhysics.forwardReference = LevelEndPos.forward;
        enigmaPhysics.normal = LevelEndPos.up;

        player.position = LevelEndPos.position;
        player.rotation = LevelEndPos.rotation;
        
        if(player.root.GetComponentInChildren<Animator>())
        {
            Debug.Log("Play Anim");
            Animator animator = player.root.GetComponentInChildren<Animator>();
            animator.CrossFadeInFixedTime("Victory Loop",0,0,0);
            //while(1>0)
            //{
                animator.SetBool("Scripted Animation", true);
                yield return null;
            //}
        }

        yield return new WaitForSeconds(delay);
        if(resultsScreen != null)
        resultsScreen.SetActive(true);
       
        yield return null;
    }

    void OnTriggerEnter(Collider col)
    {
        if(endOnTouch == true && col.GetComponentInChildren<EnigmaPhysics>() != null)
        {
            player = col.transform;
            enigmaPhysics = col.GetComponentInChildren<EnigmaPhysics>();
            endOnTouch = false;

            StartCoroutine(EndSequence());
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
