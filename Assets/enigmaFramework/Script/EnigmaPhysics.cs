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

    [Header("Movement")]
        bool moved,movedStarted;
        [System.NonSerialized]
        public Vector3 primaryAxis;
        public float weight;
        [Header("Grounded")]
            public float startSpeed;
            public float speedCap;
            public AnimationCurve slopeIntensity;
            public float groundAcceleration;
            public float accelCap;
            public AnimationCurve accelerationCurve;
            public float groundDeceleration;
            public float turnRate;
            public float turnCoefficient;
            [Space(5)]
            public float brakeSpeed;

        [Header("Air")]
            public float airAcceleration;
            public float airBrakeSpeed;
            public float airDeceleration;
    [System.NonSerialized]
    public Vector3 forwardReference;
    [System.NonSerialized]
    public Vector3 slopeForce;
    Vector3 prevFloorPos;

    // Start is called before the first frame update
    void Awake()
    {
        normal = referenceVector;
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
        // /nnDebug.DrawRay(transform.position+transform.up*col.center.y*1.75f+transform.forward*.05f,-transform.up * col.center.y*2*raycastLength,Color.magenta);
        if(Physics.Raycast(rBody.position+transform.up*col.center.y*1.75f,-transform.up,out hit,col.center.y*2*activeRayLen,raycastLayers) != false)
        {

            if(interpolateNormals == true)
            {
                tempNormal = InterpolateNormal(hit).normalized;
            }
            else
            {
                tempNormal = hit.normal;
            }
            if(Vector3.Angle(normal,tempNormal) <= angleCutoff)
            {
                normal = Vector3.Lerp(normal,tempNormal,1 - Mathf.Pow(1 - normalLerp, dl * 60));
                Debug.DrawRay(hit.point,normal,Color.magenta);
            }

        }

        grounded = hit.transform != null ? true : false;

        switch(characterState)
        {
            case 0: // Debug
                rBody.isKinematic = true;
                float speed = Input.GetKey(KeyCode.LeftShift) ? 25 : 2.5f;
                transform.position += primaryAxis * Time.fixedDeltaTime *speed;
                //rBody.position += debugInput * dl;
                break;
            case 1: // Grounded
                if(grounded == false)
                {
                    characterState = 2;
                    goto case 2;
                }

                float slopeAngle = Vector3.SignedAngle(referenceVector,normal,-Vector3.Cross(forwardReference,normal));
                float slopeCap =  (Mathf.Abs(slopeAngle) * 0.106f + 2.3f);
                activeRayLen = Mathf.Lerp(activeRayLen,raycastLength,.1f * Time.deltaTime);

                //Debug.Log(slopeCap + " " + slopeAngle);
                float finalCap = accelCap;

                float accelRatio = Mathf.Clamp(Vector3.Angle(forwardReference,referenceVector)/90,0,1);
                //Debug.Log(accelRatio);
                //Debug.Log(slopeCap+accelCap);

                StartCoroutine(StartSpeed());
                moved = primaryAxis.sqrMagnitude != 0 ? true : false;
                movedStarted = primaryAxis.sqrMagnitude > .1f && rBody.velocity.sqrMagnitude < .1f ? true : false;

                point = Vector3.Lerp(point,hit.point,Mathf.Clamp(1 - Mathf.Pow(1 - pointLerp, dl * 60 ),0,1));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(point); localPoint.x = 0; localPoint.z = 0;
                point = rBody.transform.TransformPoint(localPoint);

                rBody.velocity += primaryAxis.normalized * groundAcceleration * accelRatio * Time.fixedDeltaTime * accelerationCurve.Evaluate(rBody.velocity.magnitude/finalCap);
                if(rBody.velocity.magnitude/finalCap > 1)
                {
                    rBody.velocity -= rBody.velocity * groundAcceleration * accelRatio * Time.fixedDeltaTime * .125f;
                }
                //Debug.DrawRay(transform.position,primaryAxis.normalized * groundAcceleration * accelRatio * Time.fixedDeltaTime * accelerationCurve.Evaluate(rBody.velocity.magnitude/finalCap),Color.green);

                float turnAngle = Vector3.SignedAngle(primaryAxis,rBody.velocity,normal);
                rBody.velocity = Quaternion.AngleAxis(-turnAngle * Time.fixedDeltaTime * turnRate,normal) * rBody.velocity;

                if(moved == false)
                {
                    rBody.velocity += rBody.velocity * groundDeceleration * Time.fixedDeltaTime * Mathf.Clamp(rBody.velocity.magnitude,0,1);
                    if(rBody.velocity.sqrMagnitude<1f)
                            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,0.3f*accelRatio);
                        if(rBody.velocity.sqrMagnitude<0.05f && Mathf.Abs(slopeAngle) < 45f)
                        {
                            rBody.velocity = Vector3.zero;
                        }
                }
                
		        slopeForce = -Vector3.ProjectOnPlane(referenceVector,normal).normalized * slopeIntensity.Evaluate(Mathf.Abs(slopeAngle)) * accelerationCurve.Evaluate(rBody.velocity.magnitude/(slopeCap*6)) * Time.fixedDeltaTime; 
                rBody.velocity += slopeForce;
                rBody.velocity = Vector3.ProjectOnPlane(rBody.velocity,normal);
                rBody.velocity = Vector3.ClampMagnitude(rBody.velocity,speedCap);
                //Debug.DrawRay(transform.position + -Vector3.Cross(forwardReference,normal) * .1f,-Vector3.ProjectOnPlane(referenceVector,normal).normalized * slopeIntensity.Evaluate(Mathf.Abs(slopeAngle)) * accelerationCurve.Evaluate(rBody.velocity.magnitude/(slopeCap*6)), Color.red);

                if(rBody.velocity.sqrMagnitude != 0)
                    forwardReference = rBody.velocity.normalized;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                rBody.position = point;
                rBody.transform.up = normal;

                if(Mathf.Abs(slopeAngle) > 90 && rBody.velocity.magnitude<5)
                {
                    grounded = false; characterState = 2;
                    activeRayLen = 0;
                }

                break;
            case 2: // Airborne
                if(grounded == true)
                {
                    characterState = 1;
                    goto case 1;
                }
                rBody.velocity += -referenceVector.normalized * weight * Time.fixedDeltaTime;
                rBody.velocity += primaryAxis * airAcceleration * Time.deltaTime;
                normal = Vector3.RotateTowards(normal,referenceVector,2f*Time.deltaTime,0).normalized;
                activeRayLen = Mathf.Lerp(activeRayLen,raycastLength, 2f * Time.deltaTime);
		    
                normalForward = Vector3.ProjectOnPlane(rBody.velocity,normal);

                if(rBody.velocity.sqrMagnitude != 0 && normalForward.sqrMagnitude != 0)
                    forwardReference = normalForward;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                rBody.transform.up = normal;
                point = transform.position;
                break;
            case 3: // Scripted

                break;
        }

        //Debug.DrawRay(point,normalForward,Color.blue);
        //Debug.DrawRay(point,rBody.velocity,Color.cyan);
        //Debug.DrawRay(point,normal,Color.green);
        //Debug.DrawRay(point,normalRight,Color.red);

    }

    IEnumerator StateTrigger(float oldState)
    {
        Debug.Log("Switched from Character State " + oldState + " to " + characterState  );
        switch(characterState)
        {
            case 1:
                if(oldState == 2)
                {
                    float hitAngle = Mathf.Clamp(Vector3.Angle(-rBody.velocity,normal)*1.1f,0,90);
                    prevFloorPos = hit.transform.position;
                    rBody.velocity = rBody.velocity.normalized * Mathf.Clamp(hitAngle/90,0,1) * rBody.velocity.magnitude;
                }

                break;
            case 2:
                if(oldState == 1)
                {

                    //physBody.velocity = physBody.velocity.normalized * physBody.velocity.magnitude * groundToAirTransition + floorVelocity;
                }
                break;
        }
        yield return null;
    }

    IEnumerator StartSpeed()
    {
        if(moved == false && primaryAxis.magnitude != 0 && rBody.velocity.magnitude < 0.2f)
        {
            Debug.Log("Start Speed!");
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
        //float maxVal = Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z);
        //scale = new Vector3(scale.x/maxVal,scale.y/maxVal,scale.z/maxVal);
        //scale = new Vector3(1/Mathf.Abs(scale.x),1/Mathf.Abs(scale.y),1/Mathf.Abs(scale.z)); //create vector to "correct" for scale
        //Vector3 scaleFull = new Vector3(1/scale.x,1/scale.y,1/scale.z);
        //This is only a bandaid solution and gets less accurate the more you skew a mesh, if you know how to get the actual normals after scaling please contact me at
        // n.dx on Discord or NDXDirectorsCut on Twitter

        //Debug.Log(scale);

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[iHit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[iHit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[iHit.triangleIndex * 3 + 2]];

        Vector3 scaleAlt = new Vector3(1/scale.x,1/scale.y,1/scale.z);
        //Debug.Log(triangles[iHit.triangleIndex * 3 + 0]);
        //Debug.DrawRay(Vector3.Scale(mesh.vertices[triangles[iHit.triangleIndex * 3 + 0]],iHit.transform.lossyScale) + iHit.transform.position,Vector3.Scale(n0.normalized,scaleAlt).normalized,Color.red);
        //Debug.DrawRay(Vector3.Scale(mesh.vertices[triangles[iHit.triangleIndex * 3 + 1]],iHit.transform.lossyScale) + iHit.transform.position,Vector3.Scale(n0.normalized,scaleAlt).normalized,Color.green);
        //Debug.DrawRay(Vector3.Scale(mesh.vertices[triangles[iHit.triangleIndex * 3 + 2]],iHit.transform.lossyScale) + iHit.transform.position,Vector3.Scale(n0.normalized,scaleAlt).normalized,Color.blue);

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = iHit.barycentricCoordinate;

        n0 = Vector3.Scale(n0,scaleAlt);
        n1 = Vector3.Scale(n1,scaleAlt);
        n2 = Vector3.Scale(n2,scaleAlt);

        //Debug.DrawRay(iHit.point,n0,Color.red);
        //Debug.DrawRay(iHit.point,n1,Color.green);
        //Debug.DrawRay(iHit.point,n2,Color.blue);

        //baryCenter = Vector3.Scale(baryCenter,scaleAlt);
        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        //interpolatedNormal = Vector3.Scale(interpolatedNormal,scale);

        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;


        // Transform local space normals to world space
        Transform hitTransform = iHit.collider.transform;

        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }
}
