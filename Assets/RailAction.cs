using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
    public Spline rail;
    public 	bool attached;
    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
	if(rail != null)
	{
		float posInRail = 0;	
		if(attached == false)
		{
			/*CurveSample projectionSample = rail.GetProjectionSample(rail.transform.InverseTransformPoint(transform.position));
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
			attached = true;*/
		}
		if(attached == true)
		{
			enigmaPhysics.characterState = 3;
       			CurveSample railSample = rail.GetSampleAtDistance(posInRail);
			//transform.position = rail.transform.InverseTransformPoint(railSample.location);
			Debug.DrawRay(rail.transform.position + railSample.location,railSample.tangent,Color.blue);
			Debug.DrawRay(rail.transform.position + railSample.location,railSample.up,Color.green);
			Debug.DrawRay(rail.transform.position + railSample.location,Vector3.Cross(railSample.tangent,railSample.up),Color.red);
			enigmaPhysics.forwardReference = railSample.tangent;
			posInRail += .25f * Time.deltaTime;
			
		}
	}
    }
}
