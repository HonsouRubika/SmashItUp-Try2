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

    [Header("Mini-game title")]
    public SpriteRenderer[] skinPlayers;
    public SpriteRenderer[] playersNumber;
    public GameObject[] playersDisplay;
    public Sprite[] skinsSprite;
    public Sprite[] numberSprite;

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

        //affichage equipes
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                //no change
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                //equipe 1
                for (int j = 0; j < GameManager.Instance.playersTeam.Length; j++)
                {
                    if (GameManager.Instance.playersTeam[j] == 0) playersNumber[0].sprite = numberSprite[j];
                }
                //equipe 2
                int cursor = 1;
                for (int j = 0; j < GameManager.Instance.playersTeam.Length; j++)
                {
                    if (GameManager.Instance.playersTeam[j] == 1)
                    {
                        playersNumber[cursor].sprite = numberSprite[j];
                        cursor++;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                //equipe 1
                int cursor1 = 0;
                for (int j = 0; j < GameManager.Instance.playersTeam.Length; j++)
                {
                    cursor1++;
                    if (GameManager.Instance.playersTeam[j] == 0) playersNumber[cursor1].sprite = numberSprite[j];
                }
                //equipe 2
                int cursor2 = 2;
                for (int j = 0; j < GameManager.Instance.playersTeam.Length; j++)
                {
                    if (GameManager.Instance.playersTeam[cursor2] == 1)
                    {
                        playersNumber[cursor2].sprite = numberSprite[cursor2];
                        cursor2++;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.Coop:
                //no change
                break;
        }
        for (int i = 0; i< GameManager.Instance.playersTeam.Length; i++)
        {
            switch (players[i].GetComponent<PlayerSkins>().currentSkin.name)
            {
                case "KamékinV2":
                    skinPlayers[i].sprite = skinsSprite[0];
                    break;
                case "LaptiteFraise":
                    skinPlayers[i].sprite = skinsSprite[1];
                    break;
                case "Pouletto":
                    skinPlayers[i].sprite = skinsSprite[2];
                    break;
                default:
                    Debug.LogWarning("New sprite needs to be linked in script FocusPlayers");
                    skinPlayers[i].sprite = skinsSprite[0];
                    break;
            }
        }
        
        //set position of players elements
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                for (int i = 0; i < GameManager.Instance.playersTeam.Length; i++)
                {

                }
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                for (int i = 0; i < GameManager.Instance.playersTeam.Length; i++)
                {

                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:

                break;
            case (int)GameManager.TeamCompo.Coop:

                break;
        }
        

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
