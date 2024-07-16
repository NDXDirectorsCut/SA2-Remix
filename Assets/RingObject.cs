using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject : MonoBehaviour
{
    public int ringValue = 1;
    public bool canPickup = true;
    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        sound = transform.root.GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<RingAction>() != null && canPickup == true)
        {
            RingAction ringScript = col.GetComponent<RingAction>();
            ScoreSystem scoreScript = col.GetComponent<ScoreSystem>();
            scoreScript.score += 10;
            ringScript.ringCount += ringValue;
            sound.Play();
            Destroy(gameObject);
            Destroy(transform.root.gameObject,2.5f);
            
        }
    }
}
