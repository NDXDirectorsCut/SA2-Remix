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
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1;
        Debug.Log("LoadingStage");
        SceneManager.LoadScene(stageName, LoadSceneMode.Single);
    }

    void OnClick()
    {
        Debug.Log("Clicked");
        StartCoroutine(Load());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
