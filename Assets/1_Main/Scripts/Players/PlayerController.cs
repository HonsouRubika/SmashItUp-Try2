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
    public float jumpHoldTimer = 0.1f;
    private float jumpHoldTimerActu = 0;
    private bool isJumpHoldTimerSetted = false;
    private bool isWallJumpHoldTimerSetted = false;
    private bool coyoteTimeCheck = false;
    private float shaitanerieDUnity = 1f;
    private float shaitanerieDUnityActu = 0;
    public float wallGripFallSpeed = 0;
    private bool isJump = false;
    private bool isWallJump = false;


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
    float nextAttackTime = 0f;

    [Header("Stun")]
    public float stunTime = 0.5f;
    private float stunTimeActu;
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
            if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu)
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

    private void Update()
    {
        PlayerAttacked();

        verifProjectionMax();

        //stun
        if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu)
        {
            //anim
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Expulsion(false);
            }
            
            //reset var
            stunTimeActu = 0;
            blockStunTimeActu = 0;
            //isBeingProjected = false;

            /////////////////////////////////////
            //////////// DEPLACEMENT ////////////
            /////////////////////////////////////
            move();

            /////////////////////////////////
            //////////// ATTAQUE ////////////
            /////////////////////////////////

            attack();
            
        }
        else //player is stun
        {
            //anim
            playerAnimScript.Expulsion(true);
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
            nextAttackTime = Time.time + 1f / attackRate;

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
        else if ((isGrippingLeft || isGrippingRight) && jumpState == JumpState.Falling)
        {
            //Debug.Log("le perso doit glisser du mur");
            rb.velocity = new Vector2(rb.velocity.x, - wallGripFallSpeed);
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

        //Hauteur max
        if (transform.position.y > startJumpPosition + maxJumpHigh && !isJumpHoldTimerSetted && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            jumpHoldTimerActu = Time.time + jumpHoldTimer;
            isJumpHoldTimerSetted = true;
        }
        if (jumpState == JumpState.Falling && Time.time >= jumpHoldTimerActu && (isJumpHoldTimerSetted && !isWallJumpHoldTimerSetted) && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed);
            jumpState = JumpState.Falling;
            playerAnimScript.Falling(true);
            jumpHoldTimerActu = 0;
        }

        //hauteur max wall jump
        if (transform.position.y > startWallJumpPosition + maxWallJumpHigh && !isWallJumpHoldTimerSetted && isWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            jumpHoldTimerActu = Time.time + jumpHoldTimer;
            isWallJumpHoldTimerSetted = true;
        }
        if (jumpState == JumpState.Falling && Time.time >= jumpHoldTimerActu && isWallJumpHoldTimerSetted && isWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed);
            jumpState = JumpState.Falling;
            playerAnimScript.Falling(true);
            jumpHoldTimerActu = 0;
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
                playerAnimScript.Falling(false);
                startJumpPosition = transform.position.y;
                numberMaxWalljumpActu = 0; //reset nb de walljump
                isJumpHoldTimerSetted = false;
                isWallJumpHoldTimerSetted = false;
                jumpHoldTimerActu = 0;
                isBeingProjected = false;
                coyoteTimeCheck = true;
                //reset var for walljump
                wallJumpMovementFreezeActuL = Time.time;
                wallJumpMovementFreezeActuR = Time.time;

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
            if(rb.velocity.y < 0)
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
        //Debug.Log(" 1 from " + transform.position + " to " + groundCheck.transform.position);

        //Colision Side GripCheck
        float height = bc.size.y;
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
            isWallJumpHoldTimerSetted = false;
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
            isWallJumpHoldTimerSetted = false;
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
        //Debug.Log("JumpState : " + jumpState + ", coyoteTimeCheck : " + coyoteTimeCheck);
        //Debug.Log(" Grip left : " + isGrippingLeft + ", right : " + isGrippingRight);

        if ((jumpState == JumpState.Grounded || (jumpState != JumpState.Grounded && coyoteTimeCheck == true)) && !isAttackRunningL && !isAttackRunningR)
        {
            //Coyot time check
            coyoteTimeCheck = false;
            //block le check du ground
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
        }

        //Debug.Log(" 2) JumpState : " + jumpState + ", coyoteTimeCheck : " + coyoteTimeCheck);
    }

    void computeAttack()
    {
        //Gauche
        if (attackDirection && Time.time >= nextAttackTime)
        {
            //reset timeAttack
            nextAttackTime = Time.time + 1f / attackRate;

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
            nextAttackTime = Time.time + 1f / attackRate;

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
                Debug.LogWarning("Error: Le PlayerAnimScript est null");
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
