using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPlayers : MonoBehaviour
{
    public GameObject[] players;

    [Header("Focus Settings")]
    public float timeShowingPlayers;

    public GameObject cerclePrefab;
    private GameObject[] cercles;

    public Vector2 cercleSize;

    [Header("Hider Settings")]
    public GameObject canvasRef;
    public GameObject hideScreen;
    public Color colorHideScreen;

    //public Image minigameTitleImage;

    [Header("Mini-game title")]
    public Sprite captureTheFlagSprite;
    public Sprite keepTheFlagSprite;
    public Sprite dontBeWolfSprite;
    public Sprite contaminationSprite;
    public Sprite zoneSprite;
    public Sprite destroyCratesSprite;

    private Canvas canvas;

    private void Start()
    {
        hideScreen.GetComponent<SpriteRenderer>().color = colorHideScreen;
        canvas = canvasRef.GetComponent<Canvas>();
    }

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        cercles = new GameObject[players.Length];
    }

    public void EnableFocus()
    {
        StartCoroutine(GetReference());

        StartCoroutine(IncrementTimer());
    }

    private void SpawnCercle()
    {
        canvasRef.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            cercles[i] = Instantiate(cerclePrefab, players[i].transform.position, Quaternion.identity);
        }
    }

    private void DestroyCercle()
    {
        canvasRef.SetActive(false);
        Time.timeScale = 1f;

        for (int i = 0; i < cercles.Length; i++)
        {
            Destroy(cercles[i]);
        }
    }

    public void SetGameTitle(string minigameName)
    {
        /*
        switch (minigameName)
        {
            case "CaptureTheFlag":
                minigameTitleImage.sprite = captureTheFlagSprite;
                break;
            case "KeepTheFlag":
                minigameTitleImage.sprite = keepTheFlagSprite;
                break;
            case "DontBeWolf":
                minigameTitleImage.sprite = dontBeWolfSprite;
                break;
            case "Contamination":
                minigameTitleImage.sprite = contaminationSprite;
                break;
            case "Zone":
                minigameTitleImage.sprite = zoneSprite;
                break;
            case "DestroyCrates":
                minigameTitleImage.sprite = destroyCratesSprite;
                break;
        }
        */
    }

    private IEnumerator IncrementTimer()
    {
        yield return new WaitForSecondsRealtime(0.0001f);

        SpawnCercle();
        GameManager.Instance.isShowingPlayers = true;

        yield return new WaitForSecondsRealtime(timeShowingPlayers);

        DestroyCercle();
        GameManager.Instance.isShowingPlayers = false;
    }

    private IEnumerator GetReference()
    {
        yield return new WaitForSecondsRealtime(0.0001f);

        canvas.worldCamera = Camera.main;
        FindPlayers();
    }
}
