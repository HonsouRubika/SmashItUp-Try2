using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    private int player;
    private int selectedBonus;

    private int[] playersScore;
    private int[] playerPosition;

    /// <summary>
    /// TO:
    /// - attendre de jouer une certaine manche (disons la 3eme sur 5 manches)
    /// - choisir un bonus random parmis une liste de bonus
    /// - apply le bonus le temps de la partie
    /// </summary>

    void Start()
    {
        playersScore = new int[GameManager.Instance.getNbPlayer()];
        playerPosition = new int[GameManager.Instance.getNbPlayer()];
        for (int i = 0; i < playerPosition.Length; i++)
        {
            playerPosition[i] = i;
        }

        OrderPlayers();
    }

    public void ApplyBonusInGame()
    {
        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        //pick a random bonus
        selectedBonus = Random.Range(0, (int)BonusInGame.total);

        //get le joueur le plus nul
        player = 0;
        int playerScore = GameManager.Instance.getScorePlayer(0);

        for (int i = 0; i< GameManager.Instance.getNbPlayer(); i++)
        {
            if (GameManager.Instance.getScorePlayer(i) < playerScore)
            {
                playerScore = GameManager.Instance.getScorePlayer(i);
                player = i;
            }
        }

        //on apply l'effet sur le joueur (le temps de la game)
        switch (selectedBonus)
        {
            case (int)BonusInGame.Giga_Hit:
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().hammerXProjection *= 2;
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().hammerYProjection *= 2;
                break;
            case (int)BonusInGame.Unbreakable:
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().isUnbreakable = true;
                break;
        }
    }

    public void DisableBonusInGame()
    {
        switch (selectedBonus)
        {
            case (int)BonusInGame.Giga_Hit:
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().hammerXProjection /= 2;
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().hammerYProjection /= 2;
                break;
            case (int)BonusInGame.Unbreakable:
                GameManager.Instance.getSpecificPlayer(player).GetComponent<PlayerController>().isUnbreakable = true;
                break;
        }
    }

    public void ApplyBonusEndGame()
    {
        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        //pick a random bonus
        int selectedBonus = Random.Range(0, (int)BonusEndGame.total);

        //get le joueur le plus nul
        int worstPlayer = 0;
        int playerScore = GameManager.Instance.getScorePlayer(0);

        //get le meilleur joueur
        int bestPlayer = 0;
        int bestPlayerSpread = GameManager.Instance.getScorePlayer(0);

        //get des players avec les stats appropri�
        for (int i = 0; i < GameManager.Instance.getNbPlayer(); i++)
        {
            if (GameManager.Instance.getScorePlayer(i) < playerScore)
            {
                playerScore = GameManager.Instance.getScorePlayer(i);
                worstPlayer = i;
            }

            if (GameManager.Instance.getScorePlayer(i) > bestPlayerSpread)
            {
                bestPlayerSpread = GameManager.Instance.getScorePlayer(i);
                bestPlayer = i;
            }
        }

        //on apply l'effet sur le joueur (le temps de la game)
        switch (selectedBonus)
        {
            // WorstPlayer
            case (int)BonusEndGame.BackInTheGame:
                if (GameManager.Instance.getScorePlayer(playersScore[3]) < GameManager.Instance.getScorePlayer(playersScore[0]) + 30) // si la diff�rence entre le dernier et le premier joueur est inf�rieur � 30.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[3], 10); // add 10 points au pire joueur
                }   
                break;

            case (int)BonusEndGame.TakeThis:
                if (GameManager.Instance.getScorePlayer(playersScore[3]) < GameManager.Instance.getScorePlayer(playersScore[0]) + 50) // si la diff�rence entre le dernier et le premier joueur est inf�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[3], 20); // add 20 points au pire joueur
                }
                break;

            case (int)BonusEndGame.InExtremis:
                if (GameManager.Instance.getScorePlayer(playersScore[3]) > GameManager.Instance.getScorePlayer(playersScore[0]) + 50) // si la diff�rence entre le dernier et le premier joueur est sup�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[3], 30); // add 30 points au pire joueur
                }
                break;

            case (int)BonusEndGame.Duffer:
                if (GameManager.Instance.getScorePlayer(playersScore[3]) < 10) // si le score du dernier joueur est inf�rieur � 10.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[3], 40); // add 50 points au pire joueur
                }
                break;

            // Third Player
            case (int)BonusEndGame.StayInTheWake:
                if (GameManager.Instance.getScorePlayer(playersScore[2]) < GameManager.Instance.getScorePlayer(playersScore[1]) + 30) // si la diff�rence entre le troisi�me et le deuxi�me joueur est inf�rieur � 30.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[2], 20); // add 20 points au troisi�me joueur
                }
                break;

            case (int)BonusEndGame.BackOnTheTop:
                if (GameManager.Instance.getScorePlayer(playersScore[2]) < GameManager.Instance.getScorePlayer(playersScore[1]) + 50) // si la diff�rence entre le troisi�me et le deuxi�me joueur est inf�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[2], 50); // add 50 points au troisi�me joueur
                }
                break;

            case (int)BonusEndGame.ImStillHere:
                if (GameManager.Instance.getScorePlayer(playersScore[2]) > GameManager.Instance.getScorePlayer(playersScore[1]) + 50) // si la diff�rence entre le troisi�me et le deuxi�me joueur est sup�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[2], 70); // add 70 points au troisi�me joueur
                }
                break;

            // Second Player 
            case (int)BonusEndGame.StayInMyBack:
                if (GameManager.Instance.getScorePlayer(playersScore[1]) < GameManager.Instance.getScorePlayer(playersScore[2]) + 10) // si la diff�rence entre le deuxi�me et le trois�me joueur est inf�rieur � 10.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[1], 10); // add 10 points au deuxi�me joueur
                }
                break;

            case (int)BonusEndGame.RoadToTheFirstPlace:
                if (GameManager.Instance.getScorePlayer(playersScore[1]) < GameManager.Instance.getScorePlayer(playersScore[0]) + 50) // si la diff�rence entre le deuxi�me et le premier joueur est inf�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[1], 30); // add 30 points au deuxi�me joueur
                }
                break;

            case (int)BonusEndGame.IDontLikeDolphins:
                if (GameManager.Instance.getScorePlayer(playersScore[1]) > GameManager.Instance.getScorePlayer(playersScore[0]) + 50) // si la diff�rence entre le deuxi�me et le premier joueur est sup�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[1], 70); // add 70 points au deuxi�me joueur
                }
                break;

            // Best Player
            case (int)BonusEndGame.Bootlicker:
                if (GameManager.Instance.getScorePlayer(playersScore[0]) > GameManager.Instance.getScorePlayer(playersScore[0]) + 70) // si la diff�rence entre le premier et le deuxi�me joueur est sup�rieur � 70.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[0], -30); // enl�ve 30 points au meilleur joueur
                }
                break;

            case (int)BonusEndGame.ForeverBefore:
                if (GameManager.Instance.getScorePlayer(playersScore[0]) < GameManager.Instance.getScorePlayer(playersScore[0]) + 50) // si la diff�rence entre le premier et le deuxi�me joueur est inf�rieur � 50.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[0], 10); // add 10 points au meilleur joueur
                }
                break;

            case (int)BonusEndGame.ItsStartingToGetHot:
                if (GameManager.Instance.getScorePlayer(playersScore[0]) < GameManager.Instance.getScorePlayer(playersScore[0]) + 30) // si la diff�rence entre le premier et le deuxi�me joueur est inf�rieur � 30.
                {
                    GameManager.Instance.addSpecificScorePoints(playersScore[0], 20); // add 20 points au meilleur joueur
                }
                break;

        }

        //GameManager.Instance.UpdatePlayerScore();
    }

    void OrderPlayers()
    {
        for (int i = 0; i < GameManager.Instance.getNbPlayer(); i++)
        {
            playersScore[i] = GameManager.Instance.getScorePlayer(i);
        }

        System.Array.Sort(playersScore, playerPosition);

        //la liste playerPosition contient le rank des joueurs dans un ordre d�croissant
        //ex: playersScore = [2,1,4,3]
        /*
         * 1er = 2
         * 2eme = 1
         * 3eme = 4
         * 4eme = 3
        */

        //debug log
        //for (int i = 0; i < playersScore.Length; i++)
        //{
        //    Debug.Log(playerPosition[i]);
        //}
    }


    enum BonusInGame
    {
        Giga_Hit,
        Unbreakable,
        total
    }

    enum BonusEndGame
    {     
        // WorstPlayer
        Duffer,
        BackInTheGame,
        TakeThis,
        InExtremis,

        // Third Player
        StayInTheWake,
        BackOnTheTop,
        ImStillHere,

        // Second Player
        StayInMyBack,
        RoadToTheFirstPlace,
        IDontLikeDolphins,

        // Best Player
        Bootlicker,
        ForeverBefore,
        ItsStartingToGetHot,

        total
    }
}
