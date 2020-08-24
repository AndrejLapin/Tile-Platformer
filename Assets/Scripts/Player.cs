using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// BUGS:
// not reseting gravity fast enough after shooting
// not reseting gravity after dive jumping

public class Player : MonoBehaviour
{
    // Config
    [Header("Stats")]
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpMomentum = 5f;
    [SerializeField] float zeroJumpTime = 1f;
    [SerializeField] int additionalJumps = 1;
    [SerializeField] float climbSpeed = 10f;
    [SerializeField] float maxHealth = 3f;
    [SerializeField] float gravityDiveMultiplier = 2f;
    [SerializeField] float rollMultiplier = 2f;

    [Header("Process Hit")]
    [SerializeField] float knockBack = 5f;
    [SerializeField] float knockBackTime = 1f;
    [SerializeField] float damageFrames = 2f;
    


    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float baseDamage = 1f;

    [Header("Death")]
    [SerializeField] float playerDeathVelocity = 20f;
    [SerializeField] float timeBeforeRespawn = 2f;

    //influence perams
    float gravityScale;

    //in game stats
    [SerializeField] float health;
    int jumpCount;

    //[SerializeField] int maxJumps = 1; // currently not using different amount of jumps
    //int currentJumps;

    //croutines
    Coroutine jumpCoroutine;

    //State
    bool isAlive = true;
    bool shooting = false;
    bool rolling = false;
    bool isDiving = false;
    bool inAJump = false;
    bool isTouchingLadder;
    bool isTouchingGround;
    bool invincible = false;
    bool stunned = false;

    // Cached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    GameSession gameSession;
    Checkpoint activeCheckpoint;

    // Constants
    const string RUN_BOOL = "Running";
    const string CLIMB_BOOL = "Climbing";
    const string DIE_BOOL = "Dead";
    const string SHOOT_TRIGGER = "Shoot";
    const string DIVE_BOOL = "Diving";
    const string END_OF_DIVE_TRIGGER = "End of dive";
    const string PROCESS_HIT_BOOL = "Process Hit";
    const string KNOCKBACK_BOOL = "KnockBack";

    // Constant Layers
    const string ENEMY_LAYER = "Enemy";
    const string HAZARD_LAYER = "Hazards";

    private void Awake() // health wasnt updating fast enough, thats why I set health in Awake metheod
    {
        health = maxHealth;
    }

