using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public FocusPlayers focusPlayersScript;
    [HideInInspector] public ScoreValuesManager scoreValuesManagerScript;

    private int[] _selectedGameModes;
    public int[] _teamCompo;
    //team compo
    public int[] playersTeam;
    public GameObject[] players;
    public List<GameObject> playersUnsorted;

    public int _nbManches;
    public int _nbMancheActu = 0;

    public int _scoreP1 = 0, _scoreP2 = 0, _scoreP3 = 0, _scoreP4 = 0;
    public int _addingScoreP1 = 0, _addingScoreP2 = 0, _addingScoreP3 = 0, _addingScoreP4 = 0;

    public bool isPaused;
    public bool isShowingPlayers;

    //Pause
    public PauseMenu pauseScript;

    //Timer
    private bool isFirstLoadDone = false;
    public float durationMiniGame = 30;

    //Transition
    private float focusPlayerTimer = 3f;
    private float focusPlayerTimerActu;
    private TransitionState transitionState;
    private bool didTransitionStarted = false;
    private bool animatorLoaded = true;

    //Bonus
    [HideInInspector] public BonusManager bonusManagerScript;
    public int BonusRound = 3;

    //Save
    public int nbGameFinished = 0;

    //Animation
    [HideInInspector] public TransitionAnim transitionAnimScript;
    [HideInInspector] public ConsigneDisplayScript consigneAnimScript;
    public GameObject consigne;
    public GameObject curtain;
    public GameObject countdown;
    private GameObject transition;
    private GameObject countdownInstance;
    private GameObject consigneInstance;
    Animator transitionAnimator;
    Animator consigneAnimator;
    Animation countdownAnimation;

    //Test/Debug
    [Header("DEBUG")]
    public bool isTest = false;
    public string testSceneName = "Contamination01";
    public GameMode[] gameModeToTest;
    public TeamCompo[] teamCompoToTest;

    [HideInInspector] public List<PlayerController> playerControllers;

    private enum TransitionState { OPENING, OPEN, CLOSING, CLOSE, CONSIGNE, OPEN_YELLOW, CLOSE_YELLOW, OPEN_BLUE, CLOSE_BLUE, LOADING, LOADED, FOCUS, COUNTDOWN, FINISHED }

    //TRANSITION

    private GameObject transitionUI;

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
        transitionUI = transform.GetChild(4).gameObject;

        //Load save
        if (SaveSystem.LoadProgression() != null)
        {
            ProgressionData pd = SaveSystem.LoadProgression();
            nbGameFinished = pd.GetNbGameFinished();
            //Debug.LogWarning("Nb game finished : " + nbGameFinished);
        }

        BonusRound = _nbManches / 2;


        transitionState = TransitionState.OPEN;

        DontDestroyOnLoad(this);

        focusPlayersScript = GetComponentInChildren<FocusPlayers>();
        scoreValuesManagerScript = GetComponent<ScoreValuesManager>();
        bonusManagerScript = GetComponent<BonusManager>();
    }

    private void Update()
    {
        //Transition

        if (didTransitionStarted && !animatorLoaded)
        {
            //transitionAnimator = transition.GetComponent<Animator>();
            animatorLoaded = true;
        }

        if (didTransitionStarted && animatorLoaded)
        {

            //yellow = ScoreFinal
            if (transitionState == TransitionState.CLOSE_YELLOW /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close")*/)
            {
                Debug.Log("close yellow");

                //Bonus fin de partie
                /// TODO: Verifier apply des bonus/scores
                //bonusManagerScript.ApplyBonusEndGame();
                //UpdatePlayerScore();

                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;

                SceneManager.LoadScene("ScoreFinal");

                //Scene is loaded
                transitionState = TransitionState.OPEN_YELLOW;
                //5) open curtains animation
                //transitionAnimScript.OpenYellow();
            }
            else if (transitionState == TransitionState.OPEN_YELLOW /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open")*/)
            {
                //6) Show Players && goal
                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;

                //on suprr les rideaux
                /// TODO: Unfreeze players
                for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
                {
                    scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = false;
                }

                //inc nbGameFinished and THEN save
                nbGameFinished++;
                SaveSystem.SaveProgression(this);

                //Destroy(transition);
            }

            //blue = Score
            if (transitionState == TransitionState.CLOSE_BLUE /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close")*/)
            {
                //Debug.Log("Loading scene");
                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;
                //SceneManager.LoadScene("ScoreMiniGames");

                /// TODO: Unfreeze players
                for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
                {
                    scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = false;
                }

                //Debug.Log("on ouvre les rideaux");
                //Scene is loaded
                transitionState = TransitionState.OPEN_BLUE;
                //5) open curtains animation
                //transitionAnimScript.OpenBlue();
            }
            else if (transitionState == TransitionState.OPEN_BLUE /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open")*/)
            {
                //Debug.Log("blue curtain open");
                //6) Show Players && goal
                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;

                //on suprr les rideaux
                //Destroy(transition);

                //ApplyBonus
                /// TODO: Verifier efficacité des bonus
                /*
                if (_nbMancheActu == BonusRound)
                {
                    bonusManagerScript.ApplyBonusInGame();
                }
                else if (_nbMancheActu == BonusRound + 1)
                {
                    bonusManagerScript.DisableBonusInGame();
                }
                */
            }

            //red = NextMap
            if (transitionState == TransitionState.CLOSING /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("close")*/)
            {
                //on charge la prochaine scene
                transitionState = TransitionState.LOADING;
                goToNextScene();

                //Scene is loaded
                transitionState = TransitionState.OPENING;
                //5) open curtains animation
                //transitionAnimScript.OpenRed();
            }
            else if (transitionState == TransitionState.OPENING /*&& transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("open")*/)
            {
                //on suprr les rideaux
                //Destroy(transition);
                //Debug.Log("oui");

                //instantiate animation
                consigneInstance = Instantiate<GameObject>(consigne);
                consigneAnimator = consigneInstance.GetComponent<Animator>();
                consigneAnimScript = consigneInstance.GetComponent<ConsigneDisplayScript>();

                //set active bon param consigne (mode de jeu, nom du mini jeu)
                consigneAnimScript.SetActiveGoodConsigne();
                //play anim
                //consigneAnimator.Play();

                transitionState = TransitionState.CONSIGNE;
            }
            else if (transitionState == TransitionState.CONSIGNE && consigneAnimator.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
            {
                //6) Show Players && goal
                transitionState = TransitionState.FOCUS;

                focusPlayersScript.EnableFocus();

            }
            else if (transitionState == TransitionState.FOCUS && Time.time >= focusPlayerTimerActu)
            {
                Destroy(consigneInstance);
                //7) Timer "1,2,3,GO"
                countdownInstance = Instantiate<GameObject>(countdown);
                countdownAnimation = countdownInstance.GetComponent<Animation>();
                countdownAnimation.Play();

                transitionState = TransitionState.COUNTDOWN;
            }
            else if (transitionState == TransitionState.COUNTDOWN && !countdownAnimation.isPlaying)
            {
                //countdown finiched
                Destroy(countdownInstance);

                //8) unfreeze sc�ne
                for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
                {
                    scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = false;
                }
                /*
                 * PlayerController pcScript = scoreValuesManagerScript.players[i].GetComponent<PlayerController>();
                    pcScript.isFrozen = false;
                    pcScript.
                 *
                */

                transitionState = TransitionState.FINISHED;
                didTransitionStarted = false;
                //start minigame timer
                if (GameObject.Find("--UI CORNER--") != null) GameObject.Find("--UI CORNER--").GetComponent<Timer>().StartTimer();
                else if (GameObject.Find("--UI TOP--") != null) GameObject.Find("--UI TOP--").GetComponent<Timer>().StartTimer();
                else Debug.LogWarning("Error : prefab UI not found");
                //GameObject.Find("Music").GetComponent<Music_Mini_Game>().StartMusic();

            }
        }

        CheckPlayerCollision();
    }

    public void SortPlayersInList()
    {
        players = playersUnsorted.OrderBy(go => go.name).ToArray();
    }

    private void CheckPlayerCollision()
    {
        switch (playerControllers.Count)
        {
            case 2:
                if (playerControllers[0].isStunt || playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, true);
                }

                if (!playerControllers[0].isStunt && !playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, false);
                }
                break;
            case 3:
                if (playerControllers[0].isStunt || playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, true);
                }
                if (playerControllers[0].isStunt || playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[2].cc, true);
                }
                if (playerControllers[1].isStunt || playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[2].cc, true);
                }

                if (!playerControllers[0].isStunt && !playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, false);
                }
                if (!playerControllers[0].isStunt && !playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[2].cc, false);
                }
                if (!playerControllers[1].isStunt && !playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[2].cc, false);
                }
                break;
            case 4:
                if (playerControllers[0].isStunt || playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, true);
                }
                if (playerControllers[0].isStunt || playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[2].cc, true);
                }
                if (playerControllers[0].isStunt || playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[3].cc, true);
                }
                if (playerControllers[1].isStunt || playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[2].cc, true);
                }
                if (playerControllers[1].isStunt || playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[3].cc, true);
                }
                if (playerControllers[2].isStunt || playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[2].cc, playerControllers[3].cc, true);
                }

                if (!playerControllers[0].isStunt && !playerControllers[1].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[1].cc, false);
                }
                if (!playerControllers[0].isStunt && !playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[2].cc, false);
                }
                if (!playerControllers[0].isStunt && !playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[0].cc, playerControllers[3].cc, false);
                }
                if (!playerControllers[1].isStunt && !playerControllers[2].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[2].cc, false);
                }
                if (!playerControllers[1].isStunt && !playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[1].cc, playerControllers[3].cc, false);
                }
                if (!playerControllers[2].isStunt && !playerControllers[3].isStunt)
                {
                    Physics2D.IgnoreCollision(playerControllers[2].cc, playerControllers[3].cc, false);
                }
                break;
        }
    }

    //fonction � call depuis le menu suite au clic() du bouton play;
    public void initializeGameModes()
    {
        //DEBUG
        if (gameModeToTest.Length > 0) _nbManches = gameModeToTest.Length;

        _nbMancheActu = 0;
        transitionState = TransitionState.OPEN;
        _selectedGameModes = new int[_nbManches];
        _teamCompo = new int[_nbManches];

        _scoreP1 = 0;
        _scoreP2 = 0;
        _scoreP3 = 0;
        _scoreP4 = 0;

        //randomize depuis une liste de gameMode possible
        Random.InitState((int)Time.time);
        for (int i = 0; i < _nbManches; ++i)
        {
            int nextGameMode = Random.Range(0, (int)GameMode.total);

            ///// LIMITATIONS /////
            if (i > 0) //pas de limitation pour le premier mini jeu choisi (logique)
            {
                //on s'assure que le prochain game mode choisi soit diff�rent du premier
                while (GameModeKind[nextGameMode] == GameModeKind[_selectedGameModes[i - 1]])
                {
                    nextGameMode = Random.Range(0, (int)GameMode.total);
                }

            }
            _selectedGameModes[i] = nextGameMode;

            //TeamCompo
            //DEBUG
            if (teamCompoToTest.Length > 0)
            {
                //Debug.Log(teamCompoToTest[i % teamCompoToTest.Length]);
                _teamCompo[i] = (int)teamCompoToTest[i % teamCompoToTest.Length];
            }
            else
            {
                if (scoreValuesManagerScript.players.Length < 4) _teamCompo[i] = (int)TeamCompo.FFA;
                else
                {
                    //Limitation
                    int nextTeamCompo = Random.Range(0, (int)TeamCompo.Coop); //on retire la coop des Compo d'equipe possible

                    if (i > 0) //pas de limitation pour le premier mini jeu choisi (logique)
                    {

                        //on s'assure que le prochain team compo choisi soit diff�rent du premier
                        while (nextTeamCompo == _teamCompo[i - 1])
                        {
                            nextTeamCompo = Random.Range(0, (int)TeamCompo.Coop);
                        }

                    }
                    _teamCompo[i] = nextTeamCompo;
                }
            }

            //Debug.Log("Team compo : ");
            //Debug.Log(_teamCompo[i]);
            //Debug.Log(i + " : " +_selectedGameModes[i]);
            //Debug.Log(nextGameMode + " : " +GameModeKind[nextGameMode]);
        }

        bonusManagerScript.enabled = true;

        //on passe � la premi�re manche
        if (isTest && gameModeToTest.Length <= 0) TestMap();
        else if (gameModeToTest.Length > 0) TestGameMode();
        else NextMap();
    }

    public void Score()
    {
        if (!didTransitionStarted)
        {
            didTransitionStarted = true;

            //stop le timer
            GameObject ui = GameObject.Find("--UI--");
            if (ui != null) ui.GetComponent<Timer>().StopTimer();

            /// TODO: Freeze players
            for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
            {
                scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = true;
            }

            GetComponent<NewScoreSystem>().DisplayScore(true);
        }
    }

    public void FadeOutTransition()
    {
        transitionUI.SetActive(true);
    }

    public void SwitchScene()
    {
        NextMap();
        transitionUI.SetActive(false);
    }

    public void LoadSceneAfterScore()
    {
        //Debug.Log("call fnct score");
        //reset transition state
        transitionState = TransitionState.OPEN;

        //2) pop gameObject rideau(NotDestroyOnLoad)
        //transition = Instantiate<GameObject>(curtain);
        //DontDestroyOnLoad(transition);
        animatorLoaded = false;
        ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

        //get anim script
        //transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution
        //transitionAnimator = transition.GetComponent<Animator>();

        //3) play anim "fermer rideau"
        //Debug.Log("fermer le rideau");
        //transitionAnimScript.CloseBlue();
        //dois attendre que l'animation de fermeture ce termine avant de loadScene
        transitionState = TransitionState.CLOSE_BLUE;
    }

    public int GetGameModeActu()
    {
        return _selectedGameModes[_nbMancheActu - 1];
    }

    public void TestMap()
    {
        isTest = true;
        NextMap();
    }

    public void TestGameMode()
    {
        isTest = false;
        NextMap();
    }

    public void FinaleScore()
    {
        didTransitionStarted = true;
        animatorLoaded = true;

        /// TODO: Freeze players
        for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
        {
            scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = true;
        }

        //Debug.Log("call fnct score");
        //reset transition state
        //transitionState = TransitionState.OPEN;

        //2) pop gameObject rideau(NotDestroyOnLoad)
        //transition = Instantiate<GameObject>(curtain);
        //DontDestroyOnLoad(transition);
        //animatorLoaded = false;
        ///BUG : Changer le z axe du rideau => les players sont visibles par dessus le rideau

        //get anim script
        //transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution

        //3) play anim "fermer rideau"
        //Debug.Log("fermer le rideau");
        //transitionAnimScript.CloseYellow();
        //dois attendre que l'animation de fermeture ce termine avant de loadScene
        transitionState = TransitionState.CLOSE_YELLOW;
    }

    public void NextMap()
    {
        if (_nbMancheActu < _nbManches)
        {
            //partie termin�, affichage des scores finals
            //Debug.Log("Affichage des scores");
            animatorLoaded = true;
            didTransitionStarted = true;
            transitionState = TransitionState.CLOSE_YELLOW;

            didTransitionStarted = true;

            // Freeze players
            for (int i = 0; i < scoreValuesManagerScript.players.Length; i++)
            {
                scoreValuesManagerScript.players[i].GetComponent<PlayerController>().isFrozen = true;
            }

            //OLD Stop Clock
            /*
            GameObject ui = GameObject.Find("--UI--");
            if (ui != null) ui.GetComponent<Timer>().StopTimer();
            */
            /*
            if (GameObject.Find("--UI CORNER--") != null) GameObject.Find("--UI CORNER--").GetComponent<Timer>().StopTimer();
            else if (GameObject.Find("--UI TOP--") != null) GameObject.Find("--UI TOP--").GetComponent<Timer>().StopTimer();
            else Debug.LogWarning("Error : prefab UI not found");
            */

            //reset transition state
            transitionState = TransitionState.OPEN;

            //2) pop gameObject rideau(NotDestroyOnLoad)
            //transition = Instantiate<GameObject>(curtain);
            //DontDestroyOnLoad(transition);
            animatorLoaded = false;

            //get anim script
            //transitionAnimScript = transition.GetComponent<TransitionAnim>(); //bonne solution
            //transitionAnimator = transition.GetComponent<Animator>();

            //3) play anim "fermer rideau"
            //transitionAnimScript.CloseRed();
            //dois attendre que l'animation de fermeture ce termine avant de loadScene
            transitionState = TransitionState.CLOSING;
        }
        else
        {
            FinaleScore();
        }
    }

    public void goToNextScene()
    {
        //Debug.Log("next scene");
        //4) LoadScene(sc�ne2)

        if (_nbMancheActu < _nbManches && !isTest && gameModeToTest.Length <= 0 && teamCompoToTest.Length <= 0)
        {
            switch (_selectedGameModes[_nbMancheActu])
            {
                case (int)GameMode.CaptureTheFlag:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("FlagCapture" + Random.Range(1, 3));
                    break;
                case (int)GameMode.Loup:
                    _nbMancheActu++;
                    //Limitation de GameMode
                    _teamCompo[_nbMancheActu - 1] = (int)TeamCompo.FFA;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("Loup0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.CaptureDeZone:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("CaptureZone0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.Contamination:
                    _nbMancheActu++;

                    //Limitation de GameMode
                    _teamCompo[_nbMancheActu - 1] = (int)TeamCompo.FFA;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("Contamination0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.KeepTheFlag:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("KeepTheFlag0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.CaptureDeZoneMouvante:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("ZoneMouvante0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.CollectTheCoin:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("Coins0" + Random.Range(1, 3));
                    break;
                case (int)GameMode.WhackAMole:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();

                    SceneManager.LoadScene("WackAMole0" + Random.Range(1, 3));
                    break;
                default:
                    Debug.Log("Error, GameMode not found or taken out");
                    break;
            }

        }
        else if (_nbMancheActu < _nbManches & gameModeToTest.Length > 0)
        {
            if (_nbMancheActu > gameModeToTest.Length) _nbMancheActu = 0;
            switch (gameModeToTest[_nbMancheActu % gameModeToTest.Length])
            {
                case GameMode.CaptureTheFlag:
                    _nbMancheActu++;

                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("FlagCapture" + Random.Range(1, 3));
                    break;
                case GameMode.Loup:
                    _nbMancheActu++;
                    //Limitation de GameMode
                    if (teamCompoToTest.Length > 0) teamCompoToTest[_nbMancheActu % teamCompoToTest.Length] = (int)TeamCompo.FFA;
                    else _teamCompo[_nbMancheActu - 1] = (int)TeamCompo.FFA;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("Loup0" + Random.Range(1, 3));
                    break;
                case GameMode.CaptureDeZone:
                    _nbMancheActu++;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("CaptureZone0" + Random.Range(1, 3));
                    break;
                case GameMode.Contamination:
                    _nbMancheActu++;
                    //Limitation de GameMode
                    if (teamCompoToTest.Length > 0) teamCompoToTest[_nbMancheActu % teamCompoToTest.Length] = (int)TeamCompo.FFA;
                    else _teamCompo[_nbMancheActu - 1] = (int)TeamCompo.FFA;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("Contamination0" + Random.Range(1, 3));
                    break;
                case GameMode.KeepTheFlag:
                    _nbMancheActu++;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("KeepTheFlag0" + Random.Range(1, 3));
                    break;
                case GameMode.CaptureDeZoneMouvante:
                    _nbMancheActu++;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("ZoneMouvante0" + Random.Range(1, 3));
                    break;
                case GameMode.CollectTheCoin:
                    _nbMancheActu++;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("Coins0" + Random.Range(1, 3));
                    break;
                case GameMode.WhackAMole:
                    _nbMancheActu++;
                    //load team compo
                    playersTeam = AssignPlayerTeam();
                    SceneManager.LoadScene("WackAMole0" + Random.Range(1, 3));
                    break;
                default:
                    Debug.Log("Error, GameMode not found or taken out");
                    break;
            }
        }
        else if (_nbMancheActu < _nbManches & isTest)
        {
            _nbMancheActu++; //je garde ?
            SceneManager.LoadScene(testSceneName);
        }
    }

    public void PauseGame(uint playerID, InputAction.CallbackContext context)
    {
        pauseScript.GamePause(playerID, context);
    }

    public int getNbPlayer()
    {
        return scoreValuesManagerScript.players.Length;
    }

    public GameObject getSpecificPlayer(int id)
    {
        return scoreValuesManagerScript.players[id];
    }

    public void RetourMenu()
    {
        Debug.Log("Retour Menu");
        //SceneManager.LoadScene("Menu");
    }

    //actualise le nouveau score de tout les joueurs
    //dois �tre appel� avant nextMap()
    //scoreP[x] = nombre de point gagn� dans cette manche
    public void addScoresPoints(int scoreP1, int scoreP2, int scoreP3, int scoreP4)
    {
        _addingScoreP1 = scoreP1;
        _addingScoreP2 = scoreP2;
        _addingScoreP3 = scoreP3;
        _addingScoreP4 = scoreP4;
    }

    public void addSpecificScorePoints(int player, int score)
    {
        //Debug.Log("oui " + player);
        switch (player)
        {
            case 1:
                _addingScoreP1 = score;
                break;
            case 2:
                _addingScoreP2 = score;
                break;
            case 3:
                _addingScoreP3 = score;
                break;
            case 4:
                _addingScoreP4 = score;
                break;
        }
    }

    //Security to reset earn points when changing scene
    public void resetScorePoints()
    {
        _addingScoreP1 = 0;
        _addingScoreP2 = 0;
        _addingScoreP3 = 0;
        _addingScoreP4 = 0;
    }

    public void UpdatePlayerScore()
    {
        _scoreP1 += _addingScoreP1;
        _scoreP2 += _addingScoreP2;
        _scoreP3 += _addingScoreP3;
        _scoreP4 += _addingScoreP4;
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

    public int getDividedScorePlayer(int numJoueur, string division)
    {
        switch (numJoueur)
        {
            case 1:
                switch (division)
                {
                    case "hundreds":
                        return (_scoreP1 / 100) % 10;
                    case "tens":
                        return (_scoreP1 / 10) % 10;
                    case "units":
                        return _scoreP1 % 10;
                    default:
                        return -1;
                }
            case 2:
                switch (division)
                {
                    case "hundreds":
                        return (_scoreP2 / 100) % 10;
                    case "tens":
                        return (_scoreP2 / 10) % 10;
                    case "units":
                        return _scoreP2 % 10;
                    default:
                        return -1;
                }
            case 3:
                switch (division)
                {
                    case "hundreds":
                        return (_scoreP3 / 100) % 10;
                    case "tens":
                        return (_scoreP3 / 10) % 10;
                    case "units":
                        return _scoreP3 % 10;
                    default:
                        return -1;
                }
            case 4:
                switch (division)
                {
                    case "hundreds":
                        return (_scoreP4 / 100) % 10;
                    case "tens":
                        return (_scoreP4 / 10) % 10;
                    case "units":
                        return _scoreP4 % 10;
                    default:
                        return -1;
                }
            default:
                return -1;
        }
    }

    //return mvp [0-3]
    public int getMVP()
    {
        //get le meilleur joueur
        int bestPlayer = 0;
        int bestPlayerSpread = GameManager.Instance.getScorePlayer(0);

        //get des players avec les stats approprié
        for (int i = 0; i < GameManager.Instance.getNbPlayer(); i++)
        {

            if (GameManager.Instance.getScorePlayer(i + 1) > bestPlayerSpread)
            {
                bestPlayerSpread = GameManager.Instance.getScorePlayer(i + 1);
                bestPlayer = i;
            }
        }

        return bestPlayer;
    }

    //return earn points to add to player score
    public int getAddedPointsPlayer(int numJoueur)
    {
        switch (numJoueur)
        {
            case 1:
                return _addingScoreP1;
            case 2:
                return _addingScoreP2;
            case 3:
                return _addingScoreP3;
            case 4:
                return _addingScoreP4;
            default:
                return -1;
        }
    }

    //renvoie la compo d'equipe pour une manche pr�cise
    public int getTeamCompo()
    {
        return _teamCompo[_nbMancheActu - 1];
    }

    public int[] AssignPlayerTeam()
    {
        /// TODO : Attribution al�atoire pour la compp des equipes 1 et 2
        //Debug.Log("In Game : " + GameManager.Instance.getTeamCompo());
        playersTeam = new int[players.Length];

        //verif si nb players insufisant
        int teamCompo = getTeamCompo();
        if (players.Length <= 2 && teamCompo == 1) teamCompo = 0; //coop to 1v3
        if (players.Length <= 2 && teamCompo == 2) teamCompo = 3; //coop to 1v3

        for (int i = 0; i < players.Length; i++)
        {
            switch (teamCompo)
            {
                case (int)TeamCompo.FFA:
                    playersTeam[i] = i;
                    //Debug.Log("1v1v1v1");
                    //Debug.Log("In switch FFA");
                    //pas d'�quipe
                    break;
                case (int)TeamCompo.Coop:
                    //Debug.Log("coop");
                    playersTeam[i] = 0;
                    //tous ensemble equipe 0
                    break;
                case (int)TeamCompo.OneVSThree:
                    //Debug.Log("1v3");
                    if (i == getMVP()) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
                case (int)TeamCompo.TwoVSTwo:
                    //Debug.Log("2v2");
                    if (i < 2) playersTeam[i] = 0;
                    else playersTeam[i] = 1;
                    break;
            }
        }
        //Debug.Log("in switch alea : " + playersTeam.Length);

        //aléa team players
        switch (teamCompo)
        {
            case (int)TeamCompo.OneVSThree:
                //No alea since we pick the best player above
                /*
                for (int i = 0; i < playersTeam.Length; i++)
                {
                    int temp = playersTeam[i];
                    int randomIndex = Random.Range(i, playersTeam.Length);
                    playersTeam[i] = playersTeam[randomIndex];
                    playersTeam[randomIndex] = temp;
                    //Debug.Log(playersTeam[randomIndex]);
                }
                */
                break;
            case (int)TeamCompo.TwoVSTwo:
                for (int i = 0; i < playersTeam.Length; i++)
                {
                    int temp = playersTeam[i];
                    int randomIndex = Random.Range(i, playersTeam.Length);
                    playersTeam[i] = playersTeam[randomIndex];
                    playersTeam[randomIndex] = temp;
                    //Debug.Log(playersTeam[randomIndex]);
                }
                break;
        }
        
        if (teamCompo == 1 || teamCompo == 2)
        {
            for (int i = 0; i < playersTeam.Length; i++)
            {
                switch (playersTeam[i])
                {
                    case 0:
                        players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("purple");
                        players[i].GetComponent<PlayerSkins>().SetCursorTeam("purple");
                        players[i].GetComponent<PlayerSkins>().SetHaloTeam("purple");
                        break;
                    case 1:
                        players[i].GetComponent<PlayerSkins>().SetHammerColorByTeam("orange");
                        players[i].GetComponent<PlayerSkins>().SetCursorTeam("orange");
                        players[i].GetComponent<PlayerSkins>().SetHaloTeam("orange");
                        break;
                }
            }
        }
        return playersTeam;
    }

    //liste des gameMode pr�sent dans le jeu (jouable)
    public enum GameMode
    {
        CaptureTheFlag,
        Loup,
        CaptureDeZone,
        CaptureDeZoneMouvante,
        CollectTheCoin,
        Contamination,
        KeepTheFlag,
        WhackAMole,
        total //egal au nombre d'�l�ment dans l'enum
    }

    private int[] GameModeKind =
    {
        0, //CaptureTheFlag = Flag
        1, //Loup = Wolf
        2, //CaptureDeZone = Capture
        2, //CaptureDeZoneMouvante = Capture
        3, //ColectTheCoin
        1, //Contamination = Wolf
        0, //KeepTheFlag = Flag
        4  //WhackAMole
    };

    public enum TeamCompo
    {
        FFA,
        OneVSThree,
        TwoVSTwo,
        Coop, //pas pris en compte actuellement
        total //egal au nombre d'�l�ment dans l'enum
    }
}