using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text p1, p2, p3, p4;

    public float TimerNextMap = 3;
    public float TimerNextMapActu = 0;

    private void Start()
    {
        //appel de la fonction setScore du ScoreManager
        p1.text = "Player 1 : " + GameManager.Instance.GetComponent<GameManager>().getScorePlayer(1);
        p2.text = "Player 2 : " + GameManager.Instance.GetComponent<GameManager>().getScorePlayer(2);
        p3.text = "Player 3 : " + GameManager.Instance.GetComponent<GameManager>().getScorePlayer(3);
        p4.text = "Player 4 : " + GameManager.Instance.GetComponent<GameManager>().getScorePlayer(4);

        TimerNextMapActu = Time.time + TimerNextMap;

    }

    private void Update()
    {
        if (Time.time >= TimerNextMapActu)
        {
            //Next Map
            GameManager.Instance.GetComponent<GameManager>().NextMap();
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

}
