using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
	JumpAction jumpScript;
    public Animator animator;
    public GameObject sparkEffect;
    public float sparkSpeed;
    public Spline rail;
    public bool attached;
    float posInRail = 0;	
	Vector3 inputAxis;
	//[Range(-1,1)]
	float xAxis;
	float balanceSway;
	[Range(0,3)]
	public float turnTime;
	[Range(0,3)]
	public float swayTime;
	public float swaySensitivity;
	public float swayDeceleration;
	float curVelo;
	float curBalVelo;
	public float gravityForce;
	float speed;
	bool backwards;
	bool canGrind = true;
	Vector3 oldTangent;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
		jumpScript = GetComponent<JumpAction>();
    }

	IEnumerator ReGrind()
	{
		canGrind = false;
		yield return new WaitForSeconds(.25f);
		canGrind = true;
	}
    // Update is called once per frame
    void FixedUpdate()
    {
		float minDistance = 9999;
		if(attached == false)
		{
			sparkEffect.GetComponent<ParticleSystem>().Stop();
			GameObject[] rails = GameObject.FindGameObjectsWithTag("Rail");
			foreach (GameObject checkObject in rails)
			{
				if(checkObject.GetComponent<Spline>() != null)
				{
					Spline checkRail = checkObject.GetComponent<Spline>();
					CurveSample checkSample = checkRail.GetProjectionSample(checkRail.transform.InverseTransformPoint(transform.position));
					float checkDistance = Vector3.Distance(transform.position,checkRail.transform.position + checkSample.location);
					if(checkDistance < minDistance)
					{
						rail = checkRail;
						minDistance = checkDistance;
					}
				}
			}
		}

		if(rail != null)
		{
			CurveSample projectionSample = rail.GetProjectionSample(rail.transform.InverseTransformPoint(transform.position));
			float distance = Vector3.Distance(transform.position,rail.transform.position + projectionSample.location - projectionSample.up *.5f);
			if(attached == false && canGrind == true)
			{
				
				if(distance < 1f)
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
					backwards = Vector3.Angle(enigmaPhysics.forwardReference,projectionSample.tangent) > 90 ? true : false;
					float axisAlign =  1-Vector3.Angle(backwards ? -projectionSample.tangent : projectionSample.tangent, enigmaPhysics.rBody.velocity)/90;
					Debug.Log(axisAlign);
					speed = backwards ? -enigmaPhysics.rBody.velocity.magnitude * axisAlign : enigmaPhysics.rBody.velocity.magnitude * axisAlign;
					attached = true;
				}
			}
			if(attached == true && rail != null)
			{
				animator.SetBool("Scripted Animation" , true);
				//sparkEffect.GetComponent<ParticleSystem>().Play();	
				canGrind = false;
				//Rail Fall
				if((posInRail > rail.Length || posInRail < 0) && rail.IsLoop == false)
				{
					attached = false;
					enigmaPhysics.characterState = 2;
					
					if(posInRail > rail.Length)
					{
						enigmaPhysics.rBody.velocity = backwards ? -enigmaPhysics.forwardReference * speed : enigmaPhysics.forwardReference * speed;
						//enigmaPhysics.rBody.position += backwards ? -enigmaPhysics.forwardReference * 1.5f : enigmaPhysics.forwardReference * 1.5f;
					}
					else
					{
						enigmaPhysics.rBody.velocity = backwards ? -enigmaPhysics.forwardReference * speed : enigmaPhysics.forwardReference * speed;
						//enigmaPhysics.rBody.position += backwards ? -	enigmaPhysics.forwardReference * 1.5f : enigmaPhysics.forwardReference * 1.5f;
					}
					StartCoroutine(ReGrind());
					animator.SetBool("Scripted Animation" , false);
					rail = null;
					Debug.Log("GET OFFFFFFFFFFFFFFF");
					return;
				}

				enigmaPhysics.characterState = 3;
				enigmaPhysics.rBody.velocity = Vector3.zero;
				CurveSample railSample = rail.GetSampleAtDistance(posInRail);

				inputAxis =	transform.InverseTransformDirection(enigmaPhysics.primaryAxis);
				xAxis = Mathf.SmoothDamp(xAxis,inputAxis.x,ref curVelo,turnTime);

				Vector3 rightVector = Vector3.Cross(railSample.tangent,railSample.up);
				Vector3 normalVector = Vector3.Cross(rightVector,railSample.tangent);

				if(Input.GetButtonDown("Jump"))
				{
					enigmaPhysics.rBody.velocity = railSample.tangent * speed * enigmaPhysics.airSpeedPreservation;
					Vector3 jumpVector = Quaternion.AngleAxis(-xAxis*60f,railSample.tangent) * normalVector;
					StartCoroutine(jumpScript.Jump(jumpScript.initialJumpForce,jumpScript.jumpTimer,jumpScript.additiveJumpForce,jumpVector));
					StartCoroutine(ReGrind());
					//enigmaPhysics.rBody.velocity = enigmaPhysics.rBody.velocity.normalized * enigmaPhysics.rBody.velocity.magnitude * enigmaPhysics.airSpeedPreservation;
					attached = false;
					enigmaPhysics.characterState = 2;
					rail = null;
					//transform.position += enigmaPhysics.normal.normalized * .75f;
					return;
				}

				//animator.CrossFadeInFixedTime("Grind L",.25f,0,0);

				

				Debug.DrawRay(rail.transform.position + railSample.location,normalVector,Color.green);

				enigmaPhysics.forwardReference = backwards ? -railSample.tangent : railSample.tangent;
				transform.rotation = Quaternion.LookRotation(railSample.tangent,normalVector);

				transform.position = rail.transform.position + railSample.location;// + normalVector*.5f;
				//Effect
				if(Mathf.Abs(speed) > sparkSpeed && sparkEffect.GetComponent<ParticleSystem>().isPlaying == false)
				{
					Debug.Log("Play Effect");
					sparkEffect.GetComponent<ParticleSystem>().Play();
				}
				if(Mathf.Abs(speed) < sparkSpeed)
				{
					sparkEffect.GetComponent<ParticleSystem>().Stop();	
				}		
				//Physics
				float turnAngle = Vector3.SignedAngle(railSample.tangent,oldTangent,normalVector) * 1/Time.fixedDeltaTime * 0.001f;
				oldTangent = railSample.tangent;
				//Debug.Log(turnAngle);
				balanceSway = Mathf.SmoothDamp(balanceSway,turnAngle * swaySensitivity ,ref curBalVelo,swayTime);

				float sway = backwards ? -(xAxis + balanceSway) : xAxis + balanceSway;
				sway = Mathf.Clamp(sway,-1,1);
				//enigmaPhysics.normal = Quaternion.AngleAxis(sway*75f, railSample.tangent) * normalVector;

				Debug.DrawRay(transform.position,inputAxis*2,Color.red);
				if(sway > 0)
				{
					animator.Play("Grind R",0 , Mathf.Abs(sway));
				}
				if(sway < 0)
				{
					animator.Play("Grind L",0 , Mathf.Abs(sway));
				}

				float normalAngle = Vector3.SignedAngle(normalVector,enigmaPhysics.referenceVector,rightVector);
				speed += normalAngle * gravityForce * Time.deltaTime;
				speed -= Mathf.Sign(speed) * swayDeceleration * Mathf.Abs(sway) * Time.deltaTime; 
				posInRail += speed * Time.deltaTime;
		
				//if(enigmaPhysics.primaryAxis.magnitude > .1f)
				//posInRail = posInRail + inputAxis.z * 15 * Time.deltaTime;
				
			}
		}
    }
}
