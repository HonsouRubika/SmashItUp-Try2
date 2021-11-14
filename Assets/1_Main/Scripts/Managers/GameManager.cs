using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int[] _selectedGameModes;
    private int[] _teamCompo;
    int _nbManches;
    int _nbMancheActu = 0;

    int _scoreP1 = 0, _scoreP2 = 0, _scoreP3 = 0, _scoreP4 = 0;

    void Awake()
    {
        #region Make Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    //fonction à call depuis le menu suite au clic() du bouton play;
    public void initializeGameModes(int nbManches)
    {
        Debug.Log("Initialisation des gameMode pour la partie");
        _nbManches = nbManches;
        _selectedGameModes = new int[nbManches];

        _scoreP1 = 0;
        _scoreP2 = 0;
        _scoreP3 = 0;
        _scoreP4 = 0;

        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        for (int i = 0; i < nbManches; ++i)
        {
            _selectedGameModes[i] = Random.Range(0, (int)GameMode.total);
            _teamCompo[i] = Random.Range(0, (int)TeamCompo.Coop); //on retire la coop des Compo d'equipe possible
            //Debug.Log(i + " : " +_selectedGameModes[i]);
        }

        //on passe à la première manche
        NextMap();
    }

    public void Score()
    {
        SceneManager.LoadScene("Score");
    }

    public void TestMap()
    {
        SceneManager.LoadScene("CaptureTheFlag01");
    }

    public void NextMap()
    {
        //affichage game over
        //Debug.Log("Call fct Next map");
        if (_nbMancheActu < _nbManches)
        {
            switch (_selectedGameModes[_nbMancheActu])
            {
                case (int)GameMode.CaptureTheFlag:
                    //TODO : necessite nomenclature pour le nom des maps
                    Debug.Log("CaptureTheFlag");
                    _nbMancheActu++;
                    SceneManager.LoadScene("CaptureTheFlag0" + Random.Range(1, 5));
                    break;
                /*
                case (int)GameMode.Loup:
                    //TODO : necessite nomenclature pour le nom des maps
                    Debug.Log("Loup");
                    _nbMancheActu++;
                    SceneManager.LoadScene("Loup0" + Random.Range(1, 5));
                    break;
                */
                case (int)GameMode.CaptureDeZone:
                    //TODO : necessite nomenclature pour le nom des maps
                    Debug.Log("CaptureZone");
                    _nbMancheActu++;
                    SceneManager.LoadScene("CaptureZone0" + Random.Range(1, 5));
                    break;
                case (int)GameMode.DestrucBox:
                    //TODO : necessite nomenclature pour le nom des maps
                    Debug.Log("DestrucCaisse");
                    _nbMancheActu++;
                    SceneManager.LoadScene("DestrucCaisse0" + Random.Range(1, 5));
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
            Debug.Log("Affichage des scores");
        }
    }

    public void RetourMenu()
    {
        Debug.Log("Retour Menu");
        //SceneManager.LoadScene("Menu");
    }

    //actualise le nouveau score de tout les joueurs
    //dois être appelé avant nextMap()
    //scoreP[x] = nombre de point gagné dans cette manche
    public void addScores(int scoreP1, int scoreP2, int scoreP3, int scoreP4)
    {
        _scoreP1 += scoreP1;
        _scoreP2 += scoreP2;
        _scoreP3 += scoreP3;
        _scoreP4 += scoreP4;
        Debug.Log("ajout des scores");
    }

    public void addSpecificScore(int player, int score)
    {
        switch (player)
        {
            case 1:
                _scoreP1 += score;
                break;
            case 2:
                _scoreP2 += score;
                break;
            case 3:
                _scoreP3 += score;
                break;
            case 4:
                _scoreP4 += score;
                break;
        }
    }

    //renvoie le score d'un joueur en particulier
    public int getScorePlayer(int numJoueur)
    {
        switch (numJoueur)
        {
            case 1:
                Debug.Log("get score P1");
                return _scoreP1;
            case 2:
                Debug.Log("get score P2");
                return _scoreP2;
            case 3:
                Debug.Log("get score P3");
                return _scoreP3;
            case 4:
                Debug.Log("get score P4");
                return _scoreP4;
            default:
                Debug.Log("Erreur : id de joueur inconnu");
                return -1;
        } 
    }

    //renvoie la compo d'equipe pour une manche précise
    public int getTeamCompo(int laManche)
    {
        return _teamCompo[_nbMancheActu];
    }

    //liste des gameMode présent dans le jeu (jouable)
    enum GameMode
    {
        CaptureTheFlag,
        //Loup,
        CaptureDeZone,
        DestrucBox,
        total //egal au nombre d'élément dans l'enum
    }

    enum TeamCompo
    {
        FFA,
        OneVSThree,
        TwoVSTwo,
        Coop,
        total //egal au nombre d'élément dans l'enum
    }
}