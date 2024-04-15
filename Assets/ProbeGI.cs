using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeGI : MonoBehaviour
{
    public GameObject original;
    public float distance;
    public float attempts;
    public float buffer;
    //public List<GameObject> points = new List<GameObject>();
    Vector3 position,direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        GameObject[] probes = GameObject.FindGameObjectsWithTag("Respawn");
        foreach(GameObject probe in probes)
            Destroy(probe);

        position = transform.position; direction = transform.forward;
        for(int i = 0; i < attempts; i++)
        {
            if(Physics.Raycast(position,direction,out hit, distance) )
            {
                Debug.DrawLine(position,hit.point,Color.red);
                position = hit.point; direction = Vector3.Reflect(direction,hit.normal);
                Instantiate(original,hit.point + hit.normal*buffer,Quaternion.identity);
            }
        }
    }
}
