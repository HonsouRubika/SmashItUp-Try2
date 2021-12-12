using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    private int player;

    /// <summary>
    /// TO:
    /// - attendre de jouer une certaine manche (disons la 3eme sur 5 manches)
    /// - choisir un bonus random parmis une liste de bonus
    /// - apply le bonus le temps de la partie
    /// </summary>

    void Start()
    {
        
    }

    public void ApplyBonus()
    {
        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        //pick a random bonus
        int bonus = Random.Range(0, (int)Bonus.total);

        //get le joueur le plus nul
        player = 0;
        int playerScore;
        //for (int i = 0; i< numberOfPlayer; i++)

        //on apply l'effet sur le joueur (le temps de la game)
        switch (bonus)
        {
            case (int)Bonus.Giga_Hit:

                break;
        }
    }

    public void DisableBonus()
    {
        //reset les paramètres du PlayerController du player
    }


    enum Bonus
    {
        Giga_Hit,
        Unbreakable,
        total
    }
}
