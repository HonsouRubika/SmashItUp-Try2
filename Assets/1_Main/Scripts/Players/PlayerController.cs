using UnityEngine;
using UnityEngine.InputSystem;

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
    [Range(0.01f, 1f)]
    public float ratioAddForce = 1;
    private float movementActu = 0;
    
    //TODO : GameMode avec Hp
    //public uint health; 

    //Player Controll Asignation
    /*[System.NonSerialized]*/ public uint playerID = 0;

    //Sprite et animation
    public Rigidbody2D rb;
    public BoxCollider2D bc;

    //jump variable
    [Header("Jump")]
    public JumpState jumpState = JumpState.InFlight;
    public uint nbJump = 2;
    private uint nbJumpActu = 0;
    public float maxJumpHigh = 1;
    public float minJumpHeigh = 0.5f;
    private float startJumpPosition;
    private float startWallJumpPosition;
    public float maxWallJumpHigh = 1;
    public float wallJumpSpeed = 1;
    public float wallJumpMovementFreeze = 0.2f;
    private float wallJumpMovementFreezeActuL, wallJumpMovementFreezeActuR;
    public float numberMaxWalljump = 2;
    private float numberMaxWalljumpActu;
    //coyot time
    private bool coyoteTimeCheck = false;
    private bool coyoteTimeDone = false;
    public float coyotTime = 0.1f;
    private float coyotTimeActu;
    private float shaitanerieDUnity = 1f;
    private float shaitanerieDUnityActu = 0;
    //wallgrip
    public float wallGripTime = 2;
    private float wallGripTimeActu;
    private bool isWallGripStarted = false;
    public float wallGripFallSpeed = 0;
    private bool isJump = false;
    private bool isWallJump = false;

    //test sur le jump: add force
    [Range(0.001f, 0.1f)]
    public float jumpRatioAddForce = 0.1f;
    [Range(0.001f, 0.1f)]
    public float wallJumpRatioAddForce = 0.1f;
    private float jumpMovementActu = 1;
    private bool isJumpFallSetted = false;

    //Colision checks
    [Header("GroundCheck")]
    public Transform groundCheck;
    public Transform gripLeftCheck;
    public Transform gripRightCheck;
    private bool isGrippingLeft = false, isGrippingRight = false;

    ///////////Attack///////////
    //Hitbox
    [Header("Attack")]
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
    public float attackDuration = 0.1f;
    private float attackDurationActu;
    private bool isAttackRunningL, isAttackRunningR;
    private bool didAttackedBlockedL, didAttackedBlockedR;
    private float nextAttackTime = 0f;

    [Header("Stun")]
    public bool disableCollider = false;
    public float stunTime = 0.5f;
    private float stunTimeActu;
    public bool isFrozen = false;
    public float blockStunTime = 0.5f;
    private float blockStunTimeActu;

    [System.NonSerialized] public float lastTimeAttackHit = 0;
    [System.NonSerialized] public float lastTimeGotHit = 0;

    [HideInInspector] public int playerIDHit;
    private float timeAttack = 0;
    [HideInInspector] public bool hitPlayer = false;

    //Animation
    private PlayerAnim playerAnimScript;
    [HideInInspector] public Transform playerAnimator;

    //Bonus
    [System.NonSerialized] public bool isUnbreakable = false;


    void Start()
    {
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

        PlayerSoundScript = GetComponentInChildren<PlayerSound>();
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
        if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
        {
            //stun
            if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu && !isFrozen)
            {
                if (context.started)
                {
                    computeJump();
                }
                else if (context.canceled && transform.position.y >= (minJumpHeigh + startJumpPosition))
                {
                    shaitanerieDUnityActu = Time.time; //ne touche pas à ça
                    //le perso descend car il relache la touche de saut
                    startJumpPosition = transform.position.y - maxJumpHigh;
                }
                else if (context.canceled && transform.position.y < (minJumpHeigh + startJumpPosition))
                {
                    //dois s'arreter au minimum de saut
                    shaitanerieDUnityActu = Time.time;
                    startJumpPosition = transform.position.y - maxJumpHigh + minJumpHeigh;
                }
            }
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
            //Enable back collision between players 
            if (disableCollider)
            {
                Physics2D.IgnoreLayerCollision(8, 8, false);
            }

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
        else if (isFrozen)
        {
            rb.velocity = new Vector2(0, 0);
            playerAnimScript.Expulsion(false);
            //do nothing
            //player is frozen during TRANSITION
        }
        else if (playerAnimScript != null && PlayerSoundScript != null && playerAnimator != null) //player is stun
        {
            //anim
            playerAnimScript.Expulsion(true);

            //Disable the collision between players when player are stunt
            if (disableCollider)
            {
                Physics2D.IgnoreLayerCollision(8, 8, true);
            }
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
    }

    void attack()
    {
        //Attaque droite et gauche
        //Gauche
        //2)animation attaque + verif block
        if (isAttackRunningL && Time.time < attackDurationActu)
        {
            //Detection d'un blocage
            Collider2D[] hammers = Physics2D.OverlapCircleAll(hammerPointL.transform.position, hammerHitboxRange, hammerHitboxLayer);

            if (hammers.Length > 1)
            {
                //on contre
                //Debug.Log("Blocage à Gauche");

                didAttackedBlockedL = true;
            }

        }
        //3) applyAttack
        if (isAttackRunningL && Time.time >= attackDurationActu)
        {
            //Debug.Log("Attaque gauche3");
            //Animation / Attack hitbox Apparition (pour test)

            //Detection des player dans la zone
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointL.position, attackRange, enemyLayer);

            if (!didAttackedBlockedL)
            {
                //On leur applique une velocit� (effet de l'attaque)
                foreach (Collider2D enemy in hitEnemies)
                {
                    //Appliquer une velocit�
                    //Attention: check la direction pour coord x
                    enemy.GetComponent<PlayerController>().applyAttack(-hammerXProjection, hammerYProjection);
                    lastTimeAttackHit = Time.time;
                    playerIDHit = (int)enemy.GetComponent<PlayerController>().playerID;
                }
            }
            else
            {
                // apply self blockProjection
                applyBlock(hammerBlockProjection, 0);
            }
            //reset var
            didAttackedBlockedL = false;
            isAttackRunningL = false;
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
                didAttackedBlockedR = true;
            }
        }
        //3) applyAttack
        if (isAttackRunningR && Time.time >= attackDurationActu)
        {
            //reset timeAttack
            //nextAttackTime = Time.time + 1f / attackRate;

            //Animation / Attack hitbox Apparition (pour test)

            //Detection des player dans la zone
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointR.position, attackRange, enemyLayer);

            if (!didAttackedBlockedR)
            {
                //On leur applique une velocit� (effet de l'attaque)
                foreach (Collider2D enemy in hitEnemies)
                {
                    //Appliquer une velocit�
                    //Attention: check la direction pour coord x
                    enemy.GetComponent<PlayerController>().applyAttack(hammerXProjection, hammerYProjection);
                    lastTimeAttackHit = Time.time;
                    playerIDHit = (int)enemy.GetComponent<PlayerController>().playerID;
                }
            }
            else
            {
                // apply self blockProjection
                applyBlock(-hammerBlockProjection, 0);
            }
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
        if ((movementInput.x < -0.3) && !isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && !isAttackRunningL && !isAttackRunningR && (jumpState != JumpState.InFlight && jumpState != JumpState.Falling))
        {
            //gauche
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            movementActu = -1;
            attackDirection = true;

            /* 
            //AddForce
            movementActu -= ratioAddForce;
            if (movementActu < -1) movementActu = -1;
            rb.velocity = new Vector2(movementActu * speed, rb.velocity.y);
            */

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
                PlayerSoundScript.Run();
            }
        }
        else if ((movementInput.x > 0.3) && !isGrippingRight && Time.time >= wallJumpMovementFreezeActuR && !isAttackRunningL && !isAttackRunningR && (jumpState != JumpState.InFlight && jumpState != JumpState.Falling))
        {
            //droite
            rb.velocity = new Vector2(speed, rb.velocity.y);
            movementActu = 1;
            attackDirection = false;

            /*
            //AddForce
            movementActu += ratioAddForce;
            if (movementActu > 1) movementActu = 1;
            rb.velocity = new Vector2(movementActu * speed, rb.velocity.y);
            */

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
                PlayerSoundScript.Run();
            }
        }
        //en l'air
        else if ((movementInput.x < -0.3) && !isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && !isAttackRunningL && !isAttackRunningR && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling))
        {
            //gauche
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            //TODO : temp de pause quand changement de direction lors d'un saut
            //  Voir Bloc note "to do SUI" sur le bureau
            //rb.velocity = new Vector2(-movementJumpSpeed, rb.velocity.y);
            attackDirection = true;

            movementActu -= ratioAddForce;
            if (movementActu < -1) movementActu = -1;
            rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);

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

                if (jumpState == JumpState.Grounded)
                {
                    PlayerSoundScript.Run();
                }
            }
        }
        else if ((movementInput.x > 0.3) && !isGrippingRight && Time.time >= wallJumpMovementFreezeActuR && !isAttackRunningL && !isAttackRunningR && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling))
        {
            //droite
            //rb.velocity = new Vector2(movementJumpSpeed, rb.velocity.y);
            attackDirection = false;

            movementActu += ratioAddForce;
            if (movementActu > 1) movementActu = 1;
            rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);

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

                if (jumpState == JumpState.Grounded)
                {
                    PlayerSoundScript.Run();
                }
            }
        }
        else if (jumpState == JumpState.Grounded && !isBeingProjected)
        {
            //Debug.Log("x axe stop cause grounded");
            //rb.velocity = new Vector2(0, rb.velocity.y);

            if(movementActu > 0)
            {
                movementActu -= ratioAddForce;
                if (movementActu < 0) movementActu = 0;
            } 
            else if (movementActu < 0)
            {
                movementActu += ratioAddForce;
                if (movementActu > 0) movementActu = 0;
            }
            rb.velocity = new Vector2(movementActu * movementJumpSpeed, rb.velocity.y);

            //Action (anim)
            actionState = Action.Idle;

            //anim
            playerAnimScript.Running(false);
            playerAnimScript.Idle(true);
            playerAnimScript.WallSlide(false);
        }
        // grip fall au wall
        else if ((isGrippingLeft || isGrippingRight) && jumpState == JumpState.Falling)
        {
            if(!isWallGripStarted)
            {
                //le perso s'arrete un temps
                wallGripTimeActu = wallGripTime + Time.time;
                isWallGripStarted = true;
            }
            else
            {
                Debug.Log("wall grip stun");
                rb.velocity = new Vector2(0, 0);
            }

            if(Time.time >= wallGripTimeActu)
            {
                //le perso doit glisser du mur
                rb.velocity = new Vector2(rb.velocity.x, - wallGripFallSpeed);
                isWallGripStarted = false;
            }
        }
        else if (!isGrippingLeft && !isGrippingRight)
        {
            isWallGripStarted = false;
        }
        else if (isGrippingLeft)
        {
            //Debug.Log("le perso doit glisser du mur");
            playerAnimScript.WallSlide(true);
            playerAnimator.localScale = new Vector2(-Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
        }
        else if (isGrippingRight)
        {
            //Debug.Log("le perso doit glisser du mur");
            playerAnimScript.WallSlide(true);
            playerAnimator.localScale = new Vector2(Mathf.Abs(playerAnimator.localScale.x), playerAnimator.localScale.y);
        }
        else
        {
            playerAnimScript.WallSlide(false);
        }

        ///// JUMP CURVE /////
        if ((transform.position.y >= startJumpPosition + maxJumpHigh || isJumpFallSetted) && jumpState != JumpState.Grounded && !isWallGripStarted)
        {
            if (!isJumpFallSetted) isJumpFallSetted = true;

            //determine curve
            if (jumpMovementActu > -1 && isJump) jumpMovementActu -= jumpRatioAddForce;
            else if (jumpMovementActu > -1 && isWallJump) jumpMovementActu -= wallJumpRatioAddForce;
            else if (jumpMovementActu < -1) jumpMovementActu = -1;

            //apply curve
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpMovementActu);

            //check if player is falling
            if (jumpMovementActu < 0 && jumpState != JumpState.Falling)
            {
                jumpState = JumpState.Falling;
                playerAnimScript.Falling(true);
            }
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
        (Physics2D.Linecast(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x +1f, groundCheck.transform.position.y - 0.1f), 1 << LayerMask.NameToLayer("Player"))))   /*Droite*/
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
                //reset var for walljump
                wallJumpMovementFreezeActuL = Time.time;
                wallJumpMovementFreezeActuR = Time.time;

                //addForce = null
                jumpMovementActu = 0;

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
            if(!coyoteTimeCheck)
            {
                coyotTimeActu = Time.time + coyotTime;
                coyoteTimeCheck = true;
            }

            if(rb.velocity.y <= 0 && !isWallGripStarted)
            {
                //le perso chute
                //vitesse de chute constante
                jumpState = JumpState.Falling;
                rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed);
            }
            else if (jumpState == JumpState.Grounded) // ne change pas si jumpState = JumpState.Falling
            {
                jumpState = JumpState.InFlight;
            }
        }

        //Colision Side GripCheck
        //float height = bc.size.y;
        if (Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Plateform")) || 
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x , gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform"))||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ) 
            && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling )) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Plateform")))
        {
            isGrippingLeft = true;
            isBeingProjected = false;

            //Action (anim)
            actionState = Action.GripLeft;
        }
        else
        {
            isGrippingLeft = false;
        }
        if (Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ) 
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
        if ((jumpState == JumpState.Grounded || (jumpState != JumpState.Grounded && Time.time < coyotTimeActu && coyoteTimeCheck && !coyoteTimeDone && !isGrippingRight && !isGrippingLeft) || (nbJumpActu != nbJump && !isGrippingRight && !isGrippingLeft)) && !isAttackRunningL && !isAttackRunningR)
        {
            //coyot time
            if (jumpState != JumpState.Grounded && Time.time < coyotTimeActu && coyoteTimeCheck && !coyoteTimeDone) coyoteTimeDone = true;

            //double jump
            nbJumpActu++;

            //block le check du ground : ne pas toucher
            shaitanerieDUnityActu = Time.time + shaitanerieDUnity;

            //Debug.Log("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            actionState = Action.Jump;
            startJumpPosition = transform.position.y;
            //anim
            playerAnimScript.Jumping(true);
            playerAnimScript.WallSlide(false);
            PlayerSoundScript.Jump();

            //jump or walljump
            isJump = true;
            isWallJump = false;

            //Jump Curve
            jumpMovementActu = 1;
            isJumpFallSetted = false;
        }
        else if (isGrippingRight && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            jumpState = JumpState.InFlight;
            //jump to the left w/ 45� angle
            rb.velocity = new Vector2(-wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
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

            //jump or walljump
            isJump = false;
            isWallJump = true;

            //Jump Curve
            jumpMovementActu = 1;
            isJumpFallSetted = false;
        }
        else if (isGrippingLeft && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            jumpState = JumpState.InFlight;
            //jump to the right w/ 45� angle
            rb.velocity = new Vector2(wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
            actionState = Action.Jump;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;
            //block le check du ground
            shaitanerieDUnityActu = Time.time + 10;
            //freeze movement for small time
            wallJumpMovementFreezeActuL = wallJumpMovementFreeze + Time.time;

            //addForce
            movementActu = 1;

            //jump or walljump
            isJump = false;
            isWallJump = true;

            //Jump Curve
            jumpMovementActu = 1;
            isJumpFallSetted = false;
        }

        //Debug.Log(" 2) JumpState : " + jumpState + ", coyoteTimeCheck : " + coyoteTimeCheck);
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

            //apparition hammerHitBox
            hammerPointL.SetActive(true);

            //anim
            playerAnimScript.Attack();
            PlayerSoundScript.HammerPouet();
        }
        //Droite
        else if (!attackDirection && Time.time >= nextAttackTime)
        {
            //reset timeAttack
            nextAttackTime = Time.time + attackRate;

            isAttackRunningR = true;
            attackDurationActu = attackDuration + Time.time;
            //apparition hammerHitBox
            hammerPointR.SetActive(true);

            //anim
            if(playerAnimScript != null)
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
            if(PlayerSoundScript != null)
            {
                PlayerSoundScript.HammerPouet();
            }
            else
            {
                Debug.LogWarning("Error: Le PlayerSoundScript est null");
            }
            
        }
    }

    void applyAttack(float velocityX, float velocityY)
    {
        //Action (anim)
        actionState = Action.Projected;

        //Stun
        stunTimeActu = stunTime + Time.time;

        //var pour mini jeu
        lastTimeGotHit = Time.time;

        //gestion distance max
        isBeingProjected = true;
        startProjectedPostion = transform.position.x;

        //Velocit�
        rb.velocity = new Vector2(velocityX, velocityY);
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPointR.position, attackRange);
        Gizmos.DrawWireSphere(attackPointL.position, attackRange);
        Gizmos.DrawWireSphere(hammerPointL.transform.position, hammerHitboxRange);
        Gizmos.DrawWireSphere(hammerPointR.transform.position, hammerHitboxRange);
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x - 1f, groundCheck.transform.position.y - 0.1f));
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f));
        Gizmos.DrawLine(groundCheck.transform.position, new Vector2(groundCheck.transform.position.x + 1f, groundCheck.transform.position.y - 0.1f));
        /* DRAW RAYCASR
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y - 0.5f));
        Gizmos.DrawLine(transform.position, new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y - 2.2f));
        // Test
        Gizmos.DrawLine(new Vector2(transform.position.x, gripLeftCheck.position.y + 2.15f), new Vector2(gripRightCheck.position.x, gripLeftCheck.position.y + 2.15f));
        */
        }

    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Falling,
        Landed
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
