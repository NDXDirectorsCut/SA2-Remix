using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rank 
{
    public string name;
    public Sprite rankGraphic;
    public int requiredScore;
}


public class LevelEnd : MonoBehaviour
{
    [Header("Ranks")]
    public Rank[] ranks = new Rank[5];


    public Transform LevelEndPos;
    public Transform CameraPos;
    public float delay;
    public bool endOnTouch;
    public bool triggered = false;
    Transform player;
    EnigmaPhysics enigmaPhysics;
    EnigmaCamera camScript;
    GameObject resultsScreen;
    int totalRings;
    

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

        ResultLogic results = camObject.GetComponentInChildren<ResultLogic>(true);
        results.totalRings = totalRings;
        results.gameObject.SetActive(true);
        StartCoroutine(results.UpdateValues());

        for(int i = 0; i < ranks.Length; i++)
        {
            if(results.totalScore > ranks[i].requiredScore)
            {
                results.rankImage.sprite = ranks[i].rankGraphic;
                break;
            }
        }
        
        TimerLogic timer = camObject.GetComponentInChildren<TimerLogic>();
        timer.running = false;

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
        var foundRings = FindObjectsOfType<RingObject>();
        totalRings = foundRings.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
