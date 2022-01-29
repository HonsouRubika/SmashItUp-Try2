using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewScoreSystem : MonoBehaviour
{
    //New score system
    [Header("Score System")]
    public float scoreStaysTime;
    public float timeBeforeFirstIncrementation;
    public float timeBtwPops;

    private bool displayScore = false;

    private float timerScoreStays = 0;

    [Header("Canvas")]
    public GameObject ScorePanel;
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;

    private Transform[] P1Points;

    private void Start()
    {
        ScorePanel.SetActive(false);

        P1Points = new Transform[P1.transform.childCount];
        for (int i = 0; i < P1Points.Length; i++)
        {
            P1Points[i] = P1.transform.GetChild(i);
        }
    }

    private void Update()
    {
        if (timerScoreStays < scoreStaysTime && displayScore)
        {
            timerScoreStays += Time.deltaTime;
        }
        else
        {
            DisplayScore(false);
        }
    }

    public void DisplayScore(bool enable)
    {
        switch (enable)
        {
            case true:
                displayScore = true;
                ScorePanel.SetActive(true);
                timerScoreStays = 0;
                StartCoroutine(DistributePoints());
                break;
            case false:
                displayScore = false;
                ScorePanel.SetActive(false);
                GameManager.Instance.LoadSceneAfterScore();
                break;
        }
    }

    private IEnumerator DistributePoints()
    {
        yield return new WaitForSeconds(timeBeforeFirstIncrementation);


    }
}
