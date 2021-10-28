using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int[] _selectedGameModes;
    int _nbManches;
    int _nbMancheActu = 0;


    //fonction à call depuis le menu suite au clic() du bouton play;
    public void initializeGameModes(int nbManches)
    {
        Debug.Log("Initialisation des gameMode pour la partie");
        _nbManches = nbManches;
        _selectedGameModes = new int[nbManches];
        int nbGameModeMax = (int)GameMode.total;


        //randomize depuis une liste de gameMode possible
        for (int i = 0; i < nbManches; ++i)
        {
            _selectedGameModes[i] = Random.Range(0, nbGameModeMax);
        }

        //on passe à la première manche
        NextMap();
    }

    public void NextMap()
    {
        //affichage game over
        Debug.Log("Next map");
        if (_nbMancheActu < _nbManches)
        {
            switch (_selectedGameModes[_nbMancheActu])
            {
                case (int)GameMode.CaptureTheFlag:
                    //TODO : necessite nomenclature pour le nom des maps
                    //SceneManager.LoadScene("CaptureTheFlag"+Random(0,5).to_string);
                    break;
                case (int)GameMode.Loup:
                    //TODO : necessite nomenclature pour le nom des maps
                    //SceneManager.LoadScene("Loup"+Random(0,5).to_string);
                    break;
                case (int)GameMode.CaptureDeZone:
                    //TODO : necessite nomenclature pour le nom des maps
                    //SceneManager.LoadScene("CaptureDeZone"+Random(0,5).to_string);
                    break;
                case (int)GameMode.DestrucBox:
                    //TODO : necessite nomenclature pour le nom des maps
                    //SceneManager.LoadScene("DestrucBox"+Random(0,5).to_string);
                    break;
                default:
                    Debug.Log("Error, GameMode not found");
                    break;
            }
        }
        else
        {
            //partie terminé, affichage des cores finals
            //SceneManager.LoadScene("Scores");
        }
    }

    public void RetourMenu()
    {
        Debug.Log("Retour Menu");
        //SceneManager.LoadScene("Menu");
    }

    enum GameMode
    {
        CaptureTheFlag,
        Loup,
        CaptureDeZone,
        DestrucBox,
        total //egal le nombre d'élément dans l'enum
    }
}