using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class NewScoreSystem : MonoBehaviour
{
    //New score system
    [Header("Score System")]
    public float scoreStaysTime;
    public float timeBeforeFirstIncrementation;
    public float timeBtwPops;

    private bool displayScore = false;

    private float timerScoreStays = 0;

    [Header("Points")]
    public Sprite hammerPointBronze;
    public Sprite hammerPointSilver;
    public Sprite hammerPointGold;

    [Header("Team")]
    public Sprite purpleCursor;
    public Sprite orangeCursor;
    private Image[] teamCursor;

    [Header("Canvas")]
    public GameObject ScorePanel;
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;
    private int p1PointIndex = 0;
    private int p2PointIndex = 0;
    private int p3PointIndex = 0;
    private int p4PointIndex = 0;

    private Image[] P1Points;
    private Image[] P2Points;
    private Image[] P3Points;
    private Image[] P4Points;

    private GameObject score4Players;
    private GameObject score3Players;
    private GameObject score2Players;

    private  int[] scorePlayers;
    private GameObject[] playersUnsorted;
    private GameObject[] players;
    private int[] playersPosition;

    //Equality
    private EqualityCase equalityCase = EqualityCase.None;

    private bool allScoreZero = true;

    private ScoreValuesManager scoreValuesScript;

    public EarnPointSound EarnPointSoundScript;

    private void Start()
    {
        teamCursor = new Image[ScorePanel.transform.GetChild(8).childCount];
        for (int i = 0; i < teamCursor.Length; i++)
        {
            teamCursor[i] = ScorePanel.transform.GetChild(8).GetChild(i).GetComponent<Image>();
        }
        EnableTeamCursor(false);

        scoreValuesScript = GetComponent<ScoreValuesManager>();
        score4Players = ScorePanel.transform.GetChild(1).gameObject;
        score3Players = ScorePanel.transform.GetChild(2).gameObject;
        score2Players = ScorePanel.transform.GetChild(3).gameObject;

        ScorePanel.SetActive(false);
        DisplayScoreDependingPlayersNumber(false);

        P1Points = new Image[P1.transform.childCount];
        for (int i = 0; i < P1Points.Length; i++)
        {
            P1Points[i] = P1.transform.GetChild(i).GetComponent<Image>();
        }
        P2Points = new Image[P2.transform.childCount];
        for (int i = 0; i < P2Points.Length; i++)
        {
            P2Points[i] = P2.transform.GetChild(i).GetComponent<Image>();
        }
        P3Points = new Image[P3.transform.childCount];
        for (int i = 0; i < P3Points.Length; i++)
        {
            P3Points[i] = P3.transform.GetChild(i).GetComponent<Image>();
        }
        P4Points = new Image[P4.transform.childCount];
        for (int i = 0; i < P4Points.Length; i++)
        {
            P4Points[i] = P4.transform.GetChild(i).GetComponent<Image>();
        }

       // EarnPointSoundScript.PlayerEarnPoint();
    }

    private void Update()
    {
        if (timerScoreStays < scoreStaysTime && displayScore)
        {
            timerScoreStays += Time.deltaTime;
        }

        if (timerScoreStays > scoreStaysTime && displayScore)
        {
            DisplayScore(false);
            displayScore = false;
        }

    }

    public void DisplayScore(bool enable)
    {
        switch (enable)
        {
            case true:
                displayScore = true;
                ScorePanel.SetActive(true);
                timerScoreStays = 0;
                FillPlayersList();
                SortPlayers();
                DisplayScoreDependingPlayersNumber(true);
                StartCoroutine(DistributePoints());
                break;
            case false:
                displayScore = false;
                ScorePanel.SetActive(false);
                DisplayScoreDependingPlayersNumber(false);
                GameManager.Instance.UpdatePlayerScore();
                GameManager.Instance.resetScorePoints();
                GameManager.Instance.FadeOutTransition();
                break;
        }
    }

    private IEnumerator DistributePoints()
    {
        yield return new WaitForSeconds(timeBeforeFirstIncrementation);

        AddPoints(GameManager.Instance.getAddedPointsPlayer(1), P1Points, p1PointIndex);
    }

    private void AddPoints(int addedPoints, Image[] points, int index)
    {
        StartCoroutine(SetHammerPoints(addedPoints, points, index));
    }

    private IEnumerator SetHammerPoints(int addedPoints, Image[] points, int index)
    {
        switch (playersPosition.Length)
        {
            case 4:
                if (addedPoints == scoreValuesScript.FourPlayersPointsFirstPlace)
                {
                    for (int i = 0; i < scoreValuesScript.FourPlayersPointsFirstPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.FourPlayersPointsSecondPlace)
                {
                    for (int i = 0; i < scoreValuesScript.FourPlayersPointsSecondPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.FourPlayersPointsThirdPlace)
                {
                    for (int i = 0; i < scoreValuesScript.FourPlayersPointsThirdPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.FourPlayersPointsFourthPlace)
                {
                    for (int i = 0; i < scoreValuesScript.FourPlayersPointsFourthPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }

                if (points == P1Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(2), P2Points, p2PointIndex)); p1PointIndex = index; }
                else if (points == P2Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(3), P3Points, p3PointIndex)); p2PointIndex = index; }
                else if (points == P3Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(4), P4Points, p4PointIndex)); p3PointIndex = index; }
                else if (points == P4Points) p4PointIndex = index;
                break;
            case 3:
                if (addedPoints == scoreValuesScript.ThreePlayersPointsFirstPlace)
                {
                    for (int i = 0; i < scoreValuesScript.ThreePlayersPointsFirstPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.ThreePlayersPointsSecondPlace)
                {
                    for (int i = 0; i < scoreValuesScript.ThreePlayersPointsSecondPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.ThreePlayersPointsThirdPlace)
                {
                    for (int i = 0; i < scoreValuesScript.ThreePlayersPointsThirdPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }

                if (points == P1Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(2), P2Points, p2PointIndex)); p1PointIndex = index; }
                else if (points == P2Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(3), P3Points, p3PointIndex)); p2PointIndex = index; }
                else if (points == P3Points) p3PointIndex = index;
                break;
            case 2:
                if (addedPoints == scoreValuesScript.TwoPlayersPointsFirstPlace)
                {
                    for (int i = 0; i < scoreValuesScript.TwoPlayersPointsFirstPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }
                else if (addedPoints == scoreValuesScript.TwoPlayersSecondPlace)
                {
                    for (int i = 0; i < scoreValuesScript.TwoPlayersSecondPlace; i++)
                    {
                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].sprite = hammerPointGold;
                        else if (index > points.Length - 1) points[index - points.Length].sprite = hammerPointSilver;
                        else points[index].sprite = hammerPointBronze;

                        if (index > (points.Length * 2) - 1) points[index - (points.Length * 2)].color = new Color(1, 1, 1, 1);
                        else if (index > points.Length - 1) points[index - points.Length].color = new Color(1, 1, 1, 1);
                        else points[index].color = new Color(1, 1, 1, 1);

                        index++;

                        yield return new WaitForSeconds(timeBtwPops);
                    }
                }

                if (points == P1Points) { StartCoroutine(SetHammerPoints(GameManager.Instance.getAddedPointsPlayer(2), P2Points, p2PointIndex)); p1PointIndex = index; }
                else if (points == P2Points) p2PointIndex = index;
                break;
        }
    }

    public void DisplayTeam(int player, string team)
    {
        switch (team)
        {
            case "purple":
                switch (player)
                {
                    case 0:
                        teamCursor[0].sprite = purpleCursor;
                        break;
                    case 1:
                        teamCursor[1].sprite = purpleCursor;
                        break;
                    case 2:
                        teamCursor[2].sprite = purpleCursor;
                        break;
                    case 3:
                        teamCursor[3].sprite = purpleCursor;
                        break;
                }
                break;
            case "orange":
                switch (player)
                {
                    case 0:
                        teamCursor[0].sprite = orangeCursor;
                        break;
                    case 1:
                        teamCursor[1].sprite = orangeCursor;
                        break;
                    case 2:
                        teamCursor[2].sprite = orangeCursor;
                        break;
                    case 3:
                        teamCursor[3].sprite = orangeCursor;
                        break;
                }
                break;
        }

        EnableTeamCursor(true);
    }

    public void EnableTeamCursor(bool enable)
    {
        switch (enable)
        {
            case true:
                for (int i = 0; i < teamCursor.Length; i++)
                {
                    teamCursor[i].gameObject.SetActive(true);
                }
                break;
            case false:
                for (int i = 0; i < teamCursor.Length; i++)
                {
                    teamCursor[i].gameObject.SetActive(false);
                }
                break;
        }
    }

    private void FillPlayersList()
    {
        playersUnsorted = GameObject.FindGameObjectsWithTag("Player");
        players = playersUnsorted.OrderBy(go => go.name).ToArray();

        scorePlayers = new int[players.Length];
        playersPosition = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playersPosition[i] = i;
        }
    }

    private void DisplayScoreDependingPlayersNumber(bool enable)
    {
        if (enable)
        {
            switch (playersPosition.Length)
            {
                case 4:
                    score4Players.SetActive(true);
                    break;
                case 3:
                    score3Players.SetActive(true);
                    break;
                case 2:
                    score2Players.SetActive(true);
                    break;
            }
        }
        else
        {
            score4Players.SetActive(false);
            score3Players.SetActive(false);
            score2Players.SetActive(false);
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

        /*switch (equalityCase)
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
        }*/
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
}
