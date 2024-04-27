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
        public LayerMask raycastLayers;
        public RaycastHit hit;
        public bool interpolateNormals;
        [Range(0,90)]
        public float angleCutoff;
        public bool grounded;
        
        public int characterState = 0;
    
    [Header("Movement")]
        public Vector3 primaryAxis;
        bool moved,movedStarted;
        public float gravitationalPull;
        [Header("Grounded")]
            public float groundStartAcceleration;
            public float accelerationOffset;
            public AnimationCurve accelerationCurve;
            public float groundDeceleration;
            public float turnRate;
            public float turnCoefficient;
            float accelCap;
            [Space(5)]
            public float brakeSpeed;

        [Header("Air")] 
            public float airAcceleration;
            public float airBrakeSpeed;
            public float airDeceleration;
    [System.NonSerialized]
    public Vector3 forwardReference;    

    // Start is called before the first frame update
    void Start()
    {
        normal = referenceVector;
        normalForward = transform.forward;
        normalRight = Vector3.Cross(normal,normalForward);
        point = transform.position;
        forwardReference = transform.forward;

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
        if(Physics.Raycast(rBody.position+transform.up*col.center.y*1.75f,-transform.up,out hit,col.center.y*2*raycastLength,raycastLayers) != false)
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

                accelCap = groundStartAcceleration * 29.33f + accelerationOffset;
                

                point = Vector3.Lerp(point,hit.point,Mathf.Clamp(1 - Mathf.Pow(1 - pointLerp, dl * 60 ),0,1));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(point); localPoint.x = 0; localPoint.z = 0;
                point = rBody.transform.TransformPoint(localPoint);

                rBody.velocity += primaryAxis * Time.deltaTime * groundStartAcceleration * 36 * (1-(rBody.velocity.magnitude/accelCap));

                moved = primaryAxis.sqrMagnitude != 0 ? true : false;
                movedStarted = primaryAxis.sqrMagnitude > .1f && rBody.velocity.sqrMagnitude < .1f ? true : false; 

                normalRight = Vector3.Cross(normal,rBody.velocity.normalized).normalized;
                normalForward = Vector3.Cross(normalRight,normal).normalized;

                rBody.velocity = normalForward * rBody.velocity.magnitude;
                float turnAngle = Vector3.SignedAngle(primaryAxis,rBody.velocity,normal);
                rBody.velocity = Quaternion.AngleAxis(-turnAngle * Time.fixedDeltaTime * turnRate,normal) * rBody.velocity;

                rBody.position = point;
                rBody.transform.up = normal;

                if(moved == false)
                {
                    rBody.velocity += rBody.velocity * groundDeceleration * 6 *Time.fixedDeltaTime * Mathf.Clamp(rBody.velocity.magnitude,0,1);
                    if(rBody.velocity.sqrMagnitude<1f)
                            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,0.3f);
                        if(rBody.velocity.sqrMagnitude<0.05f)
                        {
                            rBody.velocity = Vector3.zero;
                        }
                }

                if(rBody.velocity.sqrMagnitude != 0)
                    forwardReference = normalForward;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                /*
                accelCap = groundStartAcceleration * 29.33f + accelerationOffset;
                
                point = Vector3.Lerp(point,hit.point,Mathf.Clamp(1 - Mathf.Pow(1 - pointLerp, dl * 60 ),0,1));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(point); localPoint.x = 0; localPoint.z = 0;
                point = rBody.transform.TransformPoint(localPoint);
                //Debug.DrawRay(point,tempNormal,Color.gray);

                moved = primaryAxis.sqrMagnitude != 0 ? true : false;
                movedStarted = primaryAxis.sqrMagnitude > .1f && rBody.velocity.sqrMagnitude < .1f ? true : false; 
                accelCap = groundStartAcceleration * 29.33f + accelerationOffset;
                //Debug.Log(accelCap);

                normalRight = Vector3.Cross(normal,rBody.velocity.normalized).normalized;
                normalForward = Vector3.Cross(normalRight,normal).normalized;

                rBody.velocity = normalForward * rBody.velocity.magnitude;

                if(rBody.velocity.sqrMagnitude != 0)
                    forwardReference = normalForward;
                else
                    forwardReference = Vector3.ProjectOnPlane(forwardReference,normal);

                float turnAngle = Vector3.SignedAngle(primaryAxis,rBody.velocity,normal);
                rBody.velocity = Quaternion.AngleAxis(-turnAngle * Time.fixedDeltaTime * turnRate,normal) * rBody.velocity;

                rBody.velocity += primaryAxis * Time.fixedDeltaTime * groundStartAcceleration * 36 * (1-(rBody.velocity.magnitude/accelCap));//accelerationCurve.Evaluate();
                //Debug.Log(primaryAxis * Time.fixedDeltaTime * groundStartAcceleration * accelerationCurve.Evaluate(rBody.velocity.magnitude/accelCap));
                if(moved == false)
                {
                    rBody.velocity += rBody.velocity * groundDeceleration * 6 *Time.fixedDeltaTime * Mathf.Clamp(rBody.velocity.magnitude,0,1);
                    if(rBody.velocity.sqrMagnitude<1f)
                            rBody.velocity = Vector3.Lerp(rBody.velocity,Vector3.zero,0.3f);
                        if(rBody.velocity.sqrMagnitude<0.05f)
                        {
                            rBody.velocity = Vector3.zero;
                        }
                }

                rBody.position = point;
                rBody.rotation = Quaternion.LookRotation(forwardReference,normal);
                //transform.uo = normal;
                //rBody.transform.up = normal;*/

                break;
            case 2: // Airborne
                if(grounded == true)
                {
                    characterState = 1;
                    goto case 1;
                }
                rBody.velocity += -referenceVector * gravitationalPull * Time.deltaTime;
                normal = Vector3.RotateTowards(normal,referenceVector,1.25f*Time.deltaTime,0).normalized;

                normalRight = Vector3.Cross(normal,rBody.velocity.normalized).normalized;
                normalForward = Vector3.Cross(normalRight,normal).normalized;

                if(rBody.velocity.sqrMagnitude != 0)
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

    Vector3 InterpolateNormal(RaycastHit iHit)
    {

        MeshCollider meshCollider = iHit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            Debug.Log("Abort Mission, using hit.normal instead");
            return iHit.normal;
            
        }

        Mesh mesh = meshCollider.sharedMesh;

        if(mesh.isReadable == false)
        {
            Debug.Log("Abort Mission, using hit.normal instead");
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
