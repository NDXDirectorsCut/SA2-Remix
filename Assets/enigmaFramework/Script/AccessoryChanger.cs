using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryChanger : MonoBehaviour
{
    public List<GameObject> firstSet = new List<GameObject>();
    public List<GameObject> secondSet = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad7))
        {
            for(int i = 0;i<firstSet.Count;i++)
            {
                firstSet[i].SetActive(true);
            }
            for(int i = 0;i<secondSet.Count;i++)
            {
                secondSet[i].SetActive(false);
            }
        }
        if(Input.GetKey(KeyCode.Keypad8))
        {
            for(int i = 0;i<firstSet.Count;i++)
            {
                firstSet[i].SetActive(false);
            }
            for(int i = 0;i<secondSet.Count;i++)
            {
                secondSet[i].SetActive(true);
            }
        }
    }
}
