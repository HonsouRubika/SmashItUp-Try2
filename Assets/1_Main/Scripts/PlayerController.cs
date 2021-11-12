using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //new input
    private CharacterController controller;
    private Vector2 movementInput = Vector2.zero;

    [Header("Déplacement")]
    public float speed = 3;
    public float jumpSpeed = 3;

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
    private JumpState jumpState = JumpState.InFlight;
    public float wallJumpMovementFreeze = 0.2f;
    private float wallJumpMovementFreezeActuL, wallJumpMovementFreezeActuR;
    public float numberMaxWalljump = 2;
    private float numberMaxWalljumpActu;

    //Colision checks
    [Header("GroundCheck")]
    public Transform groundCheck;
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
    public float hammerProjection = 3;
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
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            movementInput = context.ReadValue<Vector2>();
        }     
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
        {
            //stun
            if (Time.time >= stunTimeActu && Time.time >= blockStunTimeActu)
            {
                if (context.started) computeJump();
            }
        }
    }
    public void OnAttacked(InputAction.CallbackContext context)
    {
        if (!PauseMenu.isPaused)
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
                    enemy.GetComponent<PlayerController>().applyAttack(-hammerProjection, 0);
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
                    enemy.GetComponent<PlayerController>().applyAttack(hammerProjection, 0);
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
        if ((movementInput.x < -0.3) && (!isGrippingLeft || jumpState == JumpState.Grounded) && Time.time >= wallJumpMovementFreezeActuL && !isAttackRunningL && !isAttackRunningR)
        {
            //gauche
            //rb.velocity = new Vector2(-speed, rb.velocity.y);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            attackDirection = true;

            //anim
            if (!PauseMenu.isPaused)
            {
                playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
            }
            
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
            }
        }
        else if ((movementInput.x > 0.3) && (!isGrippingRight || jumpState == JumpState.Grounded) && Time.time >= wallJumpMovementFreezeActuR && !isAttackRunningL && !isAttackRunningR)
        {
            //droite
            rb.velocity = new Vector2(speed, rb.velocity.y);
            attackDirection = false;

            //anim
            if (!PauseMenu.isPaused)
            {
                playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
            }
            
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Running(true);
            }
        }
        else if (jumpState != JumpState.InFlight)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            //anim
            playerAnimScript.Running(false);
            playerAnimScript.Idle(true);
        }

        //Hauteur max
        if (transform.position.y > startJumpPosition + maxJumpHigh)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 0.05f);
        }
        //hauteur max wall jump
        if (transform.position.y > startWallJumpPosition + maxWallJumpHigh)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 0.05f);
        }

        //Colision Sol
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Plateform")))
        {
            //le perso touche le sol
            jumpState = JumpState.Grounded;
            startJumpPosition = transform.position.y;
            numberMaxWalljumpActu = 0; //reset nb de walljump

            //reset var for walljump
            wallJumpMovementFreezeActuL = Time.time;
            wallJumpMovementFreezeActuR = Time.time;

            //anim
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Jumping(false);
            }
        }
        else
        {
            jumpState = JumpState.InFlight;
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
        //Saut + wall jump
        if (jumpState == JumpState.Grounded && !isAttackRunningL && !isAttackRunningR)
        {
            //Debug.Log("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumpState = JumpState.InFlight;
            startJumpPosition = transform.position.y;

            //anim
            playerAnimScript.Jumping(true);
        }
        else if (isGrippingRight && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            //jump to the left w/ 45� angle
            rb.velocity = new Vector2(-wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
            jumpState = JumpState.InFlight;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;

            //freeze movement for small time
            wallJumpMovementFreezeActuR = wallJumpMovementFreeze + Time.time;
        }
        else if (isGrippingLeft && !isAttackRunningL && !isAttackRunningR && numberMaxWalljumpActu < numberMaxWalljump)
        {
            //jump to the right w/ 45� angle
            rb.velocity = new Vector2(wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
            jumpState = JumpState.InFlight;
            startJumpPosition = transform.position.y;
            startWallJumpPosition = transform.position.y;
            numberMaxWalljumpActu++;

            //freeze movement for small time
            wallJumpMovementFreezeActuL = wallJumpMovementFreeze + Time.time;
        }
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

    void OnCollisionStay2D(Collision2D col)
    {
        //Exception anti "grip" sur le cot� des plateforme
        //Bug: Le grip ne s'effectue pas au niveau des pieds. Solution : Changer la HitBox (� faire apr�s changement du sprite et animations)

        if (jumpState == JumpState.InFlight && col.gameObject.tag == "Plateform")
        {
            if (col.gameObject.transform.position.x <= transform.position.x)
            {
                isGrippingLeft = true;
                //Debug.Log("Gripping Left");
            }
            else
            {
                isGrippingRight = true;
                //Debug.Log("Gripping Right");
            }

        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Plateform" && jumpState == JumpState.InFlight)
        {
            isGrippingLeft = false;
            isGrippingRight = false;
            //Debug.Log("Just stopped gripping");
        }
    }

    public enum JumpState
    {
        Grounded,
        PrepareToJump,
        Jumping,
        InFlight,
        Landed
    }
}
