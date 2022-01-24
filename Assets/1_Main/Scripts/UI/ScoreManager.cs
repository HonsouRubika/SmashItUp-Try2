using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public float TimerNextMap = 3;
    private float TimerNextMapActu = 0;
    public float waitBeforeAddPoints = 0;
    private float timerBeforeAddPoints = 0;
    public float waitBeforeUpdateScore = 0;
    private float timerBeforeUpdateScore = 0;

    private bool flagAddedPoints = false;
    private bool flagUpdateScore = false;

    [Space]
    //P1
    public Transform P1ScoreNumber;
    private TextMeshProUGUI P1ScoreUnits;
    private TextMeshProUGUI P1ScoreTens;
    private TextMeshProUGUI P1ScoreHundreds;

    //P2
    public Transform P2ScoreNumber;
    private TextMeshProUGUI P2ScoreUnits;
    private TextMeshProUGUI P2ScoreTens;
    private TextMeshProUGUI P2ScoreHundreds;

    //P3
    public Transform P3ScoreNumber;
    private TextMeshProUGUI P3ScoreUnits;
    private TextMeshProUGUI P3ScoreTens;
    private TextMeshProUGUI P3ScoreHundreds;

    //P4
    public Transform P4ScoreNumber;
    private TextMeshProUGUI P4ScoreUnits;
    private TextMeshProUGUI P4ScoreTens;
    private TextMeshProUGUI P4ScoreHundreds;

    public List<GameObject> playerAddedPoints;
    private List<TextMeshProUGUI> playerAddedPointsText;

    public List<Transform> tpPoints = new List<Transform>();

    public GameObject rollingBoard;
    public List<GameObject> scoreBoard;

    public GameObject bonusBanner;
    public GameObject bonusText;
    private bool isBonus = false;

    public SpinnerSound SpinnerSoundScript;

    [Header("Crowns")]
    public SpriteRenderer crownP1;
    public SpriteRenderer crownP2;
    public SpriteRenderer crownP3;
    public SpriteRenderer crownP4;

    [Space]
    public Sprite crownGold;
    public Sprite crownSilver;
    public Sprite crownBronze;

    public int[] scorePlayers;

    private GameObject[] playersUnsorted;
    private GameObject[] players;
    public int[] playersPosition;

    //Equality
    public EqualityCase equalityCase = EqualityCase.None;

    private bool allScoreZero = true;

    [Header("Display Score")]
    public GameObject[] hideP3Objects;
    public GameObject[] hideP4Objects;


    private void Start()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        scorePlayers = new int[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        //crowns
        crownP1.sprite = null;
        crownP2.sprite = null;
        crownP3.sprite = null;
        crownP4.sprite = null;

        //bonus
        if (GameManager.Instance._nbMancheActu == GameManager.Instance.BonusRound)
        {
            //GameManager.Instance.bonusManagerScript.ApplyBonusInGame();
            isBonus = true;
        }
        else if (GameManager.Instance._nbMancheActu == GameManager.Instance.BonusRound + 1)
        {
            //bonusManagerScript.DisableBonusInGame();
        }
        bonusBanner.SetActive(false);
        bonusText.SetActive(false);

        SpawnPlayer();
        FillPlayerScoreValues();
        DisplayPlayersScore();
        SortPlayers();
        DisplayScoreDependingPlayers();

        playerAddedPointsText = new List<TextMeshProUGUI>(new TextMeshProUGUI[playerAddedPoints.Count]);
        for (int i = 0; i < playerAddedPoints.Count; i++)
        {
            playerAddedPoints[i].SetActive(false);
            playerAddedPointsText[i] = playerAddedPoints[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        for (int i = 0; i < playerAddedPointsText.Count; i++)
        {
            if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
            {
                playerAddedPointsText[i].text = "+ " + GameManager.Instance.getAddedPointsPlayer(i + 1);
            }  
        }

        TimerNextMapActu = Time.time + TimerNextMap;
        timerBeforeAddPoints = Time.time + waitBeforeAddPoints;
        timerBeforeUpdateScore = Time.time + waitBeforeUpdateScore;
    }

    private void Update()
    {
        if (Time.time >= timerBeforeAddPoints && !flagAddedPoints)
        {
            SpinnerSoundScript.ScoreSpinning();

            flagAddedPoints = true;
            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
                {
                    playerAddedPoints[i].SetActive(true);
                    scoreBoard[i].SetActive(false);

                    switch (i)
                    {
                        case 0:
                            P1ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 1:
                            P2ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 2:
                            P3ScoreNumber.gameObject.SetActive(false);
                            break;
                        case 3:
                            P4ScoreNumber.gameObject.SetActive(false);
                            break;

                    }
                }    
            }
            rollingBoard.SetActive(true);
        }

        if (Time.time >= timerBeforeUpdateScore && !flagUpdateScore)
        {
            flagUpdateScore = true;

            GameManager.Instance.UpdatePlayerScore();
            DisplayPlayersScore();
            SortPlayers();

            for (int i = 0; i < playerAddedPoints.Count; i++)
            {
                playerAddedPoints[i].SetActive(false);
                scoreBoard[i].SetActive(true);
            }

            switch (GameManager.Instance.players.Length)
            {
                case 2:
                    P1ScoreNumber.gameObject.SetActive(true);
                    P2ScoreNumber.gameObject.SetActive(true);
                    break;
                case 3:
                    P1ScoreNumber.gameObject.SetActive(true);
                    P2ScoreNumber.gameObject.SetActive(true);
                    P3ScoreNumber.gameObject.SetActive(true);
                    break;
                case 4:
                    P1ScoreNumber.gameObject.SetActive(true);
                    P2ScoreNumber.gameObject.SetActive(true);
                    P3ScoreNumber.gameObject.SetActive(true);
                    P4ScoreNumber.gameObject.SetActive(true);
                    break;    
            }

            rollingBoard.SetActive(false);

            SpinnerSoundScript.EndSpinning();
        } 

        if (Time.time >= TimerNextMapActu)
        {
            //Next Map
            if (GameManager.Instance._nbMancheActu < GameManager.Instance._nbManches && !isBonus)
            {
                //end display bonus
                bonusBanner.SetActive(false);
                bonusText.SetActive(false);

                GameManager.Instance.NextMap();
                GameManager.Instance.resetScorePoints();
            }
            else if (!isBonus)
            {
                //end display bonus
                bonusBanner.SetActive(false);
                bonusText.SetActive(false);

                GameManager.Instance.FinaleScore();
                GameManager.Instance.resetScorePoints();
            }
            else
            {
                isBonus = false;
                //bonus end game
                GameManager.Instance.bonusManagerScript.ApplyBonusEndGame(0);

                //display is bonus
                bonusBanner.SetActive(true);
                bonusText.SetActive(true);

                //reset var
                flagAddedPoints = false;
                flagUpdateScore = false;

                //UI
                FillPlayerScoreValues();
                DisplayPlayersScore();
                SortPlayers();

                playerAddedPointsText = new List<TextMeshProUGUI>(new TextMeshProUGUI[playerAddedPoints.Count]);
                for (int i = 0; i < playerAddedPoints.Count; i++)
                {
                    playerAddedPoints[i].SetActive(false);
                    playerAddedPointsText[i] = playerAddedPoints[i].GetComponentInChildren<TextMeshProUGUI>();
                }

                for (int i = 0; i < playerAddedPointsText.Count; i++)
                {
                    if (GameManager.Instance.getAddedPointsPlayer(i + 1) != 0)
                    {
                        playerAddedPointsText[i].text = "+ " + GameManager.Instance.getAddedPointsPlayer(i + 1);
                    }
                }

                TimerNextMapActu = Time.time + TimerNextMap;
                timerBeforeAddPoints = Time.time + waitBeforeAddPoints;
                timerBeforeUpdateScore = Time.time + waitBeforeUpdateScore;
            }
            
        }
    }

    private void FillPlayerScoreValues()
    {
        P1ScoreHundreds = P1ScoreNumber.GetChild(0).GetComponent<TextMeshProUGUI>();
        P1ScoreTens = P1ScoreNumber.GetChild(1).GetComponent<TextMeshProUGUI>();
        P1ScoreUnits = P1ScoreNumber.GetChild(2).GetComponent<TextMeshProUGUI>();

        P2ScoreHundreds = P2ScoreNumber.GetChild(0).GetComponent<TextMeshProUGUI>();
        P2ScoreTens = P2ScoreNumber.GetChild(1).GetComponent<TextMeshProUGUI>();
        P2ScoreUnits = P2ScoreNumber.GetChild(2).GetComponent<TextMeshProUGUI>();

        P3ScoreHundreds = P3ScoreNumber.GetChild(0).GetComponent<TextMeshProUGUI>();
        P3ScoreTens = P3ScoreNumber.GetChild(1).GetComponent<TextMeshProUGUI>();
        P3ScoreUnits = P3ScoreNumber.GetChild(2).GetComponent<TextMeshProUGUI>();

        P4ScoreHundreds = P4ScoreNumber.GetChild(0).GetComponent<TextMeshProUGUI>();
        P4ScoreTens = P4ScoreNumber.GetChild(1).GetComponent<TextMeshProUGUI>();
        P4ScoreUnits = P4ScoreNumber.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void DisplayPlayersScore()
    {
        P1ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(1, "units").ToString();
        P1ScoreTens.text = GameManager.Instance.getDividedScorePlayer(1, "tens").ToString();
        P1ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(1, "hundreds").ToString();

        P2ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(2, "units").ToString();
        P2ScoreTens.text = GameManager.Instance.getDividedScorePlayer(2, "tens").ToString();
        P2ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(2, "hundreds").ToString();

        P3ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(3, "units").ToString();
        P3ScoreTens.text = GameManager.Instance.getDividedScorePlayer(3, "tens").ToString();
        P3ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(3, "hundreds").ToString();

        P4ScoreUnits.text = GameManager.Instance.getDividedScorePlayer(4, "units").ToString();
        P4ScoreTens.text = GameManager.Instance.getDividedScorePlayer(4, "tens").ToString();
        P4ScoreHundreds.text = GameManager.Instance.getDividedScorePlayer(4, "hundreds").ToString();
    }

    public void setHardScore(int playerID, int score)
    {
        switch (playerID)
        {
            case 1:
                P1ScoreUnits.text = (score % 10).ToString();
                P1ScoreTens.text = ((score / 10) % 10).ToString();
                P1ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 2:
                P2ScoreUnits.text = (score % 10).ToString();
                P2ScoreTens.text = ((score / 10) % 10).ToString();
                P2ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 3:
                P3ScoreUnits.text = (score % 10).ToString();
                P3ScoreTens.text = ((score / 10) % 10).ToString();
                P3ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            case 4:
                P4ScoreUnits.text = (score % 10).ToString();
                P4ScoreTens.text = ((score / 10) % 10).ToString();
                P4ScoreHundreds.text = ((score / 100) % 10).ToString();
                GameManager.Instance._scoreP1 = score;
                break;
            default:
                Debug.Log("error UI score");
                break;
        }
    }

    private void SpawnPlayer()
    {
        for (int i = 0; i < playersUnsorted.Length; i++)
        {
            playersUnsorted[i].transform.position = tpPoints[i].position;
        }
    }

    private void SetCrownToPlayers(int player, Sprite crown)
    {
        switch (player)
        {
            case 1:
                crownP1.sprite = crown;
                break;
            case 2:
                crownP2.sprite = crown;
                break;
            case 3:
                crownP3.sprite = crown;
                break;
            case 4:
                crownP4.sprite = crown;
                break;
        }
    }

    private void SortPlayers()
    {
        //VIDER LE TABLEAU SCORE PLAYERS
        switch (scorePlayers.Length)
        {
            case 2:
                scorePlayers[0] = GameManager.Instance.getScorePlayer(1);
                scorePlayers[1] = GameManager.Instance.getScorePlayer(2);
                break;
            case 3:
                scorePlayers[0] = GameManager.Instance.getScorePlayer(1);
                scorePlayers[1] = GameManager.Instance.getScorePlayer(2);
                scorePlayers[2] = GameManager.Instance.getScorePlayer(3);
                break;
            case 4:
                scorePlayers[0] = GameManager.Instance.getScorePlayer(1);
                scorePlayers[1] = GameManager.Instance.getScorePlayer(2);
                scorePlayers[2] = GameManager.Instance.getScorePlayer(3);
                scorePlayers[3] = GameManager.Instance.getScorePlayer(4);
                break;
        }

        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }

        System.Array.Sort(scorePlayers, playersPosition);

        CheckIfEquality();

        switch (playersPosition.Length)
        {
            case 4:
                switch (equalityCase)
                {
                    case EqualityCase.None:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownBronze);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, null);
                        break;
                    case EqualityCase.AllEqualDifferentZero:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownGold);
                        break;
                    case EqualityCase.FirstSecondAndThirdFourth:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownSilver);
                        break;
                    case EqualityCase.FirstSecondThird:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownSilver);
                        break;
                    case EqualityCase.SecondThirdFourth:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownSilver);
                        break;
                    case EqualityCase.FirstSecond:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownBronze);
                        break;
                    case EqualityCase.SecondThird:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownBronze);
                        break;
                    case EqualityCase.ThirdFourth:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownBronze);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 4] + 1, crownBronze);
                        break;
                }
                break;
            case 3:
                switch (equalityCase)
                {
                    case EqualityCase.None:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownBronze);
                        break;
                    case EqualityCase.AllEqualDifferentZero:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownGold);
                        break;
                    case EqualityCase.FirstSecond:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        break;
                    case EqualityCase.SecondThird:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 3] + 1, crownSilver);
                        break;
                }
                break;
            case 2:
                switch (equalityCase)
                {
                    case EqualityCase.None:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownSilver);
                        break;
                    case EqualityCase.AllEqualDifferentZero:
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 1] + 1, crownGold);
                        SetCrownToPlayers(playersPosition[playersPosition.Length - 2] + 1, crownGold);
                        break;
                }
                break;
        }
    }

    private void CheckIfEquality()
    {
        //Check if no players have points
        for (int i = 0; i < scorePlayers.Length; i++)
        {
            if (scorePlayers[i] != 0)
            {
                allScoreZero = false;
                break;
            }
        }

        //Check if equality between players
        for (int i = 0; i < scorePlayers.Length; i++)
        {
            if (scorePlayers.Length == 4)
            {
                if (i == 3)
                {
                    if (scorePlayers[i] == scorePlayers[i - 1])
                    {
                        //equality between 1st & 2nd
                        equalityCase = EqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                }
                else if (i == 2)
                {
                    if (scorePlayers[i] == scorePlayers[i - 1])
                    {
                        //equality between 2st & 3rd
                        equalityCase = EqualityCase.SecondThird;
                        //Debug.Log("equality between 2st & 3rd");
                    }
                }
                else if (i == 1)
                {
                    if (scorePlayers[i] == scorePlayers[i + 2] && scorePlayers[i] == scorePlayers[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (scorePlayers[i] == scorePlayers[i + 2])
                    {
                        //equality between 1st & 2nd & 3rd
                        equalityCase = EqualityCase.FirstSecondThird;
                        //Debug.Log("equality between 1st & 2nd & 3rd");
                        break;
                    }
                    else if (scorePlayers[i] == scorePlayers[i - 1] && scorePlayers[i] == scorePlayers[i + 1])
                    {
                        //equality between 2st & 3rd & 4th
                        equalityCase = EqualityCase.SecondThirdFourth;
                        //Debug.Log("equality between 2st & 3rd & 4th");
                        break;
                    }
                    else if (scorePlayers[i] == scorePlayers[i - 1])
                    {
                        if (scorePlayers[i + 2] == scorePlayers[(i + 2) - 1])
                        {
                            //equality between 1st & 2nd and 3rd & 4th
                            equalityCase = EqualityCase.FirstSecondAndThirdFourth;
                            //Debug.Log("equality between 1st & 2nd and 3rd & 4th");
                            break;
                        }
                        else
                        {
                            //equality between 3rd & 4th
                            equalityCase = EqualityCase.ThirdFourth;
                            //Debug.Log("equality between 3rd & 4th");
                        }
                    }
                }
                else
                {
                    //no equality
                    equalityCase = EqualityCase.None;
                }
            }
            else if (scorePlayers.Length == 3)
            {
                if (i == 1)
                {
                    if (scorePlayers[i] == scorePlayers[i - 1] && scorePlayers[i] == scorePlayers[i + 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else if (scorePlayers[i] == scorePlayers[i + 1])
                    {
                        //equality between 1st & 2nd
                        equalityCase = EqualityCase.FirstSecond;
                        //Debug.Log("equality between 1st & 2nd");
                    }
                    else if (scorePlayers[i] == scorePlayers[i - 1])
                    {
                        //equality between 2st & 3rd
                        equalityCase = EqualityCase.SecondThird;
                        //Debug.Log("equality between 2st & 3rd");
                    }
                    else
                    {
                        //no equality
                        equalityCase = EqualityCase.None;
                    }
                }
            }
            else if (scorePlayers.Length == 2)
            {
                if (i == 1)
                {
                    if (scorePlayers[i] == scorePlayers[i - 1])
                    {
                        if (allScoreZero)
                        {
                            //equality between all players
                            equalityCase = EqualityCase.AllEqual;
                            //Debug.Log("equality between all players");
                            break;
                        }
                        else
                        {
                            //equality between all players different from 0
                            equalityCase = EqualityCase.AllEqualDifferentZero;
                            //Debug.Log("equality between all players different from 0");
                            break;
                        }
                    }
                    else
                    {
                        //no equality
                        equalityCase = EqualityCase.None;
                    }
                }
            }
        }
    }

    public enum EqualityCase
    {
        None,
        AllEqual,
        AllEqualDifferentZero,
        FirstSecondThird,
        SecondThirdFourth,
        FirstSecond,
        ThirdFourth,
        SecondThird,
        FirstSecondAndThirdFourth
    }

    private void DisplayScoreDependingPlayers()
    {
        switch (GameManager.Instance.players.Length)
        {
            case 2:
                hideP3Objects[0].SetActive(true);
                for (int i = 1; i < hideP3Objects.Length; i++)
                {
                    hideP3Objects[i].SetActive(false);
                }

                hideP4Objects[0].SetActive(true);
                for (int i = 1; i < hideP4Objects.Length; i++)
                {
                    hideP4Objects[i].SetActive(false);
                }
                break;
            case 3:
                hideP4Objects[0].SetActive(true);
                for (int i = 1; i < hideP4Objects.Length; i++)
                {
                    hideP4Objects[i].SetActive(false);
                }
                break;
        }
    }
}
