using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackAction : MonoBehaviour
{
	EnigmaPhysics enigmaPhysics;

	IEnumerator HomingAttack(float range)
	{
	
	}

	IEnumerator HomeIn(Transform target, float force)
	{
		
	}

	IEnumerator AirDash(float force)
	{
		
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
		
	  }
    }
}
