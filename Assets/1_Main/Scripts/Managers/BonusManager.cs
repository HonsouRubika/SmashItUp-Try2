using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    private int player;
    private int selectedBonus;

    /// <summary>
    /// TO:
    /// - attendre de jouer une certaine manche (disons la 3eme sur 5 manches)
    /// - choisir un bonus random parmis une liste de bonus
    /// - apply le bonus le temps de la partie
    /// </summary>

    void Start()
    {
        
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

        //get des players avec les stats approprié
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
            case (int)BonusEndGame.Duffer:
                GameManager.Instance.addSpecificScorePoints(worstPlayer, 75); // add 75 points au pire joueur
                break;
            case (int)BonusEndGame.Bootlicker:
                GameManager.Instance.addSpecificScorePoints(bestPlayer, -25); // on retire 25 points au meilleur joueur
                break;
        }

        GameManager.Instance.UpdatePlayerScore();
    }


    enum BonusInGame
    {
        Giga_Hit,
        Unbreakable,
        total
    }

    enum BonusEndGame
    {
        Duffer,
        Bootlicker,
        total
    }
}
