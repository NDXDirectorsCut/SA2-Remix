using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadStage : MonoBehaviour
{

    public string stageName;   
    public float delay;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(stageName, LoadSceneMode.Single);
    }

    void OnClick()
    {
        StartCoroutine(Load());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
