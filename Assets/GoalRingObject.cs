using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalRingObject : MonoBehaviour
{
    LevelEnd endScript;
    Animator animator;
    bool canTrigger = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        endScript = GetComponent<LevelEnd>();
    }

    void Update()
    {
        if(endScript.triggered == true && canTrigger == true)
        {
            canTrigger = false;
            StartCoroutine(TouchAnim());
        }
    }

    IEnumerator TouchAnim()
    {
        animator.CrossFadeInFixedTime("Touch",0,0,0);
        yield return null;
    }
}
