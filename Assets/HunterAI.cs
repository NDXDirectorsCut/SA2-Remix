using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAI : MonoBehaviour
{
    Transform target;
    public Animator animator;
    public float range;
    public float turnSpeed;
    public float shootDelay;
    public GameObject Projectile;
    public GameObject MuzzleFlash;
    float turnRate;
    bool shooting;
    RaycastHit hit;
    Vector3 normal;
    Vector3 targetDir;
    bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        normal = Vector3.up;
    }

    IEnumerator Shoot()
    {
	while(target != null)
	{
	    shooting = true;
	    yield return new WaitForSeconds(shootDelay);
	    GameObject shot = Instantiate(Projectile,MuzzleFlash.transform.position,Quaternion.identity);
	}
	yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
	Transform tempTarget = null;
        float minDist = range+1;
        foreach(GameObject potentialTarget in potentialTargets)
        {
            if(Vector3.Distance(transform.position,potentialTarget.transform.position) < range && Vector3.Distance(transform.position,potentialTarget.transform.position) < minDist)
            {
                minDist = Vector3.Distance(transform.position,potentialTarget.transform.position);
                tempTarget = potentialTarget.transform;
            }
        }

	target = tempTarget;	
	animator.SetBool("Target",tempTarget != null);

        if(Physics.Raycast(transform.position + transform.up * 1.5f,-transform.up,out hit,2f))
        {
            normal = hit.normal;
            transform.position = hit.point;
            if(target != null)
            {

                targetDir = Vector3.ProjectOnPlane((transform.position - target.position).normalized,normal);
		if(shooting == false)
			StartCoroutine(Shoot());
	    }
	    else
	    {
		targetDir = transform.forward;
	    }
            transform.rotation = Quaternion.LookRotation(transform.forward,normal);
            Debug.Log("Hit");
            Debug.DrawRay(hit.point,normal,Color.blue);
        }
        grounded = hit.transform == null ? false : true;
	
	turnRate = Vector3.SignedAngle(transform.forward,targetDir,transform.up);

	transform.forward = Vector3.MoveTowards(transform.forward,targetDir,turnSpeed * Time.deltaTime); 
        

	animator.SetFloat("TurnRate",turnRate);
    }
}
