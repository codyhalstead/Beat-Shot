using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static BaseEnemy;

public class Boss : BaseEnemy
{
    private bool isVulnerable;

    public Transform primaryFirePoint;
    public GameObject primaryProjectilePrefab;
    public Light2D bossGlow;

    [Header("CircularShot")]
    public Transform circularFirePoint;
    public GameObject circularShotProjectilePrefab;
    [SerializeField] private int circularNumberOfShots = 8;
    [SerializeField] private float circularPatternRotationSpeed = 10f;
    private float circularPatternRotationOffset = 0f;
    [SerializeField] private float circularShootCooldown = 1f;
    [SerializeField] private float circularShotDamage = 1f;
    [SerializeField] private float circularShotKnockback = 1f;
    [SerializeField] private float circularShotSpeed = 1f;
    [SerializeField] private float circularBulletLifeTime = 1f;
    private float circularShotLastShootTime = 0f;

    [Header("RocketShot")]
    public GameObject rocketShotProjectilePrefab;
    public Transform rocketFirePoint;
    [SerializeField] private float rocketShootCooldown = 1f;
    private float rocketLastShootTime = 0f;
    [SerializeField] private float rocketSpeed = 5f;
    [SerializeField] private float rocketRotateSpeed = 200f;
    [SerializeField] private float rocketLifetime = 5f;
    [SerializeField] private int rocketDamage;
    [SerializeField] private float rocketKnockback;
    [SerializeField] private float rocketExplosionDelay = 5f;

    [Header("BossUI")]
    [SerializeField] private GameObject bossHealthUI;
    [SerializeField] private PlayerHealthUI healthBar;

    [Header("BossMovementPoints")]
    [SerializeField] private Vector2 anchorPosition = new Vector2(25f, 24f);
    [SerializeField] private float anchorDistanceLimit;

    [Header("FinalState")]
    [SerializeField] private Color flashColor = new Color(1f, 0.2f, 0.2f); 
    [SerializeField] private float flashSpeed = 2f;

    private bool isAlerting = false;
    private bool isWalking = false;
    private bool inFinalState = false;

    private LevelEndTrigger levelEndTrigger;

    protected override void Start()
    {
        base.Start();
        // Begin in Patrolling state if patrol points set. Begin in Idle state otherwise
        levelEndTrigger = GetComponent<LevelEndTrigger>();
        currentState = EnemyState.Idle;
        isVulnerable = false;
        if (bossHealthUI != null)
        {
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
                healthBar.SetHealth(maxHealth);
            }
        }
    }

    void Update()
    {
        if (inFinalState && !isDead)
        {
            float t = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f; 
            bossGlow.color = Color.Lerp(Color.clear, flashColor, t);
        }
        else if (isDead)
        {
            bossGlow.color = Color.clear;
        }
    }

    void FixedUpdate()
    {

        if (target == null || isDead)
        {
            // Target is dead, do nothing
            return;
        }
        float anchorDistance = Vector2.Distance(transform.position, anchorPosition);
        float playerDistance = Vector2.Distance(transform.position, target.position);
        // Enemy not aware
        if (!hasBeenAlerted && !isAlerting)
        {
            // Player within detection range or attacked, switch to Alerted state
            if (playerDistance <= detectionRange || tookDamage)
            {
                ChangeState(EnemyState.Alerted);
            }
        }
        else if (!isAlerting)
        {
            if (anchorDistance >= anchorDistanceLimit || isWalking)
            {
                ChangeState(EnemyState.Pursuing);
            }
            else
            {
                ChangeState(EnemyState.Shooting);
            }
        }
        if (currentHealth <= (maxHealth * 60) / 100)
        {
            ShootRadial();
            if (!inFinalState && currentHealth <= (maxHealth * 20) / 100)
            {
                circularShootCooldown = (circularShootCooldown * 0.5f);
                shootCooldown = (shootCooldown * 0.75f);
                rocketSpeed = (rocketSpeed * 1.5f);
                inFinalState = true;
            }
        }
        // Execute current behavior
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.Play("Idle");
                break;

            case EnemyState.Patrolling:
                //Patrol();
                break;

            case EnemyState.Alerted:
                FaceTarget();
                StartCoroutine(Alert());
                break;

            case EnemyState.Pursuing:
                FaceTarget();
                PursueTarget();
                break;

            case EnemyState.Shooting:
                FaceTarget();
                ShootAtTarget();
                ShootHomingRocket();
                break;
        }
    }

    void PursueTarget()
    {
        Vector2 dir = (anchorPosition - (Vector2)transform.position);

        if (dir.magnitude < 0.1f)
        {
            isWalking = false;
            ChangeState(EnemyState.Idle);
            rigidBody.linearVelocity = Vector2.zero; 
            return;
        }

        if (!isWalking)
        {
            anim.Play("Walk");
            isWalking = true;
        }

        dir.Normalize();
        Vector2 movePos = rigidBody.position + dir * moveSpeed * Time.fixedDeltaTime;
        rigidBody.MovePosition(movePos);
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            // State unchanged, do nothing
            return;
        }
        currentState = newState;
    }



    IEnumerator Alert()
    {
        // Pause for alert duration, then switch to Pursuing state
        isAlerting = true;
        hasBeenAlerted = true;
        anim.Play("Enable");
        yield return new WaitForSeconds(alertDuration);
        ChangeState(EnemyState.Pursuing);
        isAlerting = false;
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(true);
        }
        isVulnerable = true;
    }

    public override void TakeDamage(int amount)
    {
       if (isVulnerable)
        {
            base.TakeDamage(amount);
            healthBar.SetHealth(currentHealth);
        }
    }

    public override void Die()
    {
        //Stop all current movement, play death anim
        isDead = true;
        isVulnerable = false;
        if (rigidBody != null)
        {
            rigidBody.linearVelocity = Vector2.zero;
            rigidBody.simulated = false;


        }
        anim.Play("Death");
        giveCurrencyToPlayerTarget();
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
        if (levelEndTrigger != null)
        {
            StartCoroutine(DelayedLevelEnd());
        }
    }

    private IEnumerator DelayedLevelEnd()
    {
        yield return new WaitForSeconds(3f);
        levelEndTrigger.ManualEndLevel();
    }

    void FaceTarget()
    {
        if (target == null)
        {
            return;
        }
        Vector3 scale = transform.localScale;
        // Flip sprite X based on target position
        if (target.position.x < transform.position.x)
        {
            // Face left
            scale.x = Mathf.Abs(scale.x) * -1;
        }
        else
        {
            // Face right
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
    }

    void ShootHomingRocket()
    {
        // Check attack cooldown
        if (Time.time - rocketLastShootTime >= rocketShootCooldown)
        {
            anim.Play("ArmShot");
            // Attack ready
            rocketLastShootTime = Time.time;
            if (rocketShotProjectilePrefab != null && rocketFirePoint != null)
            {
                // Calc attack direction
                Vector2 shootDir = (target.position - rocketFirePoint.position).normalized;
                // Rotate bullet sprite to match firing angle
                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg + 0f;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                // Create bullet
                GameObject bullet = Instantiate(rocketShotProjectilePrefab, rocketFirePoint.position, rotation);
                // Apply the collision script
                HomingProj bulletScript = bullet.GetComponent<HomingProj>();
                if (bulletScript != null)
                {
                    // Pass on weapon information to script
                    bulletScript.lifetime = rocketLifetime;
                    bulletScript.speed = rocketSpeed;
                    bulletScript.damage = rocketDamage;
                    bulletScript.knockback = rocketKnockback;
                    bulletScript.explosionDelay = rocketExplosionDelay;
                    bulletScript.rotateSpeed = rocketRotateSpeed;
                }
            }
        }
    }

    void ShootAtTarget()
    {
        // Check attack cooldown
        if (Time.time - lastShootTime >= shootCooldown)
        {
            anim.Play("ChestShot");
            // Attack ready
            lastShootTime = Time.time;
            if (primaryProjectilePrefab != null && primaryFirePoint != null)
            {
                // Calc attack direction
                Vector2 shootDir = (target.position - primaryFirePoint.position).normalized;
                // Rotate bullet sprite to match firing angle
                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg + 270f;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                // Create bullet
                GameObject bullet = Instantiate(primaryProjectilePrefab, primaryFirePoint.position, rotation);
                // Apply the collision script
                BasicProj bulletScript = bullet.GetComponent<BasicProj>();
                if (bulletScript != null)
                {
                    // Pass on weapon information to script
                    bulletScript.Initialize(shootDir, bulletLifeTime);
                    bulletScript.knockback = shotKnockback;
                    bulletScript.damage = shotDamage;
                    bulletScript.speed = shotSpeed;
                }
            }
        }
    }

    void ShootRadial()
    {
        if (Time.time - circularShotLastShootTime >= circularShootCooldown)
        {
            circularShotLastShootTime = Time.time;

            if (circularShotProjectilePrefab != null && circularFirePoint != null)
            {
                // Update the rotation offset for the pattern
                circularPatternRotationOffset += circularPatternRotationSpeed;

                for (int i = 0; i < circularNumberOfShots; i++)
                {
                    // Spread bullets evenly around a full circle
                    float angle = (360f / circularNumberOfShots) * i + circularPatternRotationOffset;
                    Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                    Vector2 direction = rotation * Vector2.right; 

                    GameObject bullet = Instantiate(circularShotProjectilePrefab, circularFirePoint.position, rotation);

                    BasicProj bulletScript = bullet.GetComponent<BasicProj>();
                    if (bulletScript != null)
                    {
                        bulletScript.Initialize(direction, circularBulletLifeTime);
                        bulletScript.knockback = circularShotKnockback;
                        bulletScript.damage = circularShotDamage;
                        bulletScript.speed = circularShotSpeed;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Set gizmos for detection/shooting/fleeing range visualizations
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
