using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EdgeMeshTrail : MonoBehaviour
{
    [Header("Caps")]
    public Mesh trailCap;
    public Mesh trailEnd;
    [Header("Trail")]
    public Mesh edgeLoop;
    public int subdivisions;

    // Start is called before the first frame update
    void OnEnable()
    {
        Mesh mesh = new Mesh { name = "MeshTrail" };
        int length = edgeLoop.vertices.Length;
        int i,j;
        Vector3[] vertices = new Vector3[length * (subdivisions+1)];
        int[] triangles = new int[] {0,length,length+1};
        /*
        Debug.DrawLine(transform.position+vertices[0],transform.position+vertices[length],Color.red);
        Debug.DrawLine(transform.position+vertices[length],transform.position+vertices[length+1],Color.red);
        Debug.DrawLine(transform.position+vertices[length+1],transform.position+vertices[0],Color.red);*/
        for(i=0;i<subdivisions+1;i++)
        {
            for(j=0;j<length;j++)
            {
                vertices[j+i*length] = edgeLoop.vertices[i] + Vector3.forward*i;
                //Debug.Log(j+i*length);
            }
        }
        Debug.Log(vertices[length]);
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
