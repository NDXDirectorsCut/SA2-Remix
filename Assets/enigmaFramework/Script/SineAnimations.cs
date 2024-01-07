using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SineAnimation
{
    public string name = "New SineAnimation";
    
    [Space(10)]
    [Range(-10f,10f)]
    public float offset;
    public bool startWithRandomOffset;
    [Space(10)]
    [Header("Position")]
    public Vector3 sinPosition;
    public float sinPosSpeed;
    public Vector3 cosPosition;
    public float cosPosSpeed;
    [Space(10)]
    [Header("Rotation")]
    public Vector3 rotOrigin;
    [Space(5)]
    public Vector3 sinRotation;
    public float sinRotSpeed;
    public Vector3 cosRotation;
    public float cosRotSpeed;
    [Space(10)]
    [Header("Bending")]
    public Vector3 bendOrigin;
    public bool affectedOrigin;
    [Space(5)]
    public Vector3 baseBend;
    public Vector3 sinBending;
    public float sinBendSpeed;
    public Vector3 cosBending;
    public float cosBendSpeed;
    public float distanceMultiplier;

}




public class SineAnimations : MonoBehaviour
{
    public List<SineAnimation> animationList = new List<SineAnimation>();
    public bool useFixedUpdate;
    Vector3 startPosition;
    Quaternion startRotation;
    float startTime;

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] origVertices;
    
    void Start()
    {
        if( GetComponent<MeshFilter>() != null )
        {
            mesh = GetComponent<MeshFilter>().mesh;
            origVertices = mesh.vertices;
        }
    }
    // If you want to resync the animation restart the script lolololo
    void OnEnable()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startTime = Time.time;

        foreach (SineAnimation anim in animationList)
        {
                
            if(anim.startWithRandomOffset == true)
            {
                anim.offset = UnityEngine.Random.Range(-10,10);

            }
        }

        if( GetComponent<MeshFilter>() != null )
        {
            mesh = GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = (originTest + startPosition) - Quaternion.AngleAxis(Time.time*angle,Vector3.right) * originTest;
        //transform.rotation = Quaternion.AngleAxis(Time.time*angle,Vector3.right) * startRotation;
        if(useFixedUpdate == false)
        {
            Vector3 posResult = Vector3.zero;
            Quaternion rotResult = Quaternion.identity; 
            Quaternion[] BendResult = new Quaternion[vertices.Length];
            int j;
            for(j=0;j<vertices.Length;j++)
            {
                BendResult[j] = Quaternion.identity;
            }
            //vertices = origVertices;
            int i;
            foreach (SineAnimation anim in animationList)
            {
                Debug.DrawRay(transform.position+anim.bendOrigin,Vector3.up,Color.red);
                float time = Time.time - startTime + anim.offset;
                
                rotResult *= Quaternion.Euler(anim.sinRotation*Mathf.Sin(time*anim.sinRotSpeed) + anim.cosRotation*Mathf.Cos(time*anim.cosRotSpeed));//* Quaternion.Euler(anim.cosRotation*Mathf.Cos(time*anim.cosRotSpeed));
                posResult += rotResult * ( (anim.sinPosition* Mathf.Sin(time* anim.sinPosSpeed)) + (anim.cosPosition* Mathf.Cos(time* anim.cosPosSpeed))
                - (anim.rotOrigin + (anim.sinPosition* Mathf.Sin(time* anim.sinPosSpeed)) + (anim.cosPosition* Mathf.Cos(time* anim.cosPosSpeed))) )+ (anim.rotOrigin + (anim.sinPosition* Mathf.Sin(time* anim.sinPosSpeed)) + (anim.cosPosition* Mathf.Cos(time* anim.cosPosSpeed)));
                //transform.position = rotResult * (startPosition - (startPosition+anim.rotOrigin)) + (startPosition+anim.rotOrigin);
                if(mesh!=null)
                {
                    
                    for(i=0;i<vertices.Length;i++)
                    {
                        Vector3 finalOrigin = anim.bendOrigin;
                        if(i>0 && anim.affectedOrigin == true)
                        {
                            
                        }

                        float dist = Vector3.Distance(vertices[i],anim.bendOrigin) * anim.distanceMultiplier;
                        BendResult[i] *= Quaternion.Euler(anim.baseBend*dist + (anim.sinBending* dist* Mathf.Sin(time* anim.sinBendSpeed)) + (anim.cosBending* dist* Mathf.Cos(time* anim.cosBendSpeed))  );
                        //Quaternion cosBendResult = Quaternion.Euler(anim.cosBending* dist* Mathf.Sin(time* anim.cosBendSpeed));
                        
                        //Debug.Log(vertices[i] + "S " + i);
                        //(origVertices[i]-anim.bendOrigin) + (BendResult* anim.bendOrigin);//0YH69-LQPR9-5Y7G6
                        vertices[i] = BendResult[i] * (origVertices[i] -anim.bendOrigin) + anim.bendOrigin;
                        //Debug.Log(vertices[i] + "E " + i);
                    }
                    
                    //mesh.RecalculateBounds();
                }
            }

            if(mesh!=null)
            {
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                if(transform.GetComponent<MeshCollider>() != null)
                {
                    transform.GetComponent<MeshCollider>().sharedMesh = mesh;
                }
            }
            transform.position = startPosition + posResult;
            
            transform.rotation = startRotation * rotResult;
        }

    }

    void FixedUpdate()
    {

    }
}
