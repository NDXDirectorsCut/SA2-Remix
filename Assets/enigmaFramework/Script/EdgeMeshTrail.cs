using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
//Shitty code by NDXDirectorsCut, Discrot.
public class TrailPoint
{
    public Vector3 position;
    public float lifetime;
    public float percentage;
    public Vector3 velocity;
    public RaycastHit hit;
}

public class EdgeMeshTrail : MonoBehaviour
{
    [Header("Caps")]
    public Mesh trailCap;
    public Mesh trailEnd;
    [Header("Trail")]
    public Mesh edgeLoop;
    Mesh mesh;// = new Mesh { name = "MeshTrail" };
    //public int subdivisions; //not the number of subdivisions on the "mesh", rather the number of edgeloops
    public float lifetime;
    [Range(0,1)]
    public float percentageCutoff;
    [Range(0,1)]
    public float smoothLerp;
    public bool stopOnCollide;
    public float collisionRadius;
    public AnimationCurve sizeOverLifetime;
    public float rate;
    //public int maxPoints;
    public float startSpeed;
    public Vector3 force;
    List<TrailPoint> points = new List<TrailPoint>();
    Quaternion sRot;

    // Start is called before the first frame update
    void OnEnable()
    {
        points.Clear();
        StartCoroutine(SpawnPoint());
        mesh = new Mesh { name = "MeshTrail" };
    }