    // Message then methods
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScale = myRigidBody.gravityScale;
        jumpCount = additionalJumps;
        //currentJumps = maxJumps; // wanted to use more than 1 jump
    }

    // Update is called once per frame
    void Update()
    {
        SetGravityScale(gravityScale);
        if (isAlive)
        {
            isTouchingLadder = IsTouchingLadder();
            isTouchingGround = IsTouchingGround();
            if (!shooting && !rolling && !stunned)
            {
                Run();
                Jump();
                FlipSprite();
                ClimLadder();
                //GetDamadged();
                Shoot();
                Dive();
            }
        }
        
    }

    

    //============================== Shooting Related Code =============================

    private void Shoot()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            myAnimator.SetTrigger(SHOOT_TRIGGER); // animation should call LaunchAnArrow() at some point in time
            shooting = true;
        }
    }

    private void LaunchAnArrow()// called in shoot animation at 0:04 
    {
        GameObject playerProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as GameObject;
        playerProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed*transform.localScale.x, 0);
    }

    private void EndShot()// called in shoot animation at 0:05 
    {
        shooting = false;
    }

    //================================= Movment Related Code ==========================

    // Dive Related Code ======================================

    private void Dive()
    {
        float direction;
        //bool touchingLadder = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladders"));

        direction = CrossPlatformInputManager.GetAxis("VerticalK");
        if (direction == -1 && !isTouchingLadder)
        {
            SetingGravityAndDiveAnimationStateToTrue();
        }

        direction = CrossPlatformInputManager.GetAxis("VerticalJ");
        if (direction < -0.5 && !isTouchingLadder)
        {
            SetingGravityAndDiveAnimationStateToTrue();
        }

        if (isTouchingGround)
        {
            isDiving = false;
            myAnimator.SetBool(DIVE_BOOL, false);
            myAnimator.SetTrigger(END_OF_DIVE_TRIGGER); // starts roll animation
        }
    }

    private void SetingGravityAndDiveAnimationStateToTrue()
    {
        SetGravityScale(gravityScale * gravityDiveMultiplier);
        isDiving = true;
        myAnimator.SetBool(DIVE_BOOL, true);
    }

    private void RollStart()// called in roll animation at 0:00 
    {
        rolling = true;
        Vector2 playerVelocity = new Vector2(runSpeed * transform.localScale.x * rollMultiplier, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
    }

    private void RollEnd()// called in roll animation at 0:04 
    {
        rolling = false;
        Vector2 playerVelocity = new Vector2(0, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
    }

    // Run ========================================

    private void Run()
    {
        float controlThrow;
        Vector2 playerVelocity;
        
        controlThrow = CrossPlatformInputManager.GetAxis("HorizontalJ");
        playerVelocity = new Vector2(controlThrow * runSpeed , myRigidBody.velocity.y);

        if (CrossPlatformInputManager.GetButton("HorizontalK"))
       {
            controlThrow = CrossPlatformInputManager.GetAxis("HorizontalK");
            playerVelocity = new Vector2(controlThrow * runSpeed , myRigidBody.velocity.y);
       }

       else if (CrossPlatformInputManager.GetButtonUp("HorizontalK"))
       {
            playerVelocity = new Vector2(0, myRigidBody.velocity.y);
       }

        myRigidBody.velocity = playerVelocity;


        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool(RUN_BOOL, playerHasHorizontalSpeed);
    }

    // Jump related ===============================

    private void Jump()
    {
        if (!isTouchingGround)
        {
            if (jumpCount > 0)
            {
                if (CrossPlatformInputManager.GetButtonDown("Jump"))
                {
                    jumpCount--;
                    ExecuteJump();
                }             
            }
            return;
        }
         
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            ExecuteJump();
        }

        if(isTouchingGround)
        {
            jumpCount = additionalJumps;
        }

    }

    private void ExecuteJump()
    {
        SetGravityScale(0);

        Vector2 jumpVelocity = new Vector2(myRigidBody.velocity.x, jumpMomentum);
        myRigidBody.velocity = jumpVelocity;

        inAJump = true;
        StartCoroutine(EndJumpState());
    }

    IEnumerator EndJumpState()
    {
        yield return new WaitForSeconds(zeroJumpTime);
        inAJump = false;
    }

    // Climb =====================================

    private void ClimLadder()
    {
        if(!isTouchingLadder)
        {
            myAnimator.SetBool(CLIMB_BOOL, false);
            return;
        }

        SetGravityScale(0);
        float controlThrow;
        Vector2 playerVelocity;

        controlThrow = CrossPlatformInputManager.GetAxis("VerticalJ");
        playerVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);

        if (CrossPlatformInputManager.GetButton("VerticalK"))
        {
            controlThrow = CrossPlatformInputManager.GetAxis("VerticalK");
            playerVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
            
        }

        else if (CrossPlatformInputManager.GetButtonUp("VerticalK"))
        {
            playerVelocity = new Vector2(myRigidBody.velocity.x, 0);
        }
        myRigidBody.velocity = playerVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool(CLIMB_BOOL, playerHasVerticalSpeed);
    }

    // Swim ======================================

    private void Swim()
    {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            //Debug.Log("Not in wota");
            //SetGravityScale(gravityScale);
            return;
        }
        Debug.Log("In WOTA");
        SetGravityScale(0.5f);
    }

    //============================== Checks Related Code ==============================

    private bool IsTouchingGround()
    {
        return myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    private bool IsTouchingLadder() // returns true if touching LADDER layer 
    {
        return myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladders"));
    }

    //================================== Damage ================================

    private void OnTriggerStay2D(Collider2D collision)
    {
        GetDamadged(collision);
    }

    private void GetDamadged(Collider2D collision)
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask(ENEMY_LAYER)) && !invincible)
        {
            health-= collision.GetComponent<EnemyDamage>().GetContactDamage();
            ProcessHit();
            //Debug.Log("Enemy is touching me");

        }
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask(HAZARD_LAYER)) && !invincible)
        {
            health--;
            ProcessHit();
            //Debug.Log("I am touching a trap");
        }
        if (health <= 0 && isAlive == true)
        {
            //Debug.Log("I have 0 health");
            isAlive = false;
            invincible = true;
            myAnimator.SetBool(DIE_BOOL, true);
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, playerDeathVelocity);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        transform.position = new Vector2(activeCheckpoint.transform.position.x, activeCheckpoint.transform.position.y);
        health = maxHealth;
        myAnimator.SetBool(DIE_BOOL, false);
        gameSession.ProcessPlayerDeath();
        isAlive = true;
        invincible = false;
    }

    private void ProcessHit()
    {
        invincible = true;
        stunned = true;
        myAnimator.SetBool(KNOCKBACK_BOOL, true);
        Vector2 playerVelocity = new Vector2(knockBack*transform.localScale.x*(-1), knockBack);
        myRigidBody.velocity = playerVelocity;
        myAnimator.SetBool(PROCESS_HIT_BOOL, true);
        StartCoroutine(SetStunnedToFalse());
        StartCoroutine(SetInvincibleToFalse());
        gameSession.UpdatePlayerHealth();
    }

    IEnumerator SetStunnedToFalse()
    {
        yield return new WaitForSeconds(knockBackTime);
        myAnimator.SetBool(KNOCKBACK_BOOL, false);
        stunned = false;
    }

    IEnumerator SetInvincibleToFalse()
    {
        yield return new WaitForSeconds(damageFrames);
        myAnimator.SetBool(PROCESS_HIT_BOOL, false);
        invincible = false;
    }

    public void SetChekpoint(Checkpoint currentChekpoint)
    {
        activeCheckpoint = currentChekpoint;
    }

    public void DeactivateChekpoint()
    {
        if(activeCheckpoint == null)
        {
            return;
        }
        else
        {
            activeCheckpoint.DeactivateThisChekpoint();
        }
    }

    public void GetMoreHealth(float amount)
    {
        health += amount;
        gameSession.UpdatePlayerHealth();
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    /*private void FreeFall()
    {
        Vector2 playerVelocity = new Vector2(myRigidBody.velocity.x, -20);
        myRigidBody.velocity = playerVelocity;
    }*/ // this function should replace gravity for the player if I ever decide to implement it

    private void SetGravityScale(float newGravity)
    {
        if (!inAJump || !isDiving || !isTouchingLadder || isTouchingGround)
        {
            myRigidBody.gravityScale = newGravity;
        }
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetBaseDamage()
    {
        return baseDamage;
    }

}
