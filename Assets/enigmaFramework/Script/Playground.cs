using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playground : MonoBehaviour
{
    public int movementType;
    public float modelHeight;
    public float gravityForce;
    public Vector3 referenceVector;
    public Vector3 velocity;
    Rigidbody rigidbody;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Rigidbody>())
            rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(Physics.Raycast(transform.position,-transform.up,out hit,1f+modelHeight))
        {
            Vector3 normal = InterpolateNormal(hit);
            switch(movementType)
            {
                case 0: //no Rigidbody
                    transform.position = hit.point + normal*modelHeight;
                    transform.up = normal;
                    Vector3 velocityRight = Vector3.Cross(normal,velocity).normalized;
                    Vector3 normalVelocity = Vector3.Cross(velocityRight,normal).normalized;
                    
                    velocity = normalVelocity * velocity.magnitude;
                    velocity += -referenceVector * gravityForce * Time.fixedDeltaTime;
                    transform.position += velocity * Time.fixedDeltaTime;

                    Debug.DrawRay(transform.position,normal,Color.green);
                    Debug.DrawRay(transform.position,velocityRight,Color.red);
                    Debug.DrawRay(transform.position,normalVelocity,Color.blue);
                    break;
                case 1: //Rigidbody
                    if(rigidbody != null)
                    {
                        rigidbody.velocity += -referenceVector * gravityForce * Time.fixedDeltaTime;
                        transform.position = hit.point + normal*modelHeight;
                        transform.up = normal;
                        Vector3 rightVelocity = Vector3.Cross(normal,rigidbody.velocity).normalized;
                        Vector3 velocityNormal = Vector3.Cross(rightVelocity,normal).normalized;

                        rigidbody.velocity = velocityNormal * rigidbody.velocity.magnitude;
                        
                        //transform.position += rigidbody.velocity * Time.fixedDeltaTime;

                        Debug.DrawRay(transform.position,normal,Color.green);
                        Debug.DrawRay(transform.position,rightVelocity,Color.red);
                        Debug.DrawRay(transform.position,velocityNormal,Color.blue);
                    }
                    break;
                case 2: //Rigidbody Precision Calculation
                    break;
            }
            
        }
    }

    Vector3 InterpolateNormal(RaycastHit iHit)
    {

        MeshCollider meshCollider = iHit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return iHit.normal;
            Debug.Log("Abort Mission, using hit.normal instead");
        }

        Mesh mesh = meshCollider.sharedMesh;

        if(mesh.isReadable == false)
        {
            return iHit.normal;
            Debug.Log("Abort Mission, using hit.normal instead");
        }

        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        Vector3 scale = hit.transform.lossyScale;
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
