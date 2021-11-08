using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehaviour : MonoBehaviour
{
    private WolfSound wolfSoundScript;

    public Score wolfScore;

    public bool player0isWolf;
    public SpriteRenderer player0Sprite;

    public bool player1isWolf;
    public SpriteRenderer player1Sprite;

    public bool player2isWolf;
    public SpriteRenderer player2Sprite;

    public bool player3isWolf;
    public SpriteRenderer player3Sprite;

    private int randomWolf;

    public Color wolfColor;
    private Color playerColor;

    private float lastTimePlayerBecomeWolf = 0;

    public PlayerController p0, p1, p2, p3;

    private float timer = 20f, timerActu;

    private GameManager GM;

    [Header("TP Points")]
    public Transform tpPoints0;
    public Transform tpPoints1;
    public Transform tpPoints2;
    public Transform tpPoints3;

    private void Start()
    {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            switch (player.GetComponent<PlayerController>().playerID)
            {
                case 0:
                    player.transform.position = tpPoints0.position;
                    break;
                case 1:
                    player.transform.position = tpPoints1.position;
                    break;
                case 2:
                    player.transform.position = tpPoints2.position;
                    break;
                case 3:
                    player.transform.position = tpPoints3.position;
                    break;
            }
        }

        wolfSoundScript = GetComponentInChildren<WolfSound>();

        timerActu = Time.time;

        randomWolf = Random.Range(0, 3);

        playerColor = player0Sprite.color;

        // Il faudrait que je puisse prendre la couleur des persos de bases afin de pouvoir les modifier en couleur de loup. Avec les spawner je ne peut pas les prend au lancement.
        if (randomWolf == 0)
        {
            player0isWolf = true;
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(2, 1, 1, 1);
            player0Sprite.color = wolfColor;
        }
        else if (randomWolf == 1)
        {
            player1isWolf = true;
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 2, 1, 1);
            player1Sprite.color = wolfColor;
        }
        else if (randomWolf == 2)
        {
            player2isWolf = true;
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 1, 2, 1);
            player2Sprite.color = wolfColor;
        }
        else if (randomWolf == 3)
        {
            player3isWolf = true;
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 1, 1, 2);
            player3Sprite.color = wolfColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (p0.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(2, 1, 1, 1);
            player0Sprite.color = wolfColor;
            Debug.Log("got hit :" + p0.lastTimeGotHit + "vs : "+ lastTimePlayerBecomeWolf);
        }
        if (p1.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 2, 1, 1);
            player1Sprite.color = wolfColor;
        }
        if (p2.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 1, 2, 1);
            player2Sprite.color = wolfColor;
        }
        if (p3.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            wolfSoundScript.WolfAttack();
            wolfScore.AddPosition(1, 1, 1, 2);
            player3Sprite.color = wolfColor;
        }
            // GESTION TIMER //
        if (Time.time >= timerActu + timer)
        {
            //next map
        }

    }
}
