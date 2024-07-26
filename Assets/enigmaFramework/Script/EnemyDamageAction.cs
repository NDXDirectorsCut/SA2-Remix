using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageAction : MonoBehaviour
{
    public GameObject root;
    public bool canTakeDamage;
    public int health;
    public int killPoints = 100;
    public GameObject deathEffect;
    public AudioClip deathSound;
    //ScoreSystem scoreSys;
    [Range(0,1)]
    public float soundVolume = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator EnemyDamage(GameObject touch)
    {
        //Debug.Log("Collided");
        if(canTakeDamage == true)
            health -= 1;

        if(health <1)
        {
            if(touch.GetComponent<ScoreSystem>() != null)
					touch.GetComponent<ScoreSystem>().score += killPoints;

            JumpAction jumpScript = touch.GetComponent<JumpAction>();
            EnigmaPhysics enigmaPhysics = touch.GetComponent<EnigmaPhysics>();
            

            GameObject effect = Instantiate(deathEffect,transform.position,Quaternion.identity);
            AudioSource sound = effect.AddComponent(typeof(AudioSource)) as AudioSource;
            sound.clip = deathSound;
            sound.volume = soundVolume;
            
            sound.Play();

            Destroy(effect,2.5f);
            Destroy(root);

        }
        yield return null;
    }

    void OnCollisionEnter(Collision col)
    {
        //if(col.gameObject.GetComponent<EnigmaPhysics>().canTriggerAction == true)
            //StartCoroutine(EnemyDamage(col.gameObject));
    }

}
