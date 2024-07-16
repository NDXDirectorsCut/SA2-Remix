using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class ResultLogic : MonoBehaviour
{

    public Image rankImage;
    public AudioSource finalSFX;

    public ScoreSystem scoreScript;
    public TimerLogic timerScript;
    public RingAction ringScript;

    public TMP_Text scoreText;
    public TMP_Text ringText;
    public TMP_Text totalRingText;
    public TMP_Text timeBonusText;
    public TMP_Text finalScoreText;

    public TMP_Text mCount;
    public TMP_Text sCount;
    public TMP_Text msCount;

    public int totalRings;
    int timeBonus;
    public int totalScore;

    // Start is called before the first frame update
    void Start()
    {
        //timeBonus = 10000 - (20 * ());
        //timeBonus = Mathf.Clamp(timeBonus,0,999999);
        //scoreScript = transform.root.GetComponentInChildren<ScoreSystem>(true);
        //timerScript = transform.root.GetComponentInChildren<TimerLogic>(true);
        //ringScript = transform.root.GetComponentInChildren<RingAction>(true);
        
    }

    // Update is called once per frame
    public IEnumerator UpdateValues()
    {
        timeBonus = (int)(10000 - (20 * (Mathf.Clamp(Time.timeSinceLevelLoad - 60, 0, Time.timeSinceLevelLoad))));
        totalScore = timeBonus + scoreScript.score;

        scoreText.text = scoreScript.score.ToString();
        ringText.text = ringScript.ringCount.ToString();
        totalRingText.text = totalRings.ToString();
        mCount.text = timerScript.mCount.text;
        sCount.text = timerScript.sCount.text;
        msCount.text = timerScript.msCount.text;

        timeBonusText.text = timeBonus.ToString();
        finalScoreText.text = scoreScript.score.ToString();

        yield return new WaitForSeconds(4.75f);
        finalSFX.Play();
        timeBonusText.text = "0";
        finalScoreText.text = totalScore.ToString();

        yield return new WaitForSeconds(3.75f);
        SceneManager.LoadScene("Menu",LoadSceneMode.Single);

        yield return null;
    }
}
