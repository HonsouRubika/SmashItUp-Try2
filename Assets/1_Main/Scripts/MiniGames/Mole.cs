using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mole : MonoBehaviour
{
    [Header("Hammer")]
    public LayerMask hammerLayer;

    private Collider2D hammerCollider = null;
    private BoxCollider2D moleCollider;

    private float moleColliderBoundsX;
    private float moleColliderBoundsY;

    [Header("Score")]
    private Score scoreScript;
    public int pointsToAdd = 1;
    public GameObject floatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    private Vector2 pos;

    private void Start()
    {
        scoreScript = transform.parent.GetComponentInParent<WhackAMoleManager>().scoreScript;

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
                    break;
                case 1:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player1Color;
                    scoreScript.AddScore(0, pointsToAdd, 0, 0);
                    break;
                case 2:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player2Color;
                    scoreScript.AddScore(0, 0, pointsToAdd, 0);
                    break;
                case 3:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player3Color;
                    scoreScript.AddScore(0, 0, 0, pointsToAdd);
                    break;
            }

            DestroyCrate();
        }
    }

    private void DestroyCrate()
    {
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
