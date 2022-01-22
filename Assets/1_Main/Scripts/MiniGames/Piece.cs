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
    private int pointsToAddAlone1V3 = 0;
    public GameObject floatingPoint;
    public Color player0Color;
    public Color player1Color;
    public Color player2Color;
    public Color player3Color;

    public ParticleSystem shineFX;

    public CoinSound CoinSoundScript;

    private void Start()
    {
        collectThePiecesManager = GetComponentInParent<CollectThePiecesManager>();
        collectThePiecesManager.piecesNumber++;

        scoreScript = collectThePiecesManager.scoreScript;

        pointsToAddAlone1V3 = pointsToAdd * 2;
    }

    //Test FX 
    void CreateShine()
    {
        Instantiate(shineFX, transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController PC = collision.GetComponent<PlayerController>();

            GameObject floatPoint = Instantiate(floatingPoint, transform.position, Quaternion.identity);
            
            switch (PC.playerID)
            {
                case 0:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player0Color;

                    if (collectThePiecesManager.OneVSThreeEnable && collectThePiecesManager.playerAlone1v3 == 0)
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAddAlone1V3.ToString();
                        scoreScript.AddScore(pointsToAddAlone1V3, 0, 0, 0);
                    }
                    else
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();
                        scoreScript.AddScore(pointsToAdd, 0, 0, 0);
                    }
                    break;
                case 1:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player1Color;

                    if (collectThePiecesManager.OneVSThreeEnable && collectThePiecesManager.playerAlone1v3 == 1)
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAddAlone1V3.ToString();
                        scoreScript.AddScore(0, pointsToAddAlone1V3, 0, 0);
                    }
                    else
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();
                        scoreScript.AddScore(0, pointsToAdd, 0, 0);
                    }
                    break;
                case 2:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player2Color;

                    if (collectThePiecesManager.OneVSThreeEnable && collectThePiecesManager.playerAlone1v3 == 2)
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAddAlone1V3.ToString();
                        scoreScript.AddScore(0, 0, pointsToAddAlone1V3, 0);
                    }
                    else
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();
                        scoreScript.AddScore(0, 0, pointsToAdd, 0);
                    }
                    break;
                case 3:
                    floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().color = player3Color;

                    if (collectThePiecesManager.OneVSThreeEnable && collectThePiecesManager.playerAlone1v3 == 3)
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAddAlone1V3.ToString();
                        scoreScript.AddScore(0, 0, 0, pointsToAddAlone1V3);
                    }
                    else
                    {
                        floatPoint.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+ " + pointsToAdd.ToString();
                        scoreScript.AddScore(0, 0, 0, pointsToAdd);
                    }
                    break;
            }

            DestroyPiece();

            
        }
    }

    private void DestroyPiece()
    {
        CoinSoundScript.PlayerTakeCoin();
        collectThePiecesManager.piecesNumber--;
        CreateShine();
        Destroy(gameObject);
    }
}
