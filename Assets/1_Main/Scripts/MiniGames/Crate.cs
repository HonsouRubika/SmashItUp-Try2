using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crate : MonoBehaviour
{
    private DetructCrate_Rules detructCrateScript;
    private Score scoreScript;
    private DestructSound destructSoundScript;

    [Header("Hammer")]
    public LayerMask hammerLayer;

    [Header("Score")]
    public int pointsToAdd = 1;
    public GameObject floatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    private Collider2D hammerCollider = null;
    private BoxCollider2D crateCollider;

    private float crateColliderBoundsX;
    private float crateColliderBoundsY;

    private Vector2 pos;

    private void Start()
    {
        detructCrateScript = GetComponentInParent<DetructCrate_Rules>();
        detructCrateScript.cratesNumber++;

        scoreScript = detructCrateScript.scoreScript;

        crateCollider = GetComponent<BoxCollider2D>();

        crateColliderBoundsX = crateCollider.bounds.size.x;
        crateColliderBoundsY = crateCollider.bounds.size.y;
        pos = transform.position;

        destructSoundScript = GetComponentInChildren<DestructSound>();
    }

    private void FixedUpdate()
    {
        hammerCollider = Physics2D.OverlapBox(pos, new Vector2(crateColliderBoundsX, crateColliderBoundsY), 0f, hammerLayer);
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
        detructCrateScript.cratesNumber--;
        Destroy(gameObject);
        destructSoundScript.PlayerDestroy();
    }

    /*Renderer the overlappBox 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(pos, new Vector2(crateColliderBoundsX, crateColliderBoundsY));
    }
    */
}
