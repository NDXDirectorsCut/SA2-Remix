using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public Animator animator;
    public Spline rail;
    public bool attached;
    float posInRail = 0;	
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	if(rail != null)
	{
		CurveSample projectionSample = rail.GetProjectionSample(rail.transform.InverseTransformPoint(transform.position));
		float distance = Vector3.Distance(transform.position,rail.transform.position + projectionSample.location);
		if(attached == false && distance < 1)
		{
			
            		float sumLength = 0;
           		for(int i=0; i<rail.curves.Count; i++)
           		{
         		       if(rail.curves[i] == projectionSample.curve)
          		       {
            		    		break;
            		       }
               			else
               			{
                    			sumLength += rail.curves[i].Length;
             			}
           		}
           		sumLength += projectionSample.distanceInCurve;
			posInRail = sumLength;
			attached = true;
		}
		if(attached == true)
		{
			enigmaPhysics.characterState = 3;
			enigmaPhysics.rBody.velocity = Vector3.zero;
       			CurveSample railSample = rail.GetSampleAtDistance(posInRail);
			animator.CrossFadeInFixedTime("Grind L",.25f,0,0);
			Vector3 rightVector = Vector3.Cross(railSample.tangent,railSample.up);
			Vector3 normalVector = Vector3.Cross(rightVector,railSample.tangent);
			Debug.DrawRay(rail.transform.position + railSample.location,normalVector,Color.green);
			enigmaPhysics.forwardReference = railSample.tangent;
			transform.up = normalVector;
			enigmaPhysics.normal = normalVector;
			transform.position = rail.transform.position + railSample.location;// + normalVector*.5f;
			if(enigmaPhysics.primaryAxis.magnitude > .1f)
				posInRail = Input.GetKey(KeyCode.LeftShift) ? posInRail - 15 * Time.deltaTime : posInRail + 15 * Time.deltaTime;
			
		}
	}
    }
}
