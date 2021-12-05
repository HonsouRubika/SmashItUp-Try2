using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Piece : MonoBehaviour
{
    private CollectThePiecesManager collectThePiecesManager;
    private Score scoreScript;

    [Header("Score")]
    public int pointsToAdd = 1;
    public GameObject floatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    private void Start()
    {
        collectThePiecesManager = GetComponentInParent<CollectThePiecesManager>();
        collectThePiecesManager.piecesNumber++;

        scoreScript = collectThePiecesManager.scoreScript;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController PC = collision.GetComponent<PlayerController>();

            GameObject floatPoint = Instantiate(floatingPoint, transform.position, Quaternion.identity);
            floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();

            switch (PC.playerID)
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

            DestroyPiece();
        }
    }

    private void DestroyPiece()
    {
        collectThePiecesManager.piecesNumber--;
        Destroy(gameObject);
    }
}
