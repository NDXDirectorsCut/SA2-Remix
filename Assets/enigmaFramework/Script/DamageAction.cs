using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageAction : MonoBehaviour
{
    RingAction ringScript;
    public bool canTakeDamage = true;
    // Start is called before the first frame update
    void Start()
    {
	if(GetComponent<RingAction>())
            ringScript = GetComponent<RingAction>();
    }

    IEnumerator Die()
    {
	SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	yield return null;
    }

    public IEnumerator TakeDamage()
    {
	if(ringScript != null && ringScript.ringCount > 0 )
	{
	    StartCoroutine(ringScript.DropRings());
 	}
	else
	{
	    StartCoroutine(Die());
	}
	yield return null;	
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.T) )
	    StartCoroutine(Die());
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Damage" && canTakeDamage == true)
        {
            StartCoroutine(TakeDamage());
        }
    }
}
