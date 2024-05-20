using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAction : MonoBehaviour
{
	EnigmaPhysics enigmaPhysics;
	public float airDashForce;
	public float homingForce;
	public float homingRange;

	IEnumerator HomingCheck(float range)
	{
		yield return null;
	}

	IEnumerator HomeIn(Transform target, float force)
	{
		yield return null;
	}

	IEnumerator AirDash(float force)
	{
		yield return null;
	}
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(enigmaPhysics.characterState == 2)
		{
			HomingCheck(homingRange);
		}
    }
}
