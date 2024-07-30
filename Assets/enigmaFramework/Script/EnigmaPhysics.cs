using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaPhysics : MonoBehaviour
{
    [System.NonSerialized]
    public Rigidbody rBody;
    [System.NonSerialized]
    public CapsuleCollider col;

    [Header("Physics")]
        public Vector3 referenceVector;
        public Vector3 normal;
        [Range(0,1)]
        public float normalLerp;
        public Vector3 point;
        [Range(0,1)]
        public float pointLerp;

        public Vector3 normalForward {get; set;}
        public Vector3 normalRight {get; set;}
        [Range(0,2)]
        public float raycastLength;
        [System.NonSerialized]
        public float activeRayLen;
        public LayerMask raycastLayers;
        public RaycastHit hit;
        public bool interpolateNormals;
        [Range(0,90)]
        public float angleCutoff;
        public bool grounded;

        public int characterState = 0;
    	int characterState_copy = 0;

    [Header("Movement")]
        bool moved,movedStarted;
        [System.NonSerialized]
        public Vector3 primaryAxis;
        public float weight;
	public bool canTriggerAction;
        [Header("Grounded")]
            public float startSpeed;
            public float speedCap;
            public AnimationCurve slopeIntensity;
            public float groundAcceleration;
            public float accelCap;
            public AnimationCurve accelerationCurve;
            public float groundDeceleration;
            public float turnRate;
            public float turnDeceleration;
            [Space(5)]
            public float brakeSpeed;

        [Header("Air")]
            public float airAcceleration;
            public float airTurnSpeed;
            public float airDeceleration;
            public float airTurnDeceleration;
	    public float airSpeedPreservation;
    [System.NonSerialized]
    public Vector3 forwardReference;
    [System.NonSerialized]
    public Vector3 slopeForce;
    [System.NonSerialized]
    public Vector3 linearSlopeForce;
    Vector3 prevFloorPos;

    // Start is called before the first frame update
    void Awake()
    {
        normal = referenceVector; // set default normal to be the base up direction
        normalForward = transform.forward; 
        normalRight = Vector3.Cross(normal,normalForward);
        point = transform.position;
        forwardReference = transform.forward;
        activeRayLen = raycastLength;
        if(GetComponent<Rigidbody>() != null)
            rBody = GetComponent<Rigidbody>();
        if(GetComponent<CapsuleCollider>() != null)
            col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 tempNormal = referenceVector;
        float dl = Time.fixedDeltaTime;
        // Raycast to get surface information
        if(Physics.Raycast(rBody.position+transform.up*col.center.y*1.75f,-transform.up,out hit,col.center.y*2*activeRayLen,raycastLayers) != false)
        {
            if(interpolateNormals == true)
            {
                tempNormal = InterpolateNormal(hit).normalized; // use barycentric coordinates to make a smoothed normal
            }
            else
            {
                tempNormal = hit.normal; // use default raycast normal
            }
            if(Vector3.Angle(normal,tempNormal) <= angleCutoff)
            {
                grounded = hit.transform != null ? true : false; // determine whether the player is on the ground or not
                normal = Vector3.Lerp(normal,tempNormal,1 - Mathf.Pow(1 - normalLerp, dl * 60)); // interpolate the normal
                Debug.DrawRay(hit.point,normal,Color.magenta);
            }
            else
            {
                grounded = false;
            }

        }

        grounded = hit.transform != null ? grounded : false;

        //handle state transitions
        if(characterState != characterState_copy)
        {
            StartCoroutine(StateTrigger(characterState_copy));
            characterState_copy = characterState;
        }

        //state machine
        switch(characterState)
        {
            case 0: // Debug
                rBody.isKinematic = true;
                float speed = Input.GetKey(KeyCode.LeftShift) ? 25 : 2.5f;
                transform.position += primaryAxis * Time.fixedDeltaTime *speed;
                break;
            case 1: // Grounded

                // switch to next state
                if(grounded == false)
                {
                    characterState = 2;
                    goto case 2;
                }

                // get slope angle
                float slopeAngle = Vector3.SignedAngle(referenceVector,normal,-Vector3.Cross(forwardReference,normal)); 
                float slopeCap = (Mathf.Abs(slopeAngle) * 0.106f + 2.3f); // Max speed gravity can affect the player at
                activeRayLen = Mathf.Lerp(activeRayLen,raycastLength,.1f * Time.deltaTime); // return raycast length to normal

                float finalCap = accelCap;
                float accelRatio = Mathf.Clamp(Vector3.Angle(forwardReference,referenceVector)/90,0,1);

                // begin the player with some speed rather than 0
                StartCoroutine(StartSpeed());
                moved = primaryAxis.sqrMagnitude != 0 ? true : false;
                movedStarted = primaryAxis.sqrMagnitude > .1f && rBody.velocity.sqrMagnitude < .1f ? true : false;

                // determine the position the player should be at
                point = Vector3.Lerp(point,hit.point,Mathf.Clamp(1 - Mathf.Pow(1 - pointLerp, dl * 60 ),0,1));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(point); localPoint.x = 0; localPoint.z = 0;
                point = rBody.transform.TransformPoint(localPoint);

                // accelerate the player if they push the stick more than halfway 
                rBody.velocity += primaryAxis.normalized * groundAcceleration * accelRatio * Time.fixedDeltaTime * accelerationCurve.Evaluate(rBody.velocity.magnitude/finalCap);
                
                // decelerate the player when they go over the max accel speed
                if(rBody.velocity.magnitude/finalCap > 1)
                {
                    rBody.velocity -= rBody.velocity * groundAcceleration * accelRatio * Time.fixedDeltaTime * .125f;
                }

                // turn handling
                float turnAngle = Vector3.SignedAngle(primaryAxis,rBody.velocity,normal);
                float finalTurnAngle = -turnAngle * Time.fixedDeltaTime * turnRate;
                finalTurnAngle = Mathf.Abs(finalTurnAngle) > Mathf.Abs(turnAngle) ? turnAngle : finalTurnAngle;

                rBody.velocity = Quaternion.AngleAxis(finalTurnAngle, normal) * rBody.velocity;
                rBody.velocity -= rBody.velocity * turnDeceleration * Mathf.Abs(turnAngle/360) * Time.fixedDeltaTime; // decelerate the player when turning quickly

                // decelerate the player when not moving
                if(moved == false)
                {
                    rBody.velocity += rBody.velocity * groundDeceleration * Time.fixedDeltaTime * Mathf.Clamp(rBody.velocity.magnitude,0,1);
                    if(rBody.velocity.sqrMagnitude<.25f)
                            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,0.3f*accelRatio);
                        if(rBody.velocity.sqrMagnitude<0.05f && Mathf.Abs(slopeAngle) < 45f)
                        {
                            rBody.velocity = Vector3.zero;
                        }
                }
                
                // push the player down based on the slope
		        slopeForce = -Vector3.ProjectOnPlane(referenceVector,normal).normalized * slopeIntensity.Evaluate(Mathf.Abs(slopeAngle)) * accelerationCurve.Evaluate(rBody.velocity.magnitude/(slopeCap*6)) * Time.fixedDeltaTime; 
                linearSlopeForce = -Vector3.ProjectOnPlane(referenceVector,normal).normalized * accelerationCurve.Evaluate(rBody.velocity.magnitude/(slopeCap*6)) * Time.fixedDeltaTime; 
                rBody.velocity += slopeForce;

                // keep the player velocity perpendicular to the normal
                rBody.velocity = Vector3.ProjectOnPlane(rBody.velocity,normal);
                rBody.velocity = Vector3.ClampMagnitude(rBody.velocity,speedCap);

                // set the forward direction if above a minimum speed
                if(rBody.velocity.sqrMagnitude > .2f)
                    forwardReference = rBody.velocity.normalized;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                // correct the character controller position and rotation
                rBody.position = point;
                rBody.transform.up = normal;

                // kick the player off the ground when they're not moving fast enough
                if(Mathf.Abs(slopeAngle) > 85 && rBody.velocity.magnitude<5)
                {
                    grounded = false; characterState = 2;
                    activeRayLen = 0;
                }

                break;
            case 2: // Airborne

                // switch to next state
                if(grounded == true)
                {
                    characterState = 1;
                    goto case 1;
                }

                Vector3 projectedVelo = Vector3.ProjectOnPlane(rBody.velocity,normal);
                Vector3 correctedAxis = Quaternion.FromToRotation(normal,referenceVector) * primaryAxis;

                // apply gravity
                rBody.velocity += -referenceVector.normalized * weight * Time.fixedDeltaTime;
                rBody.velocity += correctedAxis * airAcceleration * Mathf.Clamp((rBody.velocity.magnitude/airAcceleration),0,1) * Time.deltaTime;
                
                // turn logic
                float brakeAngle = Vector3.SignedAngle(correctedAxis,projectedVelo,normal);
                float finalBrakeAngle = -brakeAngle * Time.fixedDeltaTime * airTurnSpeed;
                finalBrakeAngle = Mathf.Abs(finalBrakeAngle) > Mathf.Abs(brakeAngle) ? brakeAngle : finalBrakeAngle;

                rBody.velocity = Quaternion.AngleAxis( finalBrakeAngle ,referenceVector) * rBody.velocity;
                rBody.velocity -= projectedVelo * airTurnDeceleration * Mathf.Abs(brakeAngle/360) * Time.fixedDeltaTime; // decelerate the player when turning quickly

                // decelerate the player when not actively moving
                moved = primaryAxis.sqrMagnitude != 0 ? true : false;
                if(moved == false)
                {
                    rBody.velocity += projectedVelo * airDeceleration * Time.fixedDeltaTime * Mathf.Clamp(projectedVelo.magnitude,0,1);
                }

                // transition normal to up direction
                normal = Vector3.RotateTowards(normal,referenceVector,3f*Time.deltaTime,0).normalized;

                // rotate the player around their center
                Vector3 pos = transform.position + transform.up * .5f;
                transform.position = (Quaternion.FromToRotation(transform.up,normal) * (transform.position-pos)) + pos;
                transform.up = Quaternion.FromToRotation(transform.up,normal) * transform.up;

                // correct ray length
                activeRayLen = Mathf.Lerp(activeRayLen,raycastLength, 4f * Time.deltaTime);
		    
                //determine forward direction perpendicular to the up direction
                normalForward = Vector3.ProjectOnPlane(rBody.velocity,normal);

                if(normalForward.sqrMagnitude > 0.2f)
                    forwardReference = normalForward;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                point = transform.position;
                break;
            case 3: // Scripted
                // state behaviour is determined by an external source
                break;
        }
    }

    IEnumerator StateTrigger(float oldState)
    {
        switch(characterState)
        {
            case 1:
                if(oldState == 2)
                {
                    // speed preservation handling
                    float hitAngle = Mathf.Clamp(Vector3.Angle(-rBody.velocity,normal)*1.1f,0,90);
                    rBody.velocity = rBody.velocity.normalized * Mathf.Clamp(hitAngle/90,0,1) * rBody.velocity.magnitude;
                }

                break;
            case 2:
                if(oldState == 1)
                {
                    // remove some of the player's speed when becoming airborne
                    rBody.velocity = rBody.velocity.normalized * rBody.velocity.magnitude * airSpeedPreservation;// + floorVelocity;
                }
                break;
        }
        yield return null;
    }

    IEnumerator StartSpeed()
    {
        if(moved == false && primaryAxis.magnitude != 0 && rBody.velocity.magnitude < 0.2f)
        {
            rBody.velocity = primaryAxis.normalized * startSpeed;
        }
        yield return null;
    }

    Vector3 InterpolateNormal(RaycastHit iHit)
    {

        MeshCollider meshCollider = iHit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return iHit.normal;

        }

        Mesh mesh = meshCollider.sharedMesh;

        if(mesh.isReadable == false)
        {
            return iHit.normal;
        }

        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        Vector3 scale = iHit.transform.lossyScale;

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[iHit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[iHit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[iHit.triangleIndex * 3 + 2]];

        Vector3 scaleAlt = new Vector3(1/scale.x,1/scale.y,1/scale.z);
        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = iHit.barycentricCoordinate;

        n0 = Vector3.Scale(n0,scaleAlt);
        n1 = Vector3.Scale(n1,scaleAlt);
        n2 = Vector3.Scale(n2,scaleAlt);

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;

        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;


        // Transform local space normals to world space
        Transform hitTransform = iHit.collider.transform;

        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }
}
