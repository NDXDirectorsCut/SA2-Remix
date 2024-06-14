using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAction : MonoBehaviour
{
    public int ringCount;
    public int maxRingDrop = 20;
    public float ringLife;
    public bool loseAllRings;
    public GameObject dropObject;
    public float dropForce;

    public IEnumerator ReactivateRing(RingObject ringObj);
    {
	ringObj.canPickup = false;
	yield return new WaitForSeconds(.5f);
	ringObj.canPickup = true;
    }

    public IEnumerator DropRings()
    {
	int clampedRing = Mathf.Clamp(ringCount,0,maxRingDrop);
	ringCount = loseAllRings ? 0 : ringCount - clampedRing;
	for(int i = 0; i < clampedRing; i++)
	{
	    GameObject dropRing = Instantiate(dropObject,transform.position,Quaternion.identity);
	    dropRing.GetComponentInChildren<Rigidbody>().velocity = Quaternion.AngleAxis(360*i/clampedRing,transform.up) * transform.forward *dropForce; 
	    ReactivateRing(dropRing.GetComponentInChildren<RingObject>());
	}
	ringCount = Mathf.Clamp(ringCount,0,ringCount);
	yield return null;
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
