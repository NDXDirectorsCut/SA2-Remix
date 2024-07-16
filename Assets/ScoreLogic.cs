using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreLogic : MonoBehaviour
{
    public TMP_Text scoreText;
    ScoreSystem scoreSys;
    // Start is called before the first frame update
    void Start()
    {
        scoreSys = transform.root.GetComponentInChildren<ScoreSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = scoreSys.score.ToString("0000000");
    }
}
