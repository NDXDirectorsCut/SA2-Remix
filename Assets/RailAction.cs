using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailAction : MonoBehaviour
{
    EnigmaPhysics enigmaPhysics;
	JumpAction jumpScript;
    Animator animator;
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
	[Header("Sounds")]
	public AudioClip landSound;
	public AudioClip grindSound;
	public AudioClip fastGrindSound;
	public float volume;
	AudioSource grindLoop;
	AudioSource fastLoop;

    // Start is called before the first frame update
    void Start()
    {
        enigmaPhysics = GetComponent<EnigmaPhysics>();
		animator = transform.root.GetComponentInChildren<Animator>();
		jumpScript = GetComponent<JumpAction>();
		
		grindLoop = animator.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		fastLoop = animator.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;	
		grindLoop.loop = true; fastLoop.loop = true;
		grindLoop.clip = grindSound; fastLoop.clip = fastGrindSound;
    }

	IEnumerator ReGrind()
	{
		canGrind = false;
		yield return new WaitForSeconds(.5f);
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
					Vector3 checkPosition = checkRail.transform.TransformPoint(checkSample.location) - checkRail.transform.TransformDirection(checkSample.up) *.5f;
					float checkDistance = Vector3.Distance(transform.position, checkPosition );
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
			//Debug.DrawRay(projectionSample.location)
			Vector3 projectionForward = rail.transform.TransformDirection(projectionSample.tangent);
			Vector3 projectionRight = Vector3.Cross(projectionForward,projectionSample.up);
			Vector3 projectionUp = Vector3.Cross(projectionRight,projectionForward);

			Vector3 railPosition = rail.transform.TransformPoint(projectionSample.location);// - projectionUp *.5f;//rail.transform.rotation * (rail.transform.position + projectionSample.location - projectionSample.up *.5f);
			float distance = Vector3.Distance(transform.position,railPosition);
			Debug.DrawRay(railPosition,projectionUp,Color.blue);
			if(attached == false && canGrind == true)
			{
				
				if(distance < 2/3f)
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
					//Vector3 projectionForward = rail.transform.TransformDirection(projectionSample.tangent);
					backwards = Vector3.Angle(enigmaPhysics.forwardReference,projectionForward) > 90 ? true : false;
					float axisAlign =  1-Vector3.Angle(backwards ? -projectionForward : projectionForward, enigmaPhysics.rBody.velocity)/90;
					Debug.Log(axisAlign);
					speed = backwards ? -enigmaPhysics.rBody.velocity.magnitude * axisAlign : enigmaPhysics.rBody.velocity.magnitude * axisAlign;
					attached = true;
				}
			}
			if(attached == true && rail != null)
			{
				animator.SetBool("Scripted Animation" , true);
				//sparkEffect.GetComponent<ParticleSystem>().Play();	

				if(canGrind == true)
				{
					AudioSource landSource = animator.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
					landSource.clip = landSound;
					landSource.volume = volume;
					landSource.Play();
					grindLoop.Play(); fastLoop.Play();
					Destroy(landSource,landSound.length+15);
				}
				canGrind = false;
				float absSpeed = Mathf.Abs(speed);

				//Debug.Log( Mathf.Clamp(speed/sparkSpeed,0,1) );
				grindLoop.volume = Mathf.Lerp(1 * volume * Mathf.Clamp(absSpeed/2,0,1),0, Mathf.Clamp((absSpeed/2f)/sparkSpeed,0,1) );
				fastLoop.volume = Mathf.Lerp(0,1 * volume, Mathf.Clamp((absSpeed/2f)/sparkSpeed,0,1));


				//Rail Fall
				if((posInRail > rail.Length || posInRail < 0) && rail.IsLoop == false)
				{
					attached = false;
					grindLoop.Stop(); fastLoop.Stop();
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

				Vector3 forwardVector = rail.transform.TransformDirection(railSample.tangent);
				Vector3 rightVector = Vector3.Cross(forwardVector,railSample.up);
				Vector3 normalVector = Vector3.Cross(rightVector,forwardVector);

				if(Input.GetButtonDown("Jump"))
				{
					enigmaPhysics.rBody.velocity = forwardVector * speed * enigmaPhysics.airSpeedPreservation;
					Vector3 jumpVector = Quaternion.AngleAxis(-xAxis*60f,forwardVector) * normalVector;
					StartCoroutine(jumpScript.Jump(jumpScript.initialJumpForce,jumpScript.jumpTimer,jumpScript.additiveJumpForce,jumpVector,true));
					StartCoroutine(ReGrind());
					//enigmaPhysics.rBody.velocity = enigmaPhysics.rBody.velocity.normalized * enigmaPhysics.rBody.velocity.magnitude * enigmaPhysics.airSpeedPreservation;
					attached = false;
					grindLoop.Stop(); fastLoop.Stop();
					enigmaPhysics.characterState = 2;
					rail = null;
					//transform.position += enigmaPhysics.normal.normalized * .75f;
					return;
				}

				//animator.CrossFadeInFixedTime("Grind L",.25f,0,0);

				

				Debug.DrawRay(rail.transform.position + railSample.location,normalVector,Color.green);

				enigmaPhysics.forwardReference = backwards ? -forwardVector : forwardVector;
				transform.rotation = Quaternion.LookRotation(forwardVector,normalVector);

				transform.position = rail.transform.TransformPoint(railSample.location);//rail.transform.position + railSample.location;// + normalVector*.5f;
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
				Vector3 pForward = Vector3.ProjectOnPlane(forwardVector,normalVector); Vector3 pForwardOld = Vector3.ProjectOnPlane(oldTangent,normalVector);

				float turnAngle = Vector3.SignedAngle(pForward,pForwardOld,normalVector) * 1/Time.fixedDeltaTime * 0.001f;
				oldTangent = forwardVector;
				//Debug.Log(turnAngle);
				balanceSway = Mathf.SmoothDamp(balanceSway,turnAngle * swaySensitivity ,ref curBalVelo,swayTime);

				float sway = backwards ? -(xAxis + balanceSway) : xAxis + balanceSway;
				sway = Mathf.Clamp(sway,-1,1);
				enigmaPhysics.normal = enigmaPhysics.referenceVector;

				Debug.DrawRay(transform.position,inputAxis*2,Color.red);
				if(sway > 0)
				{
					animator.Play("Grind R",0 , Mathf.Abs(sway));
				}
				if(sway < 0)
				{
					animator.Play("Grind L",0 , Mathf.Abs(sway));
				}

				float normalAngle = Vector3.SignedAngle( Vector3.ProjectOnPlane(normalVector,rightVector) , Vector3.ProjectOnPlane(enigmaPhysics.referenceVector,rightVector) , rightVector);
				speed += normalAngle * gravityForce * Time.deltaTime;
				speed -= Mathf.Sign(speed) * swayDeceleration * Mathf.Abs(sway) * Time.deltaTime; 
				posInRail += speed * Time.deltaTime;
		
				//if(enigmaPhysics.primaryAxis.magnitude > .1f)
				//posInRail = posInRail + inputAxis.z * 15 * Time.deltaTime;
				
			}
		}
    }
}
