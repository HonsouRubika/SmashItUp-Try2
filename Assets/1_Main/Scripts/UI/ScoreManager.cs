using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text p1, p2, p3, p4;

    public float TimerNextMap = 3;
    public float TimerNextMapActu = 0;

    private GameObject[] players;
    public List<Transform> tpPoints = new List<Transform>();

    private void Start()
    {
        //appel de la fonction setScore du ScoreManager
        p1.text = "Player 1 : " + GameManager.Instance.getScorePlayer(1);
        p2.text = "Player 2 : " + GameManager.Instance.getScorePlayer(2);
        p3.text = "Player 3 : " + GameManager.Instance.getScorePlayer(3);
        p4.text = "Player 4 : " + GameManager.Instance.getScorePlayer(4);

        TimerNextMapActu = Time.time + TimerNextMap;
        SpawnPlayer();
    }

    private void Update()
    {
        if (Time.time >= TimerNextMapActu)
        {
            //Next Map
            GameManager.Instance.NextMap();
        }
    }

    public void setScore(int playerID, int score)
    {
        switch (playerID)
        {
            case 1:
                p1.text = "Player 1 : " + score;
                break;
            case 2:
                p2.text = "Player 2 : " + score;
                break;
            case 3:
                p3.text = "Player 3 : " + score;
                break;
            case 4:
                p4.text = "Player 4 : " + score;
                break;
            default:
                Debug.Log("error UI score");
                break;
        }
    }

    private void SpawnPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = tpPoints[i].position;
        }
    }
}
