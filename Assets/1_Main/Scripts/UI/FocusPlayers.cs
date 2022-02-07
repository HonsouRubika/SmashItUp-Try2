using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPlayers : MonoBehaviour
{
    public GameObject[] players;

    [Header("Focus Settings")]
    public float timeShowingPlayers;
    private float timeShowingPlayersActu;

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
    public Sprite destroyCratesSprite;

    [Header("Display Team Compo")]
    public SpriteRenderer[] skinPlayers;
    public SpriteRenderer[] playersNumber;
    public GameObject[] playersDisplay;
    //public Sprite[] skinsSprite;
    public Sprite[] kamekinSprite;
    public Sprite[] pouletoSprite;
    public Sprite[] fraiseSprite;
    public Sprite[] cuppySprite;
    public Sprite[] numberSprite;
    public Transform[] oneVSThreePlayerDisplayCoordonate;
    public Transform[] twoVSTwoPlayerDisplayCoordonate;
    public Transform[] FFAPlayerDisplayCoordonate;
    public GameObject VS;
    public GameObject VS2;
    public GameObject VS3;
    private int[] playersTeam; //from GameManager


    private Canvas canvas;

    private void Start()
    {
        hideScreen.GetComponent<SpriteRenderer>().color = colorHideScreen;
        canvas = canvasRef.GetComponent<Canvas>();

        VS2.SetActive(false);
        VS3.SetActive(false);

        GameManager.Instance.isShowingPlayers = false;
    }

    private void Update()
    {
        if (GameManager.Instance.isShowingPlayers && Time.time > timeShowingPlayersActu)
        {
            //DestroyCercle();
            canvasRef.SetActive(false);
            GameManager.Instance.isShowingPlayers = false;
        }
    }

    public void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        cercles = new GameObject[players.Length];
    }

    public void EnableFocus()
    {
        //StartCoroutine(GetReference());
        //StartCoroutine(IncrementTimer());
        playersTeam = GameManager.Instance.playersTeam;
        GetReference();
        IncrementTimer();
        
    }

    private void SpawnCercle()
    {
        canvasRef.SetActive(true);
        //Time.timeScale = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            cercles[i] = Instantiate(cerclePrefab, players[i].transform.position, Quaternion.identity);
        }
    }

    private void DestroyCercle()
    {
        canvasRef.SetActive(false);
        //Time.timeScale = 1f;

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

    public void IncrementTimer()
    {
        //SpawnCercle();
        canvasRef.SetActive(true);

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
                case "KamékinMulticolor(Clone)":
                    skinPlayers[i].sprite = kamekinSprite[i];
                    break;
                case "LaptiteFraise(Clone)":
                    skinPlayers[i].sprite = fraiseSprite[i];
                    break;
                case "Pouletto(Clone)":
                    skinPlayers[i].sprite = pouletoSprite[i];
                    break;
                case "Cuppy(Clone)":
                    skinPlayers[i].sprite = cuppySprite[i];
                    break;
                case "TakoTako(Clone)":
                    skinPlayers[i].sprite = kamekinSprite[i];
                    break;
                case "DevilPrincess(Clone)":
                    skinPlayers[i].sprite = kamekinSprite[i];
                    break;
                default:
                    Debug.LogWarning("New sprite needs to be linked in script FocusPlayers");
                    skinPlayers[i].sprite = kamekinSprite[i];
                    break;
            }
        }
        
        //set position of players elements
        switch (GameManager.Instance.getTeamCompo())
        {
            case (int)GameManager.TeamCompo.FFA:
                //no change in players placement
                for (int j = 0; j < playersTeam.Length; j++)
                {   
                    playersDisplay[j].transform.position = new Vector2(FFAPlayerDisplayCoordonate[j].position.x, FFAPlayerDisplayCoordonate[j].position.y);
                }
                //add FFA display

                //hide VS sprite
                if(players.Length > 1)
                {
                    VS.SetActive(true);
                    VS2.SetActive(false);
                }
                else
                {
                    VS.SetActive(false);
                }
                if (playersTeam.Length > 2)
                {
                    VS2.SetActive(true);
                    VS.SetActive(true);
                }
                if (playersTeam.Length > 3)
                {
                    VS3.SetActive(true);
                    VS2.SetActive(true);
                }
                break;

            case (int)GameManager.TeamCompo.OneVSThree:
                //display VS sprite
                VS.SetActive(true);
                VS2.SetActive(false);
                VS3.SetActive(false);
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
                VS2.SetActive(false);
                VS3.SetActive(false);
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
                VS.SetActive(false);
                VS2.SetActive(false);
                VS3.SetActive(false);
                //hide VS sprite
                VS.SetActive(false);
                //add COOP display
                break;
        }
        timeShowingPlayersActu = Time.time + timeShowingPlayers;
        GameManager.Instance.isShowingPlayers = true;
    }

    public void GetReference()
    {
        canvas.worldCamera = Camera.main;
        FindPlayers();
    }
}
