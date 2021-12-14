using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mole : MonoBehaviour
{
    private WhackAMoleManager whackAWholeScript;
    private Score scoreScript;

    [HideInInspector] public CheckPlayerIsClose currentTpScript;
    [HideInInspector] public float moleTimeStay;
    private float timer;

    [Header("Hammer")]
    public LayerMask hammerLayer;

    private Collider2D hammerCollider = null;
    private BoxCollider2D moleCollider;

    private float moleColliderBoundsX;
    private float moleColliderBoundsY;

    [Header("Score")]
    public int pointsToAdd = 1;
    public GameObject floatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    private Vector2 pos;
    public int moleID;
    public WhackAMoleManager whackAMoleScript;

    private void Start()
    {
        whackAWholeScript = transform.parent.GetComponentInParent<WhackAMoleManager>();
        scoreScript = whackAWholeScript.scoreScript;

        moleCollider = GetComponent<BoxCollider2D>();

        moleColliderBoundsX = moleCollider.bounds.size.x;
        moleColliderBoundsY = moleCollider.bounds.size.y;
        pos = transform.position;
    }

    private void FixedUpdate()
    {
        hammerCollider = Physics2D.OverlapBox(pos, new Vector2(moleColliderBoundsX, moleColliderBoundsY), 0f, hammerLayer);
    }

    private void Update()
    {
        if (hammerCollider != null)
        {
            GameObject floatPoint = Instantiate(floatingPoint, transform.position, Quaternion.identity);
            floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();

            switch (hammerCollider.GetComponentInParent<PlayerController>().playerID)
            {
                case 0:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player0Color;
                    scoreScript.AddScore(pointsToAdd, 0, 0, 0);
                    whackAWholeScript.scorePlayers[0]++;
                    break;
                case 1:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player1Color;
                    scoreScript.AddScore(0, pointsToAdd, 0, 0);
                    whackAWholeScript.scorePlayers[1]++;
                    break;
                case 2:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player2Color;
                    scoreScript.AddScore(0, 0, pointsToAdd, 0);
                    whackAWholeScript.scorePlayers[2]++;
                    break;
                case 3:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player3Color;
                    scoreScript.AddScore(0, 0, 0, pointsToAdd);
                    whackAWholeScript.scorePlayers[3]++;
                    break;
            }

            DestroyCrate();
            whackAWholeScript.MoleInScene--;
            //whackAWholeScript.moleDestroyed++;
            currentTpScript.alreadyMole = false;
        }

        if (timer <= moleTimeStay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            DestroyCrate();
            whackAWholeScript.MoleInScene--;
            //whackAWholeScript.moleDestroyed++;
            currentTpScript.alreadyMole = false;
        }
    }

    private void DestroyCrate()
    {
        whackAMoleScript.despawnMole(moleID);
        Destroy(gameObject);
    }

    /*Renderer the overlappBox 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(pos, new Vector2(moleColliderBoundsX, moleColliderBoundsY));
    }
    */
}
