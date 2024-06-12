using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAction : MonoBehaviour
{
    public int ringCount;
    public int maxRingDrop = 20;

    

    IEnumerator RingHit()
    {
	int clampedRing = Mathf.Clamp(ringCount,0,maxRingDrop);
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
