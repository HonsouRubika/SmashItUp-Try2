using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public enum JumpState
{
    Grounded,
    PrepareToJump,
    Jumping,
    InFlight,
    Falling,
    Landed
}

public class PlayerController : MonoBehaviour
{
    //new input
    private CharacterController controller;
    private Vector2 movementInput = Vector2.zero;
    private PlayerSound PlayerSoundScript;

    //Action
    private Action actionState = Action.Jump;

    [Header("Déplacement")]
    public float speed = 3;
    public float jumpSpeed = 3;
    public float movementJumpSpeed = 3;
    //addForce
    public float ratioAddForce = 1;
    public float ratioAddForceAir = 1;
    private float movementActu = 0;

    //TODO : GameMode avec Hp
    //public uint health; 

    //Player Controll Asignation
    /*[System.NonSerialized]*/
    public uint playerID = 0;

    //Sprite et animation
    public Rigidbody2D rb;
    //public BoxCollider2D bc;
    public CapsuleCollider2D cc;

    //jump variable
    [Header("Jump")]
    public JumpState jumpState = JumpState.InFlight;
    public uint nbJump = 2;
    private uint nbJumpActu = 0;
    public float maxJumpHigh = 1;
    private float startJumpPosition;
    public float minJumpHeigh = 0.5f;
    public float fallSpeedMultiplier = 1f;
    private bool didTouchRoof = false;
    private bool didTouchRoofInitiated = false;
    private float tempVelocityRoofCheck = 0;
    private float velocityWhenTouchedRoof = 0;

    
    //wall jump
    [Header("WallJump")]
    private float startWallJumpPosition;
    public float maxWallJumpHigh = 1;
    public float wallJumpSpeed = 1;
    public float wallJumpAngleY = 40 / (Mathf.Sqrt(2) / 2);
    public float wallJumpMovementFreeze = 0.2f;
    private float wallJumpMovementFreezeActuL;
    private float wallJumpMovementFreezeActuR;
    public float numberMaxWalljump = 2;
    private float numberMaxWalljumpActu;
    //coyot time
    private bool coyoteTimeCheck = false;
    private bool coyoteTimeDone = false;
    public float coyotTime = 0.1f;
    private float coyotTimeActu;
    private float shaitanerieDUnity = 0.1f;
    private float shaitanerieDUnityActu = 0;
    //wallgrip
    public float wallGripTime = 1;
    private float wallGripTimeActu;
    private bool isWallGripStarted = false;
    public float wallGripFallSpeed = 0;
    private bool isJump = false;
    private bool isWallJump = false;

    //test sur le jump: add force
    //[Range(0.001f, 5f)]
    //public float jumpRatioAddForce = 0.1f;
    public float newJumpRatioAddForce = 20;
    //[Range(0.001f, 5f)]
    //public float wallJumpRatioAddForce = 0.1f;
    //private float jumpMovementActu = 1;
    private bool isJumpFallSetted = false;
    //private float startCurveVelocity;
    public float jumpAirIntake;

    //Colision checks
    [Header("GroundCheck")]
    public Transform groundCheck;
    public Transform gripLeftCheck;
    public Transform gripRightCheck;
    private bool isGrippingLeft = false, isGrippingRight = false;

    ///////////Attack///////////
    //Hitbox
    [Header("Attack")]
    public Transform hammerFXSpawn;
    public Transform attackPointL;
    public Transform attackPointR;
    public GameObject hammerPointL;
    public GameObject hammerPointR;
    public float attackRange = 0.5f;
    public float hammerHitboxRange = 0.75f;
    public LayerMask enemyLayer, hammerHitboxLayer;
    //Force et Distance de projection
    public float hammerXProjection = 3;
    public float hammerYProjection = 3;
    public float hammerBlockProjection = 1.5f;
    public float HammerSideProjectionMaxDistance = 1;
    private float startProjectedPostion = 0;
    public float projectionStopSpeed = 0.5f; //ralentissement après distance de projection atteinte
    [System.NonSerialized] public bool isBeingProjected = false;
    //false = droite; true = gauche
    //Paramètre vitesse
    private bool attackDirection = false;
    public float attackRate = 2f;
    public bool attackIsCD = false;
    public float attackDuration = 0.1f;
    private float attackDurationActu;
    public float untilAttackEffectiveDuration;
    private float untilAttackEffectiveDurationActu;
    private bool isAttackRunningL, isAttackRunningR;
    private bool didAttackedBlockedL, didAttackedBlockedR;
    private float nextAttackTime = 0f;
    //vibration
    public float vibrationDurration = 0.5f;
    private float vibrationDurrationActu;
    private bool isVibrationSet = false;


    [Header("Stun")]
    //public bool disableCollider = false;
    public float stunTime = 0.5f;
    private float stunTimeActu;
    public bool isFrozen = false;
    public float blockStunTime = 0.5f;
    private float blockStunTimeActu;
    private float invicibilityTimeActu;
    public float invicibilityTime = 0.5f;
    [HideInInspector] public bool isStunt = false;


    [Header("LD")]
    public float trampolineJump = 6;

    [Header("StartMenu")]
    private Vector2 _spawn;
    private bool isSkinSelected = false;

    [System.NonSerialized] public float lastTimeAttackHit = 0;
    [System.NonSerialized] public float lastTimeGotHit = 0;

    [HideInInspector] public int playerIDHit;
    private float timeAttack = 0;
    [HideInInspector] public bool hitPlayer = false;

    //Animation
    private PlayerAnim playerAnimScript;
    [HideInInspector] public Transform playerAnimator;

    //Skin
    private PlayerSkins playerSkinScript;

    //Bonus
    [System.NonSerialized] public bool isUnbreakable = false;

    //FX
    private PlayerFX playerFXScript;
    private bool attackOneTime = false;

    void Start()
    {
        playerFXScript = GetComponent<PlayerFX>();
        cc = GetComponent<CapsuleCollider2D>();
        GameManager.Instance.playerControllers.Add(this);
        GameManager.Instance.playersUnsorted.Add(gameObject);
        GameManager.Instance.SortPlayersInList();

        ///integrate wallride?
        wallGripTime = 0;

        //init var
        startJumpPosition = transform.position.y;
        stunTimeActu = 0;
        blockStunTimeActu = 0;
        wallJumpMovementFreezeActuL = 0;
        wallJumpMovementFreezeActuR = 0;
        numberMaxWalljumpActu = 0;
        attackDurationActu = 0;
        isAttackRunningL = false;
        isAttackRunningR = false;
        didAttackedBlockedL = false;
        didAttackedBlockedR = false;

        //deactivate hammerHitBox
        hammerPointL.SetActive(false);
        hammerPointR.SetActive(false);

        //get anim script
        playerAnimScript = GetComponentInChildren<PlayerAnim>();

        playerSkinScript = GetComponent<PlayerSkins>();

        PlayerSoundScript = GetComponentInChildren<PlayerSound>();
    }

    public void PlayerConnected(Vector2 spawn, uint id)
    {
        playerID = id;
        _spawn = spawn;

        //player not in scene
        rb.transform.position = _spawn;
        isFrozen = true;
        isSkinSelected = false;

        //already set false in PlayerSkin Start()
        //playerSkinScript.currentSkin.SetActive(false);
    }

    public void PlayerDepop()
    {
        //player not in scene
        isFrozen = true;
        rb.transform.position = _spawn;
        playerSkinScript.currentSkin.SetActive(false);
        isSkinSelected = false;
    }

    public void SkinSelected()
    {
        rb.transform.position = _spawn; 
        playerSkinScript.currentSkin.SetActive(true);
        isFrozen = false;
        isSkinSelected = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
        {
            movementInput = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers && isSkinSelected)
        {
            //stun
            if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu && !isFrozen)
            {
                if (context.started)
                {
                    computeJump();
                }
                //long jump
                /*
                else if (context.canceled && !didTouchRoof && transform.position.y >= (minJumpHeigh + startJumpPosition))
                {
                    shaitanerieDUnityActu = Time.time; //ne touche pas à ça
                    //le perso descend car il relache la touche de saut
                    //startJumpPosition = transform.position.y - maxJumpHigh;
                    isJumpFallSetted = true;
                }
                //small jump
                else if (context.canceled && !didTouchRoof && transform.position.y < (minJumpHeigh + startJumpPosition))
                {
                    //dois s'arreter au minimum de saut
                    shaitanerieDUnityActu = Time.time;
                    startJumpPosition = startJumpPosition - maxJumpHigh;
                }
                */
            }
        }
        else if (SceneManager.GetActiveScene().name == "NewStartScene" && !isSkinSelected)
        {
            SkinSelected();
        }
    }
    public void OnAttacked(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
        {
            //stun
            if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu)
            {
                if (context.started) computeAttack();
            }
       
        }
    }

    public void OnBPressed(InputAction.CallbackContext context)
    {
        if (isSkinSelected && SceneManager.GetActiveScene().name == "NewStartScene")
        {
            if (context.started)
            {
                PlayerDepop();
            }
        }
        else if (GameManager.Instance.isPaused)
        {
            GameManager.Instance.UnPauseGame(playerID, context);
        }
    }

    public void PlayerPausesGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.PauseGame(playerID, context);
        }
    }

    private void Update()
    {
        PlayerAttacked();

        verifProjectionMax();

        //stun
        if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu && playerAnimScript != null && PlayerSoundScript != null && playerAnimator != null && !isFrozen)
        {
            isStunt = false;

            //Enable back collision between players 
            /*if (disableCollider)
            {
                //Physics2D.IgnoreLayerCollision(8, 8, false);
                for (int i = 0; i < GameManager.Instance.playerColliders.Count; i++)
                {
                    Physics2D.GetIgnoreCollision
                }
            }*/

            //anim
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Expulsion(false);
            }

            //reset var
            stunTimeActu = 0;
            blockStunTimeActu = 0;

            /////////////////////////////////////
            //////////// DEPLACEMENT ////////////
            /////////////////////////////////////
            move();

            /////////////////////////////////
            //////////// ATTAQUE ////////////
            /////////////////////////////////

            attack();

        }
        else if (Time.time < stunTimeActu - stunTime + stunTime / 5)
        {
            //vitesse de projection constante pendant le stun
            //Debug.Log("stun is active");
            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(hammerXProjection, hammerYProjection);
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(-hammerXProjection, hammerYProjection);
            }
        }
        else if (isFrozen)
        {
            rb.velocity = new Vector2(0, 0);
            playerAnimScript.Expulsion(false);
            //do nothing
            //player is frozen during TRANSITION
        }
        else if (playerAnimScript != null && PlayerSoundScript != null && playerAnimator != null) //player is stun
        {
            isStunt = true;

            //anim
            playerAnimScript.Expulsion(true);
            playerFXScript.EjectionFX();


            //Disable the collision between players when player are stunt
            /*if (disableCollider)
            {
                //Physics2D.IgnoreLayerCollision(8, 8, true);
                for (int i = 0; i < GameManager.Instance.playerColliders.Count; i++)
                {
                    Physics2D.IgnoreCollision(cc, GameManager.Instance.playerColliders[i], true);
                } 
            }*/
            PlayerSoundScript.Ejection();

            if (transform.position.x <= startProjectedPostion)
            {
                playerAnimator.localScale = new Vector2(-Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }
            else if (transform.position.x >= startProjectedPostion)
            {
                playerAnimator.localScale = new Vector2(Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }
        }
        else
        {
            //error ce produit seulement lors de la frame d'apparition de l'entité, on peux l'ignorer
            //l'un des scipt n'est pas attaché
            //Debug.Log("l'un des script n'est pas attaché");
        }

        //vibration
        if(vibrationDurrationActu > Time.time && isVibrationSet)
        {
            try
            {
                GetGamepad().SetMotorSpeeds(0.123f, 0.234f);
            }
            catch
            {
                //Debug.Log("keyboard exception");
            }
            
        }
        else if (isVibrationSet)
        {
            try
            {
                isVibrationSet = false;
                GetGamepad().SetMotorSpeeds(0, 0);
            }
            catch
            {
                //Debug.Log("keyboard exception");
            }
        }

        /*if (Time.time >= nextAttackTime)
        {
            attackIsCD = false;
            playerSkinScript.SetHammerOpacity(1);
        }
        else
        {
            attackIsCD = true;
            playerSkinScript.SetHammerOpacity(0.5f);
        }*/

        if (playerAnimScript.playerAnimator != null)
        {
            if (Time.time >= nextAttackTime)
            {
                playerSkinScript.SetHammerOpacity(1);
                attackOneTime = false;
            }
            else
            {
                if (playerAnimScript.GetCooldownPlayer())
                {
                    playerSkinScript.SetHammerOpacity(0f);
                }
            }
        }
        
    }

    void attack()
    {
        //Attaque droite et gauche
        //Gauche
        //2)animation attaque + verif block
        if (isAttackRunningL && Time.time < untilAttackEffectiveDurationActu)
        {
            //Detection d'un blocage
            Collider2D[] hammers = Physics2D.OverlapCircleAll(hammerPointL.transform.position, hammerHitboxRange, hammerHitboxLayer);

            if (hammers.Length > 1)
            {
                //on contre
                //Debug.Log("Blocage à Gauche");
                //Debug.Log(hammers.Length);

                didAttackedBlockedL = true;
            }

        }
        //3) applyAttack
        if (isAttackRunningL && Time.time < attackDurationActu && Time.time > untilAttackEffectiveDurationActu)
        {
            //Debug.Log("Attaque gauche3");
            //Animation / Attack hitbox Apparition (pour test)

            //Detection des player dans la zone
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointL.position, attackRange, enemyLayer);
            //Physics2D.OverlapCapsuleAll

            if (!didAttackedBlockedL)
            {
                //On leur applique une velocit� (effet de l'attaque)
                foreach (Collider2D enemy in hitEnemies)
                {
                    //Appliquer une velocit�
                    //Attention: check la direction pour coord x
                    if (enemy.gameObject != cc.gameObject)
                    {
                        if (!attackOneTime)
                        {
                            playerFXScript.AttackFXTouch();
                            attackOneTime = true;
                        }
                        enemy.GetComponent<PlayerController>().applyAttack(-hammerXProjection, hammerYProjection);
                        lastTimeAttackHit = Time.time;
                        playerIDHit = (int)enemy.GetComponent<PlayerController>().playerID;
                        
                    }
                }

                if (!attackOneTime)
                {
                    if (hitEnemies.Length > 0)
                    {

                    }
                    else
                    {
                        playerFXScript.AttackFXEmpty();
                        attackOneTime = true;
                    }
                }
            }
            else
            {
                // apply self blockProjection
                applyBlock(hammerBlockProjection, 0);

                if (!attackOneTime) { attackOneTime = true; playerFXScript.BlockFX(); }
            }
        }
        else if (isAttackRunningL && Time.time >= attackDurationActu)
        {
            isAttackRunningL = false;
            //reset var
            didAttackedBlockedL = false;
            //disparition hammerHitBox
            hammerPointL.SetActive(false);
        }

        //Droite
        //2) animation attaque + verif block
        if (isAttackRunningR && Time.time < attackDurationActu)
        {
            //Detection d'un blocage
            Collider2D[] hammers = Physics2D.OverlapCircleAll(hammerPointR.transform.position, hammerHitboxRange, hammerHitboxLayer);
            if (hammers.Length > 1)
            {
                //on contre
                //Debug.Log("oui : " + hammers.Length + " " + hammers[1].gameObject.name);
                //Debug.Log("oui : " + hammers.Length + " " + hammers[0].gameObject.name);
                didAttackedBlockedR = true;
                //on contre
                //Debug.Log("Blocage à droite");
                //Debug.Log(hammers.Length);
            }
            else
            {
                //Debug.Log("non : " + hammers.Length + " " + hammers[0].gameObject.name);
            }
        }
        //3) applyAttack
        if (isAttackRunningR && Time.time < attackDurationActu && Time.time > untilAttackEffectiveDurationActu)
        {
            //reset timeAttack
            //nextAttackTime = Time.time + 1f / attackRate;

            //Animation / Attack hitbox Apparition (pour test)

            //Detection des player dans la zone
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointR.position, attackRange, enemyLayer);
            //ici

            if (!didAttackedBlockedR)
            {
                //On leur applique une velocit� (effet de l'attaque)
                foreach (Collider2D enemy in hitEnemies)
                {
                    //Appliquer une velocit�
                    //Attention: check la direction pour coord x
                    if (enemy.gameObject != cc.gameObject)
                    {
                        if (!attackOneTime)
                        {
                            playerFXScript.AttackFXTouch();
                            attackOneTime = true;
                        }
                        enemy.GetComponent<PlayerController>().applyAttack(hammerXProjection, hammerYProjection);
                        lastTimeAttackHit = Time.time;
                        playerIDHit = (int)enemy.GetComponent<PlayerController>().playerID;
                    }
                }

                if (!attackOneTime)
                {
                    if (hitEnemies.Length > 0)
                    {

                    }
                    else
                    {
                        playerFXScript.AttackFXEmpty();
                        attackOneTime = true;
                    }
                }
            }
            else
            {
                // apply self blockProjection
                applyBlock(-hammerBlockProjection, 0);

                if (!attackOneTime) { attackOneTime = true; playerFXScript.BlockFX(); }
            }
        }
        else if (isAttackRunningR && Time.time >= attackDurationActu)
        {
            //reset var
            didAttackedBlockedR = false;
            isAttackRunningR = false;
            //disparition hammerHitBox
            hammerPointR.SetActive(false);
        }
    }

    private void PlayerAttacked()
    {
        if (lastTimeAttackHit > timeAttack)
        {
            timeAttack = lastTimeAttackHit;
            hitPlayer = true;
        }
        else
        {
            hitPlayer = false;
        }
    }

    void move()
    {

        //Gauche + Droite
        //au sol

        if ((movementInput.x < -0.3) && !isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && (jumpState != JumpState.InFlight && jumpState != JumpState.Falling))
        {
            //gauche
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            //movementActu = -1;
            attackDirection = true;

            //wall grip
            isWallGripStarted = false;

            
            //AddForce
            if (movementActu <= -1)
            {
                movementActu = -1;
                rb.velocity = new Vector2(movementInput.x * speed, rb.velocity.y);
            }
            else
            {
                //player is turning
                movementActu -= ratioAddForce * Time.deltaTime;
                rb.velocity = new Vector2(movementActu * speed, rb.velocity.y);
                //Debug.Log("is turning");
            }

            //Action (anim)
            actionState = Action.Run;

            //anim
            if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
            {
                playerAnimator.localScale = new Vector2(Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }

            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.WallRide(false);
                PlayerSoundScript.Run();
                playerFXScript.RunFX();
            }
        }
        else if ((movementInput.x > 0.3) && !isGrippingRight && Time.time >= wallJumpMovementFreezeActuR && (jumpState != JumpState.InFlight && jumpState != JumpState.Falling))
        {
            //droite
            //rb.velocity = new Vector2(speed, rb.velocity.y);
            //movementActu = 1;
            attackDirection = false;

            //wall grip
            isWallGripStarted = false;

            //AddForce
            if (movementActu >= 1)
            {
                movementActu = 1;
                rb.velocity = new Vector2(movementInput.x * speed, rb.velocity.y);
            }
            else
            {
                //player is turning
                movementActu += ratioAddForce * Time.deltaTime;
                rb.velocity = new Vector2(movementActu * speed, rb.velocity.y);
                //Debug.Log("is turning");
            }

            //Action (anim)
            actionState = Action.Run;

            //anim
            if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
            {
                playerAnimator.localScale = new Vector2(-Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }

            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.WallRide(false);
                PlayerSoundScript.Run();
                playerFXScript.RunFX();
            }
        }
        //en l'air
        else if ((movementInput.x < -0.3) && !isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling))
        {
            //gauche
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            //TODO : temp de pause quand changement de direction lors d'un saut
            //  Voir Bloc note "to do SUI" sur le bureau
            //rb.velocity = new Vector2(-movementJumpSpeed, rb.velocity.y);
            attackDirection = true;

            movementActu -= ratioAddForceAir * Time.deltaTime;
            if (movementActu < -1) movementActu = -1;
            rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);

            //wall grip
            isWallGripStarted = false;

            //Action (anim)
            actionState = Action.Jump;

            //anim
            if (!GameManager.Instance.isPaused)
            {
                playerAnimator.localScale = new Vector2(Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }

            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.WallRide(false);

                if (jumpState == JumpState.Grounded)
                {
                    PlayerSoundScript.Run();
                }
            }
        }
        //wall grip left
        else if ((movementInput.x < -0.3) && isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && jumpState == JumpState.Falling)
        {
            if (!isWallGripStarted)
            {
                //le perso s'arrete un temps
                wallGripTimeActu = wallGripTime + Time.time;
                isWallGripStarted = true;
            }
            else
            {
                //player doesnt move
                //rb.velocity = new Vector2(0, 0);
            }

            if (Time.time >= wallGripTimeActu)
            {
                //le perso doit glisser du mur
                playerAnimScript.WallSlide(true);
                PlayerSoundScript.WallRide(true);
                playerAnimator.localScale = new Vector2(-Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
                rb.velocity = new Vector2(0, rb.velocity.y);
                //Debug.Log("wall grip left : " + isJumpFallSetted + " , " + didTouchRoof);
                //isWallGripStarted = false;
            }
        }
        else if ((movementInput.x > 0.3) && !isGrippingRight && Time.time >= wallJumpMovementFreezeActuR && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling))
        {
            //droite
            //rb.velocity = new Vector2(movementJumpSpeed, rb.velocity.y);
            attackDirection = false;

            movementActu += ratioAddForceAir * Time.deltaTime;
            if (movementActu > 1) movementActu = 1;
            rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);

            //wall grip
            isWallGripStarted = false;

            //Action (anim)
            actionState = Action.Jump;

            //anim
            if (!GameManager.Instance.isPaused)
            {
                playerAnimator.localScale = new Vector2(-Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
            }

            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.WallRide(false);

                if (jumpState == JumpState.Grounded)
                {
                    PlayerSoundScript.Run();
                }
            }
        }
        //wall grip right
        else if ((movementInput.x > 0.3) && isGrippingRight && Time.time >= wallJumpMovementFreezeActuR && jumpState == JumpState.Falling)
        {
            if (!isWallGripStarted)
            {
                //le perso s'arrete un temps
                wallGripTimeActu = wallGripTime + Time.time;
                isWallGripStarted = true;
            }
            else
            {
                //player doesnt move
                //rb.velocity = new Vector2(0, 0);
            }

            if (Time.time >= wallGripTimeActu)
            {
                //le perso doit glisser du mur
                playerAnimScript.WallSlide(true);
                PlayerSoundScript.WallRide(true);
                playerAnimator.localScale = new Vector2(Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
                //rb.velocity = new Vector2(0, rb.velocity.y);
                //rb.velocity = new Vector2(rb.velocity.x, -wallGripFallSpeed);
                //Debug.Log("wall grip right : " + isJumpFallSetted +" , "+ didTouchRoof);
                //isWallGripStarted = false;
            }
        }
        else if (jumpState == JumpState.Grounded && !isBeingProjected)
        {
            //Debug.Log("x axe stop cause grounded");
            //rb.velocity = new Vector2(0, rb.velocity.y);
            /*
            if (movementActu > 0)
            {
                movementActu -= ratioAddForce * Time.deltaTime;
                if (movementActu < 0) movementActu = 0;
            }
            else if (movementActu < 0)
            {
                movementActu += ratioAddForce * Time.deltaTime;
                if (movementActu > 0) movementActu = 0;
            }
            */
            //rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);
            rb.velocity = new Vector2(0, rb.velocity.y);
            movementActu = 0;

            //Action (anim)
            actionState = Action.Idle;

            //anim
            playerAnimScript.Running(false);
            playerAnimScript.Idle(true);
            playerAnimScript.WallSlide(false);
            PlayerSoundScript.WallRide(false);
        }
        // grip fall au wall
        /*
        else if ((isGrippingLeft || isGrippingRight) && jumpState == JumpState.Falling && Time.time >= wallGripTimeActu)
        {
            //le perso doit glisser du mur
                rb.velocity = new Vector2(rb.velocity.x, -wallGripFallSpeed);
        }
        */
        else if (isGrippingLeft)
        {
            //Debug.Log("le perso doit glisser du mur");
            
        }
        else if (isGrippingRight)
        {
            //Debug.Log("le perso doit glisser du mur");
            
        }
        //air intake
        else if (movementInput.x == 0 && jumpState != JumpState.Grounded)
        {
            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x - (jumpAirIntake * Time.deltaTime), rb.velocity.y);
                if (rb.velocity.x < 0) rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + (jumpAirIntake * Time.deltaTime), rb.velocity.y);
                if (rb.velocity.x > 0) rb.velocity = new Vector2(0, rb.velocity.y);
            }
            //Debug.Log("1 : " + rb.velocity.x);
        }
        else
        {
            playerAnimScript.WallSlide(false);
            PlayerSoundScript.WallRide(false);
        }

        ///// JUMP CURVE /////
        /*if ((transform.position.y >= startJumpPosition + maxJumpHigh || isJumpFallSetted) && jumpState != JumpState.Grounded)*/
        if (jumpState != JumpState.Grounded)
        {
            /*if (!isJumpFallSetted)
            {
                isJumpFallSetted = true;
            }*/
            

            //if(!didTouchRoof)
            //{
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - newJumpRatioAddForce * Time.deltaTime);
                if (rb.velocity.y < -jumpSpeed * fallSpeedMultiplier) rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed * fallSpeedMultiplier);
                //Debug.Log("2 : " + rb.velocity.x);
            //}

            //check if player is falling
            if (rb.velocity.y < 0 && !didTouchRoof && jumpState != JumpState.Falling)
            {
                jumpState = JumpState.Falling;
                playerAnimScript.Falling(true);
            }
            //ici
        }

        //roof stun & fall
        /*
        if (didTouchRoof)
        {   
            tempVelocityRoofCheck -= newJumpRatioAddForce * Time.deltaTime;
            if (tempVelocityRoofCheck < 0)
            {
                didTouchRoof = false;
                //error : jumpFallSetted = false
                isJumpFallSetted = true;
                rb.velocity = new Vector2(0, tempVelocityRoofCheck);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
        */

        //Colision Side GripCheck
        //float height = bc.size.y;
        if (Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform"))) &&
            //check if touched roof
            (!Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.65f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.65f), 1 << LayerMask.NameToLayer("Plateform")) ||
            !Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.65f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.65f), 1 << LayerMask.NameToLayer("Ground")))) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")))
            && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling)) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Plateform")))
        {
            isGrippingLeft = true;
            isBeingProjected = false;

            //Action (anim)
            actionState = Action.GripLeft;
        }
        else if (Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.65f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.65f), 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.65f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.65f), 1 << LayerMask.NameToLayer("Ground")))
        {
            if (!didTouchRoofInitiated)
            {
                //fix error (r.velocity.y est deja nul ou negative => didTouchRoof)
                didTouchRoof = true;
                didTouchRoofInitiated = true;
                if (rb.velocity.y <= 0)
                {
                    //do nothing
                    //var already setted bellow
                }
                else
                {
                    tempVelocityRoofCheck = rb.velocity.y;
                    velocityWhenTouchedRoof = rb.velocity.y;
                }
            }
        }
        else
        {
            //correct error above
            if (rb.velocity.y > 0)
            {
                tempVelocityRoofCheck = rb.velocity.y;
                velocityWhenTouchedRoof = rb.velocity.y;
            }
            isGrippingLeft = false;
        }
        if (Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform"))) &&
            //check if touches roof
            (!Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.2f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ||
            !Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.2f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")))) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")))
            && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling)) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Plateform")))
        {
            isGrippingRight = true;
            isBeingProjected = false;

            //Action (anim)
            actionState = Action.GripRight;
        }
        else
        {
            isGrippingRight = false;
        }

        //Colision Sol
        if ((Physics2D.Linecast(transform.position, new Vector2(groundCheck.transform.position.x - 1f, groundCheck.transform.position.y), 1 << LayerMask.NameToLayer("Ground"))) || /*gauche*/
        (Physics2D.Linecast(transform.position, new Vector2(groundCheck.transform.position.x - 1f, groundCheck.transform.position.y), 1 << LayerMask.NameToLayer("Plateform"))) ||
        (Physics2D.Linecast(transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer("Ground"))) || /*milieu*/
        (Physics2D.Linecast(transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer("Plateform"))) ||
        (Physics2D.Linecast(transform.position, new Vector2(groundCheck.transform.position.x + 1f, groundCheck.transform.position.y), 1 << LayerMask.NameToLayer("Ground"))) || /*droite*/
        (Physics2D.Linecast(transform.position, new Vector2(groundCheck.transform.position.x + 1f, groundCheck.transform.position.y), 1 << LayerMask.NameToLayer("Plateform"))) ||
        (Physics2D.Linecast(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x - 1f, groundCheck.transform.position.y - 0.1f), 1 << LayerMask.NameToLayer("Player"))) || /*Gauche*/
        (Physics2D.Linecast(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f), 1 << LayerMask.NameToLayer("Player"))) || /*Milieu*/
        (Physics2D.Linecast(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x + 1f, groundCheck.transform.position.y - 0.1f), 1 << LayerMask.NameToLayer("Player"))))   /*Droite*/
        {
            if (Time.time >= shaitanerieDUnityActu)
            {
                //le perso touche le sol
                jumpState = JumpState.Grounded;
                nbJumpActu = 0;
                playerAnimScript.Falling(false);
                startJumpPosition = transform.position.y;
                numberMaxWalljumpActu = 0; //reset nb de walljump
                isBeingProjected = false;
                coyoteTimeCheck = false;
                coyoteTimeDone = false;
                didTouchRoof = false;
                didTouchRoofInitiated = false;
                //reset var for walljump
                wallJumpMovementFreezeActuL = Time.time;
                wallJumpMovementFreezeActuR = Time.time;

                //bounce off
                rb.velocity = new Vector2(rb.velocity.x, 0);

                //addForce = null
                //jumpMovementActu = 0;
                isJumpFallSetted = false;

                isJump = false;
                isWallJump = false;

                //anim
                if (playerAnimScript.playerAnimator != null)
                {
                    playerAnimScript.Jumping(false);
                }
            }
        }
        else
        {
            //coyot time
            if (!coyoteTimeCheck)
            {
                coyotTimeActu = Time.time + coyotTime;
                coyoteTimeCheck = true;
            }

            if (rb.velocity.y <= 0 && !isJumpFallSetted && !didTouchRoofInitiated)
            {
                //le perso chute ou touche le plafond
                isJumpFallSetted = true;
            }
            /*
            else if (rb.velocity.y <= movementJumpSpeed-startCurveVelocity && isJumpFallSetted)
            {
                jumpState = JumpState.Falling;
                rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed);
            }
            */
            else if (jumpState == JumpState.Grounded) // ne change pas si jumpState = JumpState.Falling
            {
                jumpState = JumpState.InFlight;
            }
        }

    }

    void verifProjectionMax()
    {

        //Distance de projection max
        //gauche
        if ((transform.position.x <= startProjectedPostion + -HammerSideProjectionMaxDistance) && isBeingProjected)
        {
            //Debug.Log("stop gauche");
            rb.velocity = new Vector2(rb.velocity.x + projectionStopSpeed, rb.velocity.y);
            //end projection
            if (rb.velocity.x + projectionStopSpeed >= 0)
            {
                isBeingProjected = false;
                //Action (anim)
                actionState = Action.Idle;
            }

        } //droite
        else if ((transform.position.x >= startProjectedPostion + HammerSideProjectionMaxDistance) && isBeingProjected)
        {
            //Debug.Log("stop droite");
            rb.velocity = new Vector2(rb.velocity.x - projectionStopSpeed, rb.velocity.y);
            //end projection
            if (rb.velocity.x + projectionStopSpeed <= 0)
            {
                isBeingProjected = false;
                //Action (anim)
                actionState = Action.Idle;
            }
        }
    }

    void computeJump()
    {
        ///old with wall jump restriction
        /*
         * if (jumpState == JumpState.Grounded || //simple saut
            (jumpState != JumpState.Grounded && Time.time < coyotTimeActu && coyoteTimeCheck && !coyoteTimeDone && !isGrippingRight && !isGrippingLeft) || 
            (nbJumpActu != nbJump && !isGrippingRight && !isGrippingLeft))
        */

        if (jumpState == JumpState.Grounded || //simple saut
            (jumpState != JumpState.Grounded && Time.time < coyotTimeActu && coyoteTimeCheck && !coyoteTimeDone && !didTouchRoof) || 
            (nbJumpActu != nbJump && !didTouchRoof))

        {
            //coyot time
            if (jumpState != JumpState.Grounded && Time.time < coyotTimeActu && coyoteTimeCheck && !coyoteTimeDone) coyoteTimeDone = true;

            //double jump
            nbJumpActu++;

            //wall grip
            isWallGripStarted = false;

            //roof check
            didTouchRoofInitiated = false;
            didTouchRoof = false;

            //block le check du ground : ne pas toucher
            shaitanerieDUnityActu = Time.time + shaitanerieDUnity;

            //Debug.Log("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            actionState = Action.Jump;
            startJumpPosition = transform.position.y;
            //anim
            if (playerAnimScript != null)
            {
                playerAnimScript.Jumping(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.WallRide(false);
                PlayerSoundScript.Jump();
                if (jumpState != JumpState.Grounded) playerFXScript.JumpFX();
            }

            //jump or walljump
            isJump = true;
            isWallJump = false;

            //Jump Curve
            //jumpMovementActu = 1;
            isJumpFallSetted = true;
            //ici
        }
        /// Wall jump integrated?
        /*
        else if (isGrippingRight && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            jumpState = JumpState.InFlight;
            //jump to the left w/ 45� angle
            rb.velocity = new Vector2(-wallJumpSpeed, wallJumpAngleY);
            actionState = Action.Jump;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;

            //block le check du ground
            shaitanerieDUnityActu = Time.time + shaitanerieDUnity;

            //freeze movement for small time
            wallJumpMovementFreezeActuR = wallJumpMovementFreeze + Time.time;

            //addForce
            movementActu = -1;

            //wall grip
            isWallGripStarted = false;

            //jump or walljump
            isJump = false;
            isWallJump = true;

            //Jump Curve
            //jumpMovementActu = 1;
            isJumpFallSetted = false;
        }
        else if (isGrippingLeft && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            jumpState = JumpState.InFlight;
            //jump to the right w/ 45� angle
            rb.velocity = new Vector2(wallJumpSpeed, wallJumpAngleY);
            actionState = Action.Jump;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;
            //block le check du ground
            shaitanerieDUnityActu = Time.time + shaitanerieDUnity;
            //freeze movement for small time
            wallJumpMovementFreezeActuL = wallJumpMovementFreeze + Time.time;

            //addForce
            movementActu = 1;

            //wall grip
            isWallGripStarted = false;

            //jump or walljump
            isJump = false;
            isWallJump = true;

            //Jump Curve
            //jumpMovementActu = 1;
            isJumpFallSetted = false;
        }
        */

        //Debug.Log(" 2) JumpState : " + jumpState + ", coyoteTimeCheck : " + coyoteTimeCheck);
    }

    public void computeTrampolineJump()
    {
        //block le check du ground : ne pas toucher
        shaitanerieDUnityActu = Time.time + shaitanerieDUnity;

        //Debug.Log("Jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        actionState = Action.Jump;
        startJumpPosition = transform.position.y;
        //anim
        if (playerAnimScript != null)
        {
            playerAnimScript.Jumping(true);
            playerAnimScript.WallSlide(false);
            PlayerSoundScript.WallRide(false);
            PlayerSoundScript.Jump();
            if (jumpState != JumpState.Grounded) playerFXScript.JumpFX();
        }

        //jump or walljump
        isJump = true;
        isWallJump = false;

        //Jump Curve
        //jumpMovementActu = 1;
        isJumpFallSetted = false;
    }

    void computeAttack()
    {
        //Gauche
        if (attackDirection && Time.time >= nextAttackTime)
        {
            //reset timeAttack
            nextAttackTime = Time.time + attackRate;

            isAttackRunningL = true;
            attackDurationActu = attackDuration + Time.time;
            untilAttackEffectiveDurationActu = untilAttackEffectiveDuration + Time.time;

            //apparition hammerHitBox
            hammerPointL.SetActive(true);
            hammerPointR.SetActive(false);

            //anim
            if (playerAnimScript != null) playerAnimScript.Attack();
            if(PlayerSoundScript != null) PlayerSoundScript.HammerPouet();
        }
        //Droite
        else if (!attackDirection && Time.time >= nextAttackTime)
        {
            //reset timeAttack
            nextAttackTime = Time.time + attackRate;

            isAttackRunningR = true;
            attackDurationActu = attackDuration + Time.time;
            untilAttackEffectiveDurationActu = untilAttackEffectiveDuration + Time.time;

            //apparition hammerHitBox
            hammerPointR.SetActive(true);
            hammerPointL.SetActive(false);

            //anim
            if (playerAnimScript != null)
            {
                playerAnimScript.Attack();
            }
            else
            {
                //Debug.LogWarning("Error: Le PlayerAnimScript est null");
            }
            ///ToDO : corriger error :
            /*
             * NullReferenceException: Object reference not set to an instance of an object
             * PlayerController.computeAttack () (at Assets/1_Main/Scripts/Players/PlayerController.cs:774)
            */
            if (PlayerSoundScript != null)
            {
                PlayerSoundScript.HammerPouet();
            }
            else
            {
                //Debug.LogWarning("Error: Le PlayerSoundScript est null");
            }
        }
    }

    void applyAttack(float velocityX, float velocityY)
    {
        //invicibility check
        if (Time.time > invicibilityTimeActu)
        {
            //Action (anim)
            actionState = Action.Projected;

            //Stun
            stunTimeActu = stunTime + Time.time;

            //Invicibility
            invicibilityTimeActu = invicibilityTime + Time.time;

            //var pour mini jeu
            lastTimeGotHit = Time.time;

            //gestion distance max
            isBeingProjected = true;
            startProjectedPostion = transform.position.x;

            //Velocit�
            rb.velocity = new Vector2(velocityX, velocityY);

            //Vibration
            vibrationDurrationActu = vibrationDurration + Time.time;
            isVibrationSet = true;
        }
        else
        {
            //Debug.Log("attack blocked cause player is invicible");
        }
    }
    void applyBlock(float velocityX, float velocityY)
    {
        //Action (anim)
        actionState = Action.Projected;

        //Stun
        blockStunTimeActu = blockStunTime + Time.time;

        //var pour mini jeu
        lastTimeGotHit = Time.time;

        //gestion distance max
        isBeingProjected = true;
        startProjectedPostion = transform.position.x;

        //Velocit�
        rb.velocity = new Vector2(velocityX, velocityY);

        //Vibration
        vibrationDurrationActu = vibrationDurration/2 + Time.time;
        isVibrationSet = true;
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => GetComponent<PlayerInput>().devices.Any(d => d.deviceId == g.deviceId));

        #region Linq Query Equivalent Logic
        //Gamepad gamepad = null;
        //foreach (var g in Gamepad.all)
        //{
        //    foreach (var d in _playerInput.devices)
        //    {
        //        if(d.deviceId == g.deviceId)
        //        {
        //            gamepad = g;
        //            break;
        //        }
        //    }
        //    if(gamepad != null)
        //    {
        //        break;
        //    }
        //}
        //return gamepad;
        #endregion
    }

    void OnDrawGizmosSelected()
    {
        //hammer
        Gizmos.DrawWireSphere(attackPointR.position, attackRange);
        Gizmos.DrawWireSphere(attackPointL.position, attackRange);
        Gizmos.DrawWireSphere(hammerPointL.transform.position, hammerHitboxRange);
        Gizmos.DrawWireSphere(hammerPointR.transform.position, hammerHitboxRange);
        //ground check
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x - 1f, groundCheck.transform.position.y - 0.1f));
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f));
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x + 1f, groundCheck.transform.position.y - 0.1f));

        //(Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.2f), new Vector2(gripLeftCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform"))
        //Physics2D.Linecast(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.2f), new Vector2(gripLeftCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")))
        Gizmos.DrawLine(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.65f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.65f));
        //roof check
        //Gizmos.DrawLine(new Vector2(transform.position.x - 1.175f, gripLeftCheck.position.y + 2.2f), new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 2.2f));

        //DRAW RAYCASR
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y - 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y - 2.2f));
        
        // Test
        //Gizmos.DrawLine(new Vector2(transform.position.x - 1f, gripLeftCheck.position.y + 2.2f), new Vector2(gripRightCheck.position.x - 0.125f, gripLeftCheck.position.y + 2.2f));

    }

    public enum Action
    {
        Idle,
        Run,
        Jump,
        Attack,
        GripLeft,
        GripRight,
        Projected
    }
}
