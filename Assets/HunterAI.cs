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
    public float projectileLife;
    public float projectileSpeed;
    public float projectileGravity;
    public float projectileRadius;
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

    IEnumerator ProjectileBehavior(GameObject projectile, Vector3 startVelocity)
    {
        Destroy(projectile,projectileLife);
        Vector3 velocity = startVelocity;
        while(projectile != null)
        {
            projectile.transform.position += velocity * Time.deltaTime;
            velocity += -Vector3.up * Time.deltaTime * projectileGravity;
	    if(Physics.CheckSphere(projectile.transform.position,projectileRadius))
	    {
		Destroy(projectile);
	    }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Shoot()
    {
	    while(target != null && Mathf.Abs(turnRate) < .5f)
	    {
	        shooting = true;
	        yield return new WaitForSeconds(shootDelay);
            if(Mathf.Abs(turnRate) > .5f || target == null)
                break;
            Debug.Log(turnRate);
            MuzzleFlash.GetComponent<ParticleSystem>().Play();
	    GameObject shot = Instantiate(Projectile,MuzzleFlash.transform.position,Quaternion.identity);
            animator.CrossFadeInFixedTime("Shoot",.125f,0,0);
            //yield return new WaitForSeconds(.15f);
            StartCoroutine(ProjectileBehavior(shot,-(MuzzleFlash.transform.position - (target.position + target.up*.75f)) * projectileSpeed));
	    }
	    yield return null;
        shooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player");
	    Transform tempTarget = null;
        float minDist = range+1;
        foreach(GameObject potentialTarget in potentialTargets)
        {
	    float dist = Vector3.Distance(transform.position,potentialTarget.transform.position);

            if(dist < range && dist < minDist)
            {
		RaycastHit testHit;
		Physics.Raycast(transform.position+transform.up*1.5f, -(transform.position - potentialTarget.transform.position), out testHit, dist,LayerMask.GetMask("Default"));
		if(testHit.transform == null)
		{
                    minDist = Vector3.Distance(transform.position,potentialTarget.transform.position);
               	    tempTarget = potentialTarget.transform;
		}
            }
        }

	    target = tempTarget;	
	    animator.SetBool("Target",tempTarget != null);

        turnRate = Vector3.SignedAngle(transform.forward,targetDir,transform.up);
	    

        if(Physics.Raycast(transform.position + transform.up * 1.5f,-transform.up,out hit,2f))
        {
            normal = hit.normal;
            transform.position = hit.point;
            if(target != null)
            {
                Debug.Log(turnRate);
                targetDir = Vector3.ProjectOnPlane((transform.position - target.position).normalized,normal);
                if(shooting == false && Mathf.Abs(turnRate) < .5f)
                {
                    StartCoroutine(Shoot());
                }
            }
            else
	        {
                targetDir = transform.forward;
            }
            transform.rotation = Quaternion.LookRotation(transform.forward,normal);
            //Debug.Log("Hit");
            //Debug.DrawRay(hit.point,normal,Color.blue);
            grounded = hit.transform == null ? false : true;
	        animator.SetFloat("TurnRate",turnRate);
        }

        transform.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(transform.forward,normal)
 , Quaternion.LookRotation(targetDir,normal) ,turnSpeed * Time.deltaTime );//Vector3.MoveTowards(transform.forward,targetDir,turnSpeed * Time.deltaTime); 

    }
}