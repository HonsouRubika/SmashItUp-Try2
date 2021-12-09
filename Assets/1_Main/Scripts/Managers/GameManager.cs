using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public FocusPlayers focusPlayersScript;
    [HideInInspector] public ScoreValuesManager scoreValuesManagerScript;

    private int[] _selectedGameModes;
    private int[] _teamCompo;
    int _nbManches;
    int _nbMancheActu = 0;

    int _scoreP1 = 0, _scoreP2 = 0, _scoreP3 = 0, _scoreP4 = 0;

    public bool isPaused;
    public bool isShowingPlayers;

    //Transition
    private float focusPlayerTimer = 3f;
    private float focusPlayerTimerActu;
    private float countdownTimer = 3f;
    private float countdownTimerActu;
    private TransitionState transitionState;
    private bool didTransitionStarted = false;
    private bool animatorLoaded = true;

    //Animation
    [HideInInspector] public TransitionAnim transitionAnimScript;
    public GameObject curtain;
    private GameObject transition;
    Animator transitionAnimator;

    private enum TransitionState {OPENING, OPEN, CLOSING, CLOSE, OPEN_YELLOW, CLOSE_YELLOW, OPEN_BLUE, CLOSE_BLUE, LOADING, LOADED, FOCUS, COUNTDOWN, FINISHED}

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
        scoreValuesManagerScript = GetComponent<ScoreValuesManager>();

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
        //SAVE: if (transitionState == TransitionState.CLOSING && Time.time >= closeCurtainTimerActu)
        if (didTransitionStarted && !animatorLoaded)
        {
            transitionAnimator = transition.GetComponent<Animator>();
            animatorLoaded = true;

            //closeCurtainTimerActu = transitionAnimator.

        }

        if (didTransitionStarted && animatorLoaded)
        {
            //if (transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close")) ////// Works but repeats everytime => requires bool isStarted to limit read on fnct
            /// TODO : souviens toi, je pense que c'est la meilleur solution
            // OLD : if (transitionState == TransitionState.CLOSING && Time.time >= closeCurtainTimerActu)

            //yellow = ScoreFinal
            if (transitionState == TransitionState.CLOSE_YELLOW && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close"))
            {
                Debug.Log("Loading scene");
                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;
                //SceneManager.LoadScene("FinalScore");

                Debug.Log("on ouvre les rideaux");
                //Scene is loaded
                transitionState = TransitionState.OPEN_YELLOW;
                //5) open curtains animation
                transitionAnimScript.OpenYellow();
            }
            else if (transitionState == TransitionState.OPEN_YELLOW && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open"))
            {
                Debug.Log("yellow curtain open");
                //6) Show Players && goal
                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;

                //on suprr les rideaux
                Destroy(transition);
            }

            //blue = Score
            if (transitionState == TransitionState.CLOSE_BLUE && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close"))
            {
                Debug.Log("Loading scene");
                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;
                SceneManager.LoadScene("Score");

                Debug.Log("on ouvre les rideaux");
                //Scene is loaded
                transitionState = TransitionState.CLOSE_BLUE;
                //5) open curtains animation
                transitionAnimScript.OpenBlue();
            }
            else if (transitionState == TransitionState.CLOSE_BLUE && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open"))
            {
                Debug.Log("blue curtain open");
                //6) Show Players && goal
                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;

                //on suprr les rideaux
                Destroy(transition);
            }

            //red = NextMap
            if (transitionState == TransitionState.CLOSING && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close"))
            {
                Debug.Log("Loading scene");
                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;
                goToNextScene();

                Debug.Log("on ouvre les rideaux");
                //Scene is loaded
                transitionState = TransitionState.OPENING;
                //5) open curtains animation
                transitionAnimScript.OpenRed();
            }
            else if (transitionState == TransitionState.OPENING && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open"))
            {
                Debug.Log("focus player");
                //6) Show Players && goal
                transitionState = TransitionState.FOCUS;
                focusPlayerTimerActu = focusPlayerTimer + Time.time;
                focusPlayersScript.EnableFocus();

                //on suprr les rideaux
                Destroy(transition);
            }
            ///BUG: Temps de latente via la fonction focusPlayersScript.EnableFocus();
            else if (transitionState == TransitionState.FOCUS && Time.time >= focusPlayerTimerActu)
            {
                //7) Timer "1,2,3,GO"

                /// TODO : transitionAnimScript.Counstdown();

                countdownTimerActu = 0.001f + Time.time;
                transitionState = TransitionState.COUNTDOWN;
                Debug.Log("Countdown");
            }
            /// TODO : switch quand l'anim sera prete
            //else if (transitionState == TransitionState.COUNTDOWN && transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open"))
            else if (transitionState == TransitionState.COUNTDOWN && Time.time >= countdownTimerActu)
            {
                //transition finiched
                //8) unfreeze sc�ne2
                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;
                Debug.Log("Transition finished");
                //start minigame timer
                //Timer.StartTimer();
            }
        }
    }

    //fonction � call depuis le menu suite au clic() du bouton play;
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
            //on s'assure que le prochain game mode choisi soit diff�rent du premier
            if (i > 0)
            {
                while (nextGameMode == _selectedGameModes[i - 1]) nextGameMode = Random.Range(0, (int)GameMode.total);
            }
            _selectedGameModes[i] = nextGameMode;
            _teamCompo[i] = Random.Range(0, (int)TeamCompo.Coop); //on retire la coop des Compo d'equipe possible
            //Debug.Log("Team compo : " +_teamCompo[i]);
            //Debug.Log(i + " : " +_selectedGameModes[i]);
        }

        //on passe � la premi�re manche
        NextMap();
    }

    public void Score()
    {
        Debug.Log("call fnct score");
        if (!didTransitionStarted)
        {
            didTransitionStarted = true;
            Debug.Log("call fnct score");
            //reset transition state
            transitionState = TransitionState.OPEN;

            //2) pop gameObject rideau(NotDestroyOnLoad)
            transition = Instantiate<GameObject>(curtain);
            DontDestroyOnLoad(transition);
            animatorLoaded = false;
            ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

            //get anim script
            transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution

            //3) play anim "fermer rideau"
            Debug.Log("fermer le rideau");
            transitionAnimScript.CloseBlue();
            //dois attendre que l'animation de fermeture ce termine avant de loadScene
            transitionState = TransitionState.CLOSE_BLUE;
        }
    }

    public void TestMap()
    {
        SceneManager.LoadScene("CaptureTheFlag01");
    }

    public void FinaleScore()
    {
        if (!didTransitionStarted)
        {
            didTransitionStarted = true;
            Debug.Log("call fnct score");
            //reset transition state
            transitionState = TransitionState.OPEN;

            //2) pop gameObject rideau(NotDestroyOnLoad)
            transition = Instantiate<GameObject>(curtain);
            DontDestroyOnLoad(transition);
            animatorLoaded = false;
            ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

            //get anim script
            transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution

            //3) play anim "fermer rideau"
            Debug.Log("fermer le rideau");
            transitionAnimScript.CloseYellow();
            //dois attendre que l'animation de fermeture ce termine avant de loadScene
            transitionState = TransitionState.CLOSE_YELLOW;
        }
    }

    public void NextMap()
    {
        if (!didTransitionStarted)
        {
            didTransitionStarted = true;
            Debug.Log("call fnct next map");
            //reset transition state
            transitionState = TransitionState.OPEN;

            //2) pop gameObject rideau(NotDestroyOnLoad)
            transition = Instantiate<GameObject>(curtain);
            DontDestroyOnLoad(transition);
            animatorLoaded = false;
            ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

            //get anim script
            transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution

            //3) play anim "fermer rideau"
            Debug.Log("fermer le rideau");
            transitionAnimScript.CloseRed();
            //dois attendre que l'animation de fermeture ce termine avant de loadScene
            transitionState = TransitionState.CLOSING;
        }
    }

    public void goToNextScene()
    {
        //4) LoadScene(sc�ne2)

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
            //partie termin�, affichage des cores finals
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
    //dois �tre appel� avant nextMap()
    //scoreP[x] = nombre de point gagn� dans cette manche
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

    //renvoie la compo d'equipe pour une manche pr�cise
    public int getTeamCompo()
    {
        return _teamCompo[_nbMancheActu];
    }

    //liste des gameMode pr�sent dans le jeu (jouable)
    enum GameMode
    {
        CaptureTheFlag,
        Loup,
        CaptureDeZone,
        CaptureDeZoneMouvante,
        DestrucBox,
        Contamination,
        KeepTheFlag,
        total //egal au nombre d'�l�ment dans l'enum
    }

    public enum TeamCompo
    {
        FFA,
        OneVSThree,
        TwoVSTwo,
        Coop, //pas pris en compte actuellement
        total //egal au nombre d'�l�ment dans l'enum
    }
}