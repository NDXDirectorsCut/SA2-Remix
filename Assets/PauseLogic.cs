using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseLogic : MonoBehaviour
{
    bool on = false;
    public Button firstSelect;
    public GameObject[] deactivate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            on = !on;
            if(on == true)
            {
                firstSelect.Select();
                Cursor.lockState = CursorLockMode.None;
                AudioSource[] sounds = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                //Debug.Log(sounds[0].clip);
                for(int i = 0; i < sounds.Length; i++)
                {
                    sounds[i].mute = true;
                }
                Time.timeScale = 0;
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                for(int i = 0; i < deactivate.Length; i++)
                {
                    deactivate[i].SetActive(false);
                }

                AudioSource[] sounds = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                for(int i = 0; i < sounds.Length; i++)
                {
                    sounds[i].mute = false;
                }
                transform.GetChild(0).gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
