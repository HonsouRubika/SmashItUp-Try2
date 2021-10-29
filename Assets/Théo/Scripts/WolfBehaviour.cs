using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehaviour : MonoBehaviour
{
    public bool player0;
    public SpriteRenderer player0Sprite;

    public bool player1;
    public SpriteRenderer player1Sprite;

    public bool player2;
    public SpriteRenderer player2Sprite;

    public bool player3;
    public SpriteRenderer player3Sprite;

    private int randomWolf;

    public Color wolfColor;
    private Color playerColor;

    private float lastTimePlayerBecomeWolf = 0;

    public PlayerController p0, p1, p2, p3;

    private float timer = 20f, timerActu;

    // Start is called before the first frame update
    void Start()
    {
        timerActu = Time.time;

        randomWolf = Random.Range(0, 3);

        playerColor = player0Sprite.color;

        if (randomWolf == 0)
        {
            player0 = true;
            player0Sprite.color = wolfColor;
        }
        else if (randomWolf == 1)
        {
            player1 = true;
            player1Sprite.color = wolfColor;
        }
        else if (randomWolf == 2)
        {
            player2 = true;
            player2Sprite.color = wolfColor;
        }
        else if (randomWolf == 3)
        {
            player3 = true;
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
            player0Sprite.color = wolfColor;
            Debug.Log("got hit :" + p0.lastTimeGotHit + "vs : "+ lastTimePlayerBecomeWolf);
        }
        if (p1.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            player1Sprite.color = wolfColor;
        }
        if (p2.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            player2Sprite.color = wolfColor;
        }
        if (p3.lastTimeGotHit >= lastTimePlayerBecomeWolf)
        {
            lastTimePlayerBecomeWolf = Time.time;
            //devient loup
            player3Sprite.color = wolfColor;
        }
            // GESTION TIMER //
        if (Time.time >= timerActu + timer)
        {
            //next map
        }

    }
}
