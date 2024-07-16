using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageAction : MonoBehaviour
{
    public GameObject root;
    public bool canTakeDamage;
    public int health;
    public GameObject deathEffect;
    public AudioClip deathSound;
    ScoreSystem scoreSys;
    [Range(0,1)]
    public float soundVolume = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        scoreSys = GetComponent<ScoreSystem>();
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
        StartCoroutine(EnemyDamage());
    }

}