    IEnumerator SpawnPoint()
    {
        while(1>0)
        {
            TrailPoint newPoint = new TrailPoint();
            newPoint.position = transform.position;
            newPoint.lifetime = 0;
            newPoint.velocity = transform.up * startSpeed;
            points.Add(newPoint);
            yield return new WaitForSeconds(1/rate);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mesh.Clear();
        //Debug.Log(points.Count);
        if(points.Count == 0)
            StartCoroutine(SpawnPoint());
        int i,j,k;
        for(i=0;i<points.Count;i++)
        {
            if(i<points.Count-1 && points[i].percentage<=percentageCutoff )
            {
                points[i].lifetime += Time.deltaTime;
                points[i].percentage = points[i].lifetime/lifetime;
                if(stopOnCollide == true && Physics.SphereCast(points[i].position,collisionRadius,points[i].position - points[i+1].position, out points[i].hit, Vector3.Distance(points[i].position,points[i+1].position )))
                {
                    points[i].velocity = Vector3.zero;
                    points[i].position = points[i].hit.point + points[i].hit.normal*(collisionRadius*sizeOverLifetime.Evaluate(points[i].percentage));
                }
                else if(!Physics.CheckSphere(points[i].position,collisionRadius) || stopOnCollide == false)
                {
                    
                    points[i].velocity += force * Time.deltaTime;
                    points[i].position += points[i].velocity * Time.deltaTime;
                    points[i].position = Vector3.Lerp(points[i].position,points[i+1].position,smoothLerp);
                    
                }
            }
            if(i==0 && i+1 != points.Count)
            {
                //Debug.Log( (i+1) + " " + i );
                Vector3 upVector = (points[i+1].position - points[i].position).normalized;
                Vector3 rightVector = Vector3.Cross(Vector3.up,upVector).normalized;
                Vector3 forwardVector = Vector3.Cross(upVector,rightVector).normalized;
                Debug.DrawRay(points[i].position,rightVector,Color.red);
                Debug.DrawRay(points[i].position,upVector,Color.green);
                Debug.DrawRay(points[i].position,forwardVector,Color.blue);
            }
            if(i>0 && i<points.Count-1)
            {
                float aDist = Vector3.Distance( points[i].position, points[i-1].position);
                float bDist = Vector3.Distance( points[i].position, points[i+1].position);
                //Debug.Log(i + " " + (aDist / (aDist+bDist)));
                Vector3 upVector = Vector3.Lerp( (points[i].position - points[i-1].position).normalized , (points[i+1].position - points[i].position).normalized , aDist / (aDist+bDist) );
                Vector3 rightVector = Vector3.Cross(Vector3.up,upVector).normalized;
                Vector3 forwardVector = Vector3.Cross(upVector,rightVector).normalized;
                Debug.DrawRay(points[i].position,rightVector,Color.red);
                Debug.DrawRay(points[i].position,upVector,Color.green);
                Debug.DrawRay(points[i].position,forwardVector,Color.blue);
            }
            if(i==points.Count-1 && i!=0)
            {
                //Debug.Log(i + " " + (i-1));
                Vector3 upVector = -transform.up;
                Vector3 rightVector = Vector3.Cross(Vector3.up,upVector).normalized;
                Vector3 forwardVector = Vector3.Cross(upVector,rightVector).normalized;
                Debug.DrawRay(points[i].position,rightVector,Color.red);
                Debug.DrawRay(points[i].position,upVector,Color.magenta);
                Debug.DrawRay(points[i].position,forwardVector,Color.blue);
            }


            if(points[i].lifetime > lifetime)
            {
                points.RemoveAt(i);
            }
        }
        //points[points.Count-1].position = transform.position;

        for(i=0;i<points.Count;i++)
        {
            if(i>0)
             Debug.DrawLine(points[i].position,points[i-1].position,Color.black);
        }

        //int point = points.Count;
        int subdivisions = points.Count;
        int vertexCount = edgeLoop.vertices.Length;  
        int totalVertices = vertexCount*(subdivisions);
        int faceCount = 2*vertexCount * (subdivisions-1);

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[faceCount*3];
        //int l = 0;

        for(i=0;i<subdivisions;i++)
        {
            for(j=0;j<vertexCount;j++)
            {
                Vector3 forwardDir = Vector3.zero,upDir = Vector3.zero;
                float size = sizeOverLifetime.Evaluate(points[i].percentage);
                if(i<subdivisions-1 && i>0)
                {
                    forwardDir = Vector3.Lerp(-(points[i].position - points[i+1].position),(points[i].position - points[i-1].position),.5f);
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (size * (Quaternion.LookRotation(forwardDir,upDir) * edgeLoop.vertices[j])) + points[i].position  );
                }
                if(i == 0 && i+1 != points.Count)
                {
                    float finalSize = sizeOverLifetime.Evaluate(1);
                    //Debug.Log(finalSize);
                    forwardDir = -(points[i].position - points[i+1].position);
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (finalSize * (Quaternion.LookRotation(forwardDir,upDir) * edgeLoop.vertices[j])) + points[i].position  );
                }
                if(i==subdivisions-1 && i!=0)
                {
                    //Debug.Log(i);
                    float startSize = sizeOverLifetime.Evaluate(0);
                    forwardDir = transform.forward;
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    sRot = Quaternion.Lerp(sRot,Quaternion.LookRotation(forwardDir,upDir),.4f);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (startSize * (sRot * edgeLoop.vertices[j])) + points[i].position  );
                }
                //Debug.DrawRay(transform.TransformPoint(vertices[j+i*vertexCount]),Vector3.up,Color.blue);
                //Debug.Log(j+i*vertexCount + " " + totalVertices);
            }
        }
        //Debug.Log(triangles[triangles.Length-1] + " " + vertices.Length);
        mesh.vertices = vertices;
        k=0;
        for(i=0;i<subdivisions-1;i++) // 0770985989
        {
            for(j=0;j<vertexCount-1;j++)
            {
                
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount;
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount+1;
                k++;
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount+1;
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
            }
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount;
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
                triangles[k] = j+(vertexCount*i)-vertexCount+1;
                k++;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        //points[0].velocity = Vector3.zero;
        /*
        for(i=0;i<points.Count-1;i++)
        {
            points[i].lifetime += Time.deltaTime;
            points[i].percentage = points[i].lifetime/lifetime;
            

            if(points[i].percentage <= percentageCutoff)
            {
                
                if(stopOnCollide == true && Physics.SphereCast(points[i].position,collisionRadius,points[i].position - points[i+1].position, out points[i].hit, Vector3.Distance(points[i].position,points[i+1].position )))
                {
                    points[i].position = points[i].hit.point;
                    points[i].velocity = Vector3.zero;
                    Debug.DrawRay(points[i].hit.point,points[i].hit.normal,Color.blue);
                    break;
                }
                else if(!Physics.CheckSphere(points[i].position,collisionRadius))
                {
                    points[i].velocity += force * Time.deltaTime;
                    points[i].position += points[i].velocity * Time.deltaTime;
                }
            }
            else
            {
                points[i].position = Vector3.Lerp(points[i].position,points[i+1].position,.2f);
            }

            Debug.DrawLine(points[i+1].position,points[i].position,Color.red);
            if(points[i].lifetime >= lifetime)
            {
                //points[i] = null;
                points.RemoveAt(i);
            }
            //Debug.DrawRay(points[i].position,points[i].velocity,Color.blue);
        }
        if(Input.GetKey(KeyCode.F))
        {
            points.Clear();
        }
        //Debug.DrawLine(transform.position,points[points.Count-1].position,Color.red);

        //Debug.DrawLine(endPos,points[0].position,Color.red);
        Debug.DrawRay(points[0].position,Vector3.up,Color.green);
        Debug.DrawRay(points[points.Count-1].position,-Vector3.up,Color.blue);
        */
        /*
        int subdivisions = points.Count;
        int vertexCount = edgeLoop.vertices.Length;  
        int totalVertices = vertexCount*(subdivisions);
        int faceCount = 2*vertexCount * (subdivisions-1);

        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[faceCount*3];
        //int l = 0;

        for(i=0;i<subdivisions;i++)
        {
            for(j=0;j<vertexCount;j++)
            {
                Vector3 forwardDir = Vector3.zero,upDir = Vector3.zero;
                float size = sizeOverLifetime.Evaluate(points[i].percentage);
                if(i<subdivisions-1 && i>0)
                {
                    forwardDir = Vector3.Lerp(-(points[i].position - points[i+1].position),(points[i].position - points[i-1].position),.5f);
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (size * (Quaternion.LookRotation(forwardDir,upDir) * edgeLoop.vertices[j])) + points[i].position  );
                }
                if(i == 0)
                {
                   
                    forwardDir = -(points[i].position - points[i+1].position);
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (size * (Quaternion.LookRotation(forwardDir,upDir) * edgeLoop.vertices[j])) + points[i].position  );
                }
                if(i==subdivisions-1)
                {
                    //Debug.Log(i);
                    forwardDir = transform.forward;
                    upDir = Vector3.Cross(transform.right,forwardDir);
                    sRot = Quaternion.Lerp(sRot,Quaternion.LookRotation(forwardDir,upDir),.4f);
                    vertices[j+(i*vertexCount)] = transform.InverseTransformPoint( (size * (sRot * edgeLoop.vertices[j])) + points[i].position  );
                }
                //Debug.DrawRay(transform.TransformPoint(vertices[j+i*vertexCount]),Vector3.up,Color.blue);
                //Debug.Log(j+i*vertexCount + " " + totalVertices);
            }
        }
        

        mesh.vertices = vertices;
        k=0;
        for(i=0;i<subdivisions-1;i++)
        {
            for(j=0;j<vertexCount-1;j++)
            {
                
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount;
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount+1;
                k++;
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount+1;
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
            }
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+vertexCount;
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
                triangles[k] = j+(vertexCount*i);
                k++;
                triangles[k] = j+(vertexCount*i)+1;
                k++;
                triangles[k] = j+(vertexCount*i)-vertexCount+1;
                k++;
        }
        //Debug.Log(triangles[triangles.Length-1] + " " + vertices.Length);

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        */
    }
}
