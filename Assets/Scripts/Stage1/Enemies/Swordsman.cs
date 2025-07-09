using System.Collections;
using UnityEngine;

public class Swordsman : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    private bool isAlerting = false;
    public float forwardOffset = 1f;  
    public float verticalOffset = 0.8f;

    protected override void Start()
    {
        base.Start();
        // Begin in Patrolling state if patrol points set. Begin in Idle state otherwise
        currentState = (patrolPoints != null && patrolPoints.Length > 0) ? EnemyState.Patrolling : EnemyState.Idle;
    }

    void Update()
    {
        if (target == null || isDead)
        {
            // Target is dead, do nothing
            return;
        }
        float distance = Vector2.Distance(firePoint.position, target.position);
        // Enemy not aware
        if (!hasBeenAlerted && !isAlerting)
        {
            // Player within detection range or attacked, switch to Alerted state
            if (distance <= detectionRange || tookDamage)
            {
                ChangeState(EnemyState.Alerted);
            }
        }
        // Enemy aware
        else if (!isAlerting)
        {
            // Enter Shooting state if in shooting range, enter Pursuing state otherwise
            if (distance <= shootingRange)
            {
                ChangeState(EnemyState.Shooting);
            }
            else
            {
                ChangeState(EnemyState.Pursuing);
            }
        }
        // Execute current behavior
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.Play("Idle");
                break;

            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Alerted:
                FaceTarget();
                StartCoroutine(AlertBeforePursuing());
                break;

            case EnemyState.Pursuing:
                FaceTarget();
                PursueTarget();
                break;

            case EnemyState.Shooting:
                FaceTarget();
                ShootAtTarget();
                break;
        }
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

    IEnumerator AlertBeforePursuing()
    {
        // Play animation, pause for alert duration, then switch to Pursuing state
        isAlerting = true;
        hasBeenAlerted = true;
        anim.Play("Enable");
        yield return new WaitForSeconds(alertDuration);
        ChangeState(EnemyState.Pursuing);
        isAlerting = false;
    }

    void PursueTarget()
    {
        if (target == null)
        {
            return;
        }
        // Move towards target, play walking animation
        anim.Play("Walk");
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    void ShootAtTarget()
    {
        // Check attack cooldown
        if (Time.time - lastShootTime >= shootCooldown)
        {
            // Attack ready
            anim.Play("Attack1");
            lastShootTime = Time.time;
            if (projectilePrefab != null && firePoint != null)
            {
                // Adjust targeting and firepoint positions with offsets
                // Used to accomodate sprite (both enemy and attack) misallignments
                Vector2 targetOffset = new Vector2(0f, verticalOffset); 
                Vector2 adjustedTargetPos = (Vector2)target.position + targetOffset;
                // Calc attack direction
                Vector2 shootDir = (adjustedTargetPos - (Vector2)firePoint.position).normalized;
                // Rotate bullet (attack) sprite to match firing angle
                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                // Calculate bullet (attack) spawn position
                Vector2 spawnPosition = (Vector2)firePoint.position
                                      + shootDir * forwardOffset
                                      + Vector2.down * verticalOffset;
                // Create bullet (attack)
                GameObject bullet = Instantiate(projectilePrefab, spawnPosition, rotation);
                // Apply the collision script
                SwordsmanProj bulletScript = bullet.GetComponent<SwordsmanProj>();
                if (bulletScript != null)
                {
                    // Pass on weapon information to script
                    bulletScript.Initialize(shootDir);
                    bulletScript.knockback = shotKnockback;
                    bulletScript.damage = shotDamage;
                    bulletScript.speed = shotSpeed;
                    bulletScript.lifetime = bulletLifeTime;

                }
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            // No patrol points set, do nothing
            return;
        }
        Transform destination = patrolPoints[currentPatrolIndex];
        Vector2 dir = (destination.position - transform.position);
        anim.Play("Walk");
        if (dir.magnitude < 0.2f)
        {
            // Hit patrol point, move on to next (looping)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            // Move towards next patrol point (using moveSpeed)
            transform.position += (Vector3)(dir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    public override void Die()
    {
        //Stop all current movement, play death anim
        isDead = true;
        if (rigidBody != null)
        {
            rigidBody.linearVelocity = Vector2.zero;
        }
        anim.Play("Death");
        giveCurrencyToPlayerTarget();
        logDeath();
    }

    private void logDeath()
    {
 
    }

    public void Destroy()
    {
        Destroy(gameObject);
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

    void OnDrawGizmosSelected()
    {
        // Set gizmos for detection/shooting range visualizations
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
