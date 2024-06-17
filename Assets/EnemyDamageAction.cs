using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageAction : MonoBehaviour
{
    public GameObject root;
    public bool canTakeDamage;
    public int health;
    public GameObject deathEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator EnemyDamage()
    {
        //Debug.Log("Collided");
        if(canTakeDamage == true)
            health -= 1;

        if(health <1)
        {
            GameObject effect = Instantiate(deathEffect,transform.position,Quaternion.identity);
            Destroy(effect,2.5f);
            Destroy(root);

        }
        yield return null;
    }

    void OnCollisionEnter(Collision coll)
    {
        StartCoroutine(EnemyDamage());
    }

}
