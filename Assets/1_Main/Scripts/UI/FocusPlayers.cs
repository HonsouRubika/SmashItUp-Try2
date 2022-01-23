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

    [Header("Display Team Compo")]
    public SpriteRenderer[] skinPlayers;
    public SpriteRenderer[] playersNumber;
    public GameObject[] playersDisplay;
    public Sprite[] skinsSprite;
    public Sprite[] numberSprite;
    public Transform[] oneVSThreePlayerDisplayCoordonate;
    public Transform[] twoVSTwoPlayerDisplayCoordonate;
    public GameObject VS;
    private int[] playersTeam; //from GameManager


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

        playersTeam = GameManager.Instance.playersTeam;
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

        ///////  affichage equipes  ///////
        //num des players :
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                //no change
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                //equipe 1
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 0) playersNumber[0].sprite = numberSprite[0];
                }
                //equipe 2
                int cursor1v3 = 1;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 1)
                    {
                        playersNumber[cursor1v3].sprite = numberSprite[cursor1v3];
                        cursor1v3++;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                //equipe 1
                int cursor1 = 0;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 0)
                    {
                        playersNumber[cursor1].sprite = numberSprite[cursor1];
                        cursor1++;
                    }
                }
                //equipe 2
                int cursor2 = 2;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 1)
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

        //on desactive pour les players vacants
        for (int i = playersTeam.Length; i < 4; i++)
        {
            playersDisplay[i].SetActive(false);
        }

        //display skin du bon player
        for (int i = 0; i< playersTeam.Length; i++)
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
                case "Mashmaboy":
                    skinPlayers[i].sprite = skinsSprite[3];
                    break;
                case "TakoTako":
                    skinPlayers[i].sprite = skinsSprite[4];
                    break;
                case "DevilPrincess":
                    skinPlayers[i].sprite = skinsSprite[5];
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
                //no change in players placement
                //add FFA display

                //hide VS sprite
                VS.SetActive(false);
                break;
            case (int)GameManager.TeamCompo.OneVSThree:
                //display VS sprite
                VS.SetActive(true);
                //equipe 1
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 0) playersDisplay[j].transform.position = new Vector2( oneVSThreePlayerDisplayCoordonate[0].position.x, oneVSThreePlayerDisplayCoordonate[0].position.y);
                }
                //equipe 2
                int cursor = 1;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 1)
                    {
                        playersDisplay[j].transform.position = new Vector2(oneVSThreePlayerDisplayCoordonate[cursor].position.x, oneVSThreePlayerDisplayCoordonate[cursor].position.y);
                        cursor++;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.TwoVSTwo:
                //display VS sprite
                VS.SetActive(true);
                //equipe 1
                int cursor1 = 0;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 0)
                    {
                        playersDisplay[j].transform.position = new Vector2(twoVSTwoPlayerDisplayCoordonate[cursor1].position.x, twoVSTwoPlayerDisplayCoordonate[cursor1].position.y);
                        cursor1++;
                    }
                }
                //equipe 2
                int cursor2 = 2;
                for (int j = 0; j < playersTeam.Length; j++)
                {
                    if (playersTeam[j] == 1)
                    {
                        playersDisplay[j].transform.position = new Vector2(twoVSTwoPlayerDisplayCoordonate[cursor2].position.x, twoVSTwoPlayerDisplayCoordonate[cursor2].position.y);
                        cursor2++;
                    }
                }
                break;
            case (int)GameManager.TeamCompo.Coop:
                //no change in players placement
                //hide VS sprite
                VS.SetActive(false);
                //add COOP display
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
