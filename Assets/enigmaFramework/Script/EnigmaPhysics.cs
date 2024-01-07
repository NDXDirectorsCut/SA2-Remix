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

        Vector3 normalForward;
        Vector3 normalRight;
        [Range(0,1)]
        public float raycastLength;
        public LayerMask raycastLayers;
        RaycastHit hit;
        public bool interpolateNormals;
        [Range(0,90)]
        public float angleCutoff;
        public bool grounded;
        
        public int characterState = 0;
    
    [Header("Movement")]
        public float gravitationalPull;
        [Header("Grounded")]
            public float groundStartAcceleration;
            public AnimationCurve accelerationCurve;
            public float groundDeceleration;
            [Space(5)]
            public float brakeSpeed;

        [Header("Air")] 
            public float airAcceleration;
            public float airBrakeSpeed;
            public float airDeceleration;

    

    // Start is called before the first frame update
    void Start()
    {
        normal = referenceVector;
        normalForward = transform.forward;
        normalRight = Vector3.Cross(normal,normalForward);
        point = transform.position;

        if(GetComponent<Rigidbody>() != null)
            rBody = GetComponent<Rigidbody>();
        if(GetComponent<CapsuleCollider>() != null)
            col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        float dl = Time.fixedDeltaTime;
        Debug.DrawRay(transform.position+transform.up*col.center.y*1/3,-transform.up * col.height*raycastLength,Color.magenta);
        if(Physics.Raycast(transform.position+transform.up*col.center.y*1/3,-transform.up,out hit,col.height*raycastLength,raycastLayers) != false)
        {
            Vector3 tempNormal;
            if(interpolateNormals == true)
            {
                tempNormal = InterpolateNormal(hit).normalized;
            }
            else
            {
                tempNormal = hit.normal;
            }
            Debug.DrawRay(hit.point,tempNormal,Color.gray);
            if(Vector3.Angle(normal,tempNormal) <= angleCutoff)
            {
                normal = Vector3.Lerp(normal,tempNormal,1 - Mathf.Pow(1 - normalLerp, dl * 60));
            }
        }        


        switch(characterState)
        {
            case 0: // Debug
                //rBody.position += debugInput * dl;
                break;
            case 1: // Grounded
                if(grounded == false)
                {
                    characterState = 2;
                    goto case 2;
                }
                
                point = Vector3.Lerp(point,hit.point,1 - Mathf.Pow(1 - pointLerp, dl * 60 ));
                Vector3 localPoint = rBody.transform.InverseTransformPoint(point); localPoint.x = 0; localPoint.z = 0;
                point = rBody.transform.TransformPoint(localPoint);

                normalRight = Vector3.Cross(normal,rBody.velocity.normalized).normalized;
                normalForward = Vector3.Cross(normal,normalRight).normalized;

                rBody.position = point;
                rBody.transform.up = normal;

                break;
            case 2: // Airborne
                if(grounded == true)
                {
                    characterState = 1;
                    goto case 1;
                }
                normal = referenceVector;
                point = transform.position;
                break;
            case 3: // Scripted

                break;
        }

        Debug.DrawRay(point,normalForward,Color.blue);
        Debug.DrawRay(point,normal,Color.green);
        Debug.DrawRay(point,normalRight,Color.red);

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
        float maxVal = Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z);
        scale = new Vector3(scale.x/maxVal,scale.y/maxVal,scale.z/maxVal);
        scale = new Vector3(1/Mathf.Abs(scale.x),1/Mathf.Abs(scale.y),1/Mathf.Abs(scale.z)); //create vector to "correct" for scale
        Vector3 scaleFull = new Vector3(1/scale.x,1/scale.y,1/scale.z);
        //This is only a bandaid solution and gets less accurate the more you skew a mesh, if you know how to get the actual normals after scaling please contact me at
        // n.dx on Discord or NDXDirectorsCut on Twitter

        //Debug.Log(scale);

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[iHit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[iHit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[iHit.triangleIndex * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = iHit.barycentricCoordinate;

        n0 = Vector3.Scale(n0,scale);
        n1 = Vector3.Scale(n1,scale);
        n2 = Vector3.Scale(n2,scale);
        
        baryCenter = Vector3.Scale(baryCenter,scaleFull);
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
