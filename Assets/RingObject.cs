using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject : MonoBehaviour
{
    public int ringValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
	if(col.GetComponent<RingAction>() != null)
	{
	    RingAction ringScript = col.GetComponent<RingAction>();
	    ringScript.ringCount += ringValue;
	    Destroy(gameObject);
	    
	}
    }
}
