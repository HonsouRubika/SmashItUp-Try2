using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //old input
    //[Header("Inputs")]
    public KeyCode leftKey, rightkey, jumpKey, attackKey;
    //new input
    private CharacterController controller;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool attacked = false;

    public float speed = 3;
    public float jumpSpeed = 3;

    public uint health;
    public bool controlEnabled = true;

    //Player Controll Asignation
    public uint playerID = 0;

    //Sprite et animation
    private Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    //jump variable
    private float oldYPosition;
    private float startJumpPosition;
    public float maxJumpHigh = 1;
    public float wallJumpSpeed = 1;
    private JumpState jumpState = JumpState.InFlight;
    public float wallJumpMovementFreeze = 0.2f;
    private float wallJumpMovementFreezeActuL, wallJumpMovementFreezeActuR;

    //Colision checks
    public Transform groundCheck;
    private bool isGrippingLeft = false, isGrippingRight = false;

    ///////////Attack///////////
    //Hitbox
    public Transform attackPointL, attackPointR;
    public GameObject hammerPointL, hammerPointR;
    public float attackRange = 0.5f;
    public float hammerHitboxRange = 0.75f;
    public LayerMask enemyLayer, hammerHitboxLayer;
    //Force et Distance de projection
    public float hammerProjection = 3;
    public float hammerBlockProjection = 1.5f;
    public float HammerSideProjectionMaxDistance = 1;
    private float startProjectedPostion = 0;
    //private bool selfProjectionDirection = false;
    public bool isBeingProjected = false;
        //false = droite; true = gauche
    //Paramètre vitesse
    private bool attackDirection = false;
    public float attackRate = 2f;
    public float attackDuration = 0.1f;
    private float attackDurationActu;
    private bool isAttackRunningL, isAttackRunningR;
    private bool didAttackedBlockedL, didAttackedBlockedR;
    float nextAttackTime = 0f;
    public float stunTime = 0.5f;
    private float stunTimeActu;

    public float lastTimeAttackHit = 0;
    public float lastTimeGotHit = 0;

    //Animation
    private PlayerAnim playerAnimScript;
    private Transform playerAnim;

    void Start()
    {

        //init var
        rb = GetComponent<Rigidbody2D>();
        oldYPosition = transform.position.y;
        startJumpPosition = transform.position.y;
        stunTimeActu = 0;
        wallJumpMovementFreezeActuL = 0;
        wallJumpMovementFreezeActuR = 0;
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
        movementInput = context.ReadValue<Vector2>();
        //TODO: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/ActionBindings.html
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }
    public void OnAttacked(InputAction.CallbackContext context)
    {
        attacked = context.action.triggered;
    }

    private void Update()
    {
        //Distance de projection max
        //gauche
        if ((transform.position.x <= startProjectedPostion + -HammerSideProjectionMaxDistance) && isBeingProjected)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isBeingProjected = false;
            Debug.Log("OUI gauche");
            Debug.Log("Dist max : " + -HammerSideProjectionMaxDistance);
            Debug.Log("1: " + transform.position.x + " >= " + startProjectedPostion + " + " + -HammerSideProjectionMaxDistance);

        } //droite
        else if ((transform.position.x >= startProjectedPostion + HammerSideProjectionMaxDistance) && isBeingProjected)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isBeingProjected = false;
            Debug.Log("OUI droite");
            Debug.Log("Dist max : " + HammerSideProjectionMaxDistance);
            Debug.Log("1: " + transform.position.x + " <= " + startProjectedPostion + " + " + HammerSideProjectionMaxDistance);
        }

        //stun also equal to immortality
        if (Time.time >= stunTimeActu)
        {
            //anim
            if (playerAnimScript.playerAnimator != null)
            {
                playerAnimScript.Expulsion(false);
            }
            
            //reset var
            stunTimeActu = 0;
            //isBeingProjected = false;

            /////////////////////////////////////
            //////////// DEPLACEMENT ////////////
            /////////////////////////////////////
            #region Deplacement
            //Gauche + Droite
            if ((movementInput.x < 0) && (!isGrippingLeft || jumpState == JumpState.Grounded) && Time.time >= wallJumpMovementFreezeActuL && !isAttackRunningL && !isAttackRunningR)
            {
                //gauche
                //rb.velocity = new Vector2(-speed, rb.velocity.y);
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                attackDirection = true;

                //anim
                playerAnim.localScale = new Vector2(Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
                if (playerAnimScript.playerAnimator != null)
                {
                    playerAnimScript.Running(true);
                }
            }
            else if ((movementInput.x > 0) && (!isGrippingRight || jumpState == JumpState.Grounded) && Time.time >= wallJumpMovementFreezeActuR && !isAttackRunningL && !isAttackRunningR)
            {
                //droite
                rb.velocity = new Vector2(speed, rb.velocity.y);
                attackDirection = false;

                //anim
                playerAnim.localScale = new Vector2(-Mathf.Abs(playerAnim.localScale.x), playerAnim.localScale.y);
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

            //Saut + wall jump
            if (jumped && jumpState == JumpState.Grounded && !isAttackRunningL && !isAttackRunningR)
            {
                //Debug.Log("Jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                jumpState = JumpState.InFlight;
                startJumpPosition = transform.position.y;

                //anim
                playerAnimScript.Jumping(true);
            }
            else if (jumped && isGrippingRight && !isAttackRunningL && !isAttackRunningR)
            {
                //jump to the left w/ 45� angle
                rb.velocity = new Vector2(-wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
                jumpState = JumpState.InFlight;
                startJumpPosition = transform.position.y;

                //freeze movement for small time
                wallJumpMovementFreezeActuR = wallJumpMovementFreeze + Time.time;
            }
            else if (jumped && isGrippingLeft && !isAttackRunningL && !isAttackRunningR)
            {
                //jump to the right w/ 45� angle
                rb.velocity = new Vector2(wallJumpSpeed, jumpSpeed / (Mathf.Sqrt(2) / 2));
                jumpState = JumpState.InFlight;
                startJumpPosition = transform.position.y;

                //freeze movement for small time
                wallJumpMovementFreezeActuL = wallJumpMovementFreeze + Time.time;
            }

            //Hauteur max
            if (transform.position.y > startJumpPosition + maxJumpHigh)
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
                oldYPosition = transform.position.y;
            }
            #endregion

            /////////////////////////////////
            //////////// ATTAQUE ////////////
            /////////////////////////////////

            //Attaque droite et gauche
            //Gauche
            //1) debut attaque
            
            if (attacked && attackDirection && Time.time >= nextAttackTime)
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
                    }
                }
                else
                {
                    // apply self blockProjection
                    applyAttack(hammerBlockProjection, 0);
                }
                //reset var
                didAttackedBlockedL = false;
                isAttackRunningL = false;
                //disparition hammerHitBox
                hammerPointL.SetActive(false);

            }


            //Droite
            //1) debut attaque
            if (attacked && !attackDirection && Time.time >= nextAttackTime)
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
                    }
                }
                else
                {
                    // apply self blockProjection
                    applyAttack(-hammerBlockProjection, 0);
                }
                //reset var
                didAttackedBlockedR = false;
                isAttackRunningR = false;
                //disparition hammerHitBox
                hammerPointR.SetActive(false);
            }
            
        }
        else
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
