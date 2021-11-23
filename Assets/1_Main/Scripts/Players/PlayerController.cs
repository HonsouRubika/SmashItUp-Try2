using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //new input
    private CharacterController controller;
    private Vector2 movementInput = Vector2.zero;
    private PlayerSound PlayerSoundScript;

    [Header("Déplacement")]
    public float speed = 3;
    public float jumpSpeed = 3;
    public float movementJumpSpeed = 3;

    //TODO : GameMode avec Hp
    //public uint health; 

    //Player Controll Asignation
    /*[System.NonSerialized]*/ public uint playerID = 0;

    //Sprite et animation
    private Rigidbody2D rb;

    //jump variable
    [Header("Jump")]
    public float maxJumpHigh = 1;
    private float startJumpPosition;
    private float startWallJumpPosition;
    public float maxWallJumpHigh = 1;
    public float wallJumpSpeed = 1;
    public JumpState jumpState = JumpState.InFlight;
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
    //private bool selfProjectionDirection = false;
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
    private Transform playerAnim;


    void Start()
    {
        //init var
        rb = GetComponent<Rigidbody2D>();
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
        playerAnim = playerAnimScript.transform;

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
                if (context.started) computeJump();
                else if (context.canceled)
                {
                    shaitanerieDUnityActu = Time.time;
                    //le perso descend car il relache la touche de saut
                    startJumpPosition = transform.position.y - maxJumpHigh;
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
                playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
            }
            else if (transform.position.x >= startProjectedPostion)
            {
                playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            attackDirection = true;
            
            //anim
            if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
            {
                playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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
            attackDirection = false;
            
            //anim
            if (!GameManager.Instance.isPaused && !GameManager.Instance.isShowingPlayers)
            {
                playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
            }
            
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
                playerAnimScript.WallSlide(false);
                PlayerSoundScript.Run();
            }
        }
        //movementJumpSpeed
        else if ((movementInput.x < -0.3) && !isGrippingLeft && Time.time >= wallJumpMovementFreezeActuL && !isAttackRunningL && !isAttackRunningR && (jumpState == JumpState.InFlight || jumpState == JumpState.Falling))
        {
            //gauche
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            //TODO : temp de pause quand changement de direction lors d'un saut
            //  Voir Bloc note "to do SUI" sur le bureau
            rb.velocity = new Vector2(-movementJumpSpeed, rb.velocity.y);
            attackDirection = true;

            //anim
            if (!GameManager.Instance.isPaused)
            {
                playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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
            rb.velocity = new Vector2(movementJumpSpeed, rb.velocity.y);
            attackDirection = false;

            //anim
            if (!GameManager.Instance.isPaused)
            {
                playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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
        else if (jumpState == JumpState.Grounded)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

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
            playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
        }
        else if (isGrippingRight)
        {
            //Debug.Log("le perso doit glisser du mur");
            playerAnimScript.WallSlide(true);
            playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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
        if (jumpState != JumpState.Falling && Time.time >= jumpHoldTimerActu && (isJumpHoldTimerSetted && !isWallJumpHoldTimerSetted) && isJump)
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
        if (jumpState != JumpState.Falling && Time.time >= jumpHoldTimerActu && isWallJumpHoldTimerSetted && isWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpSpeed);
            jumpState = JumpState.Falling;
            playerAnimScript.Falling(true);
            jumpHoldTimerActu = 0;
        }

        //Colision Sol
        if ((Physics2D.Linecast(transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer("Ground"))) ||
            (Physics2D.Linecast(transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer("Plateform"))))
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
            if(jumpState == JumpState.Grounded && rb.velocity.y < 0)
            {
                jumpState = JumpState.Falling;
            }
            else // ne change pas si jumpState = JumpState.Falling
            {
                jumpState = JumpState.InFlight;
            }
        }
        //Debug.Log(" 1 from " + transform.position + " to " + groundCheck.transform.position);

        //Colision Side GripCheck
        if (Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripLeftCheck.position, 1 << LayerMask.NameToLayer("Plateform")) || 
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x , gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform"))||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.1f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y + 2.1f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ) 
            && jumpState == JumpState.InFlight ) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripLeftCheck.position.x, gripLeftCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Plateform")))
        {
            isGrippingLeft = true;
            isWallJumpHoldTimerSetted = false;
        }
        else
        {
            isGrippingLeft = false;
        }
        if (Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, gripRightCheck.position, 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 0.5f), 1 << LayerMask.NameToLayer("Plateform")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.1f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y + 2.1f), 1 << LayerMask.NameToLayer("Plateform")) ||
            ((Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 2.2f), 1 << LayerMask.NameToLayer("Plateform")) ) 
            && jumpState == JumpState.InFlight ) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, new Vector2(gripRightCheck.position.x, gripRightCheck.position.y - 0.5f), 1 << LayerMask.NameToLayer("Plateform")))
        {
            isGrippingRight = true;
            isWallJumpHoldTimerSetted = false;
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
            rb.velocity = new Vector2(0, rb.velocity.y);
            isBeingProjected = false;
            /*
            Debug.Log("OUI gauche");
            Debug.Log("Dist max : " + -HammerSideProjectionMaxDistance);
            Debug.Log("1: " + transform.position.x + " >= " + startProjectedPostion + " + " + -HammerSideProjectionMaxDistance);
            */

        } //droite
        else if ((transform.position.x >= startProjectedPostion + HammerSideProjectionMaxDistance) && isBeingProjected)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isBeingProjected = false;
            /*
            Debug.Log("OUI droite");
            Debug.Log("Dist max : " + HammerSideProjectionMaxDistance);
            Debug.Log("1: " + transform.position.x + " <= " + startProjectedPostion + " + " + HammerSideProjectionMaxDistance);
            */
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
            rb.velocity = new Vector2(0, jumpSpeed);
            //jumpState = JumpState.InFlight;
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
            //jumpState = JumpState.InFlight;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;
            //block le check du ground
            shaitanerieDUnityActu = Time.time + shaitanerieDUnity;

            //freeze movement for small time
            wallJumpMovementFreezeActuR = wallJumpMovementFreeze + Time.time;

            //jump or walljump
            isJump = false;
            isWallJump = true;
        }
        else if (isGrippingLeft && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            jumpState = JumpState.InFlight;
            //jump to the right w/ 45� angle
            rb.velocity = new Vector2(wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
            //jumpState = JumpState.InFlight;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;
            //block le check du ground
            shaitanerieDUnityActu = Time.time + 10;
            //freeze movement for small time
            wallJumpMovementFreezeActuL = wallJumpMovementFreeze + Time.time;

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
            playerAnimScript.Attack();
            PlayerSoundScript.HammerPouet();
        }
    }

    void applyAttack(float velocityX, float velocityY)
    {
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
}
