using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public FocusPlayers focusPlayersScript;

    private int[] _selectedGameModes;
    private int[] _teamCompo;
    int _nbManches;
    int _nbMancheActu = 0;

    int _scoreP1 = 0, _scoreP2 = 0, _scoreP3 = 0, _scoreP4 = 0;

    public bool isPaused;
    public bool isShowingPlayers;

    //Transition
    private float openCurtainTimer = 3f;
    private float openCurtainTimerActu;
    private float closeCurtainTimer = 3f;
    private float closeCurtainTimerActu;
    private float focusPlayerTimer = 3f;
    private float focusPlayerTimerActu;
    private float countdownTimer = 3f;
    private float countdownTimerActu;
    private TransitionState transitionState;

    //Animation
    [HideInInspector] public TransitionAnim transitionAnimScript;
    public GameObject Curtain;
    private GameObject transition;

    private enum TransitionState {OPENING, OPEN, CLOSING, CLOSE, LOADING, LOADED, FOCUS, COUNTDOWN, FINISHED}

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
        transitionState = TransitionState.OPEN;

        DontDestroyOnLoad(this);

        focusPlayersScript = GetComponentInChildren<FocusPlayers>();
    }

    private void Update()
    {
        //Transition

        //TODO : Take off timers && fully use animator
        //https://stackoverflow.com/questions/34846287/get-name-of-current-animation-state/55933542
        //https://forum.unity.com/threads/current-animator-state-name.331803/
        //transitionState == TransitionState.CLOSING && Time.time >= closeCurtainTimerActu
        //                                  ==========
        //transitionAnimScript.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("AnimationName")

        if (transitionState == TransitionState.CLOSING && Time.time >= closeCurtainTimerActu)
        {
            Debug.Log("Loading scene");
            //on charge la prochaine scene
            transitionState = TransitionState.LOADING;
            goToNextScene();

            /// TODO : WAIT LOADED ?

            Debug.Log("on ouvre les rideaux");
            //Scene is loaded
            transitionState = TransitionState.OPENING;
            openCurtainTimerActu = openCurtainTimer + Time.time;
            //5) open curtains animation
            transitionAnimScript.Open();
        }
        else if (transitionState == TransitionState.OPENING && Time.time >= openCurtainTimerActu)
        {
            Debug.Log("focus player");
            //6) Show Players && goal
            transitionState = TransitionState.FOCUS;
            focusPlayerTimerActu = focusPlayerTimer + Time.time;
            focusPlayersScript.EnableFocus();
            ///BUG: Temps de latente via la fonction focusPlayersScript.EnableFocus();
        }
        else if (transitionState == TransitionState.FOCUS && Time.time >= focusPlayerTimerActu)
        {
            //7) Timer "1,2,3,GO"

            /// TODO : transitionAnimScript.Counstdown();
            
            countdownTimerActu = countdownTimer + Time.time;
            transitionState = TransitionState.COUNTDOWN;
            Debug.Log("Countdown");
        } else if (transitionState == TransitionState.COUNTDOWN && Time.time >= countdownTimerActu)
        {
            //transition finiched
            //8) unfreeze scène2
            transitionState = TransitionState.FINISHED;
            Debug.Log("Transition finished");
            Destroy(transition);
        }
    }

    //fonction à call depuis le menu suite au clic() du bouton play;
    public void initializeGameModes(int nbManches)
    {
        _nbManches = nbManches;
        _selectedGameModes = new int[nbManches];
        _teamCompo = new int[nbManches];

        _scoreP1 = 0;
        _scoreP2 = 0;
        _scoreP3 = 0;
        _scoreP4 = 0;

        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        for (int i = 0; i < nbManches; ++i)
        {
            int nextGameMode = Random.Range(0, (int)GameMode.total);
            //on s'assure que le prochain game mode choisi soit différent du premier
            //Debug.Log(_selectedGameModes[0]);
            if (i > 0)
            {
                //Debug.Log(_selectedGameModes.Length);
                while (nextGameMode == _selectedGameModes[i - 1]) nextGameMode = Random.Range(0, (int)GameMode.total);
            }
            //Debug.Log("oui");
            _selectedGameModes[i] = nextGameMode;
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

    public void FinaleScore()
    {
        //TODO : changer le nom de la scène avec celle des cores finaux
        //SceneManager.LoadScene("ScoreFinal");
    }

    public void NextMap()
    {
        //affichage game over
        //Debug.Log("Call fct Next map");

        //1) freeze scène1

        //2) pop gameObject rideau(NotDestroyOnLoad)
        transition = Instantiate<GameObject>(Curtain);
        DontDestroyOnLoad(transition);
        ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

        //get anim script
        transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution

        //3) play anim "fermer rideau"
        Debug.Log("fermer le rideau");
        transitionAnimScript.Close();
        //dois attendre que l'animation de fermeture ce termine avant de loadScene
        transitionState = TransitionState.CLOSING;
        closeCurtainTimerActu = closeCurtainTimer + Time.time;
    }

    public void goToNextScene()
    {
        //4) LoadScene(scène2)

        if (_nbMancheActu < _nbManches)
        {
            switch (_selectedGameModes[_nbMancheActu])
            {
                case (int)GameMode.CaptureTheFlag:
                    _nbMancheActu++;
                    SceneManager.LoadScene("CaptureTheFlag0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.Loup:
                    _nbMancheActu++;
                    SceneManager.LoadScene("Loup0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.CaptureDeZone:
                    _nbMancheActu++;
                    SceneManager.LoadScene("CaptureZone0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.Contamination:
                    _nbMancheActu++;
                    SceneManager.LoadScene("Contamination0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.KeepTheFlag:
                    _nbMancheActu++;
                    SceneManager.LoadScene("KeepTheFlag0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.CaptureDeZoneMouvante:
                    _nbMancheActu++;
                    SceneManager.LoadScene("ZoneMouvante0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.DestrucBox:
                    _nbMancheActu++;
                    SceneManager.LoadScene("DestrucCaisse0" + Random.Range(1, 3));
                    break;
                default:
                    Debug.Log("Error, GameMode not found");
                    break;
            }

            //Old order :
            //focusPlayersScript.EnableFocus();
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
                return _scoreP1;
            case 2:
                return _scoreP2;
            case 3:
                return _scoreP3;
            case 4:
                return _scoreP4;
            default:
                return -1;
        } 
    }

    //renvoie la compo d'equipe pour une manche précise
    public int getTeamCompo()
    {
        return _teamCompo[_nbMancheActu];
    }

    //liste des gameMode présent dans le jeu (jouable)
    enum GameMode
    {
        CaptureTheFlag,
        Loup,
        CaptureDeZone,
        CaptureDeZoneMouvante,
        DestrucBox,
        Contamination,
        KeepTheFlag,
        total //egal au nombre d'élément dans l'enum
    }

    public enum TeamCompo
    {
        FFA,
        OneVSThree,
        TwoVSTwo,
        Coop, //pas pris en compte actuellement
        total //egal au nombre d'élément dans l'enum
    }
}