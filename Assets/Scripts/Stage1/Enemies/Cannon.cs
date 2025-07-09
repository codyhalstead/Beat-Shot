using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Cannon : BaseEnemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public bool facingLeft;
    public Light2D cannonGlow;

    protected override void Start()
    {
        base.Start();
        currentState = EnemyState.Idle;
        if (!facingLeft)
        {
            // Flip cannon sprite to face right
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }
    }

    void Update()
    {
        // Target is dead, do nothing
        if (target == null || isDead)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, target.position);
        // Enter Shooting state if player is in shooting range, enter idle state otherwise 
        if (distance <= detectionRange)
        {
            ChangeState(EnemyState.Shooting);
        } 
        else
        {
            ChangeState(EnemyState.Idle);
        }
        // Execute current behavior
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.Play("Idle");
                break;

            case EnemyState.Shooting:
                Shoot();
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

    void Shoot()
    {
        // Check attack cooldown
        if (Time.time - lastShootTime >= shootCooldown)
        {
            // Attack ready
            anim.Play("Idle");
            lastShootTime = Time.time;
            if (projectilePrefab != null)
            {
                // Calc attack direction (straight left or right)
                Vector2 shootDir = Vector2.left;
                if (!facingLeft)
                {
                    shootDir = Vector2.right;
                }
                // Rotate bullet sprite to match firing angle
                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg + 270f;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
                // Create bullet
                GameObject bullet = Instantiate(projectilePrefab, firePoint.position, rotation);
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

    public override void Die()
    {
        // Stop all current movement, play death anim
        isDead = true;
        if (rigidBody != null)
        {
            rigidBody.linearVelocity = Vector2.zero;
        }
        anim.Play("Death");
        giveCurrencyToPlayerTarget();
        logDeath();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void logDeath()
    {

    }

    void OnDrawGizmosSelected()
    {
        // Set gizmos for detection/shooting range visualizations
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    public void SetGlowOrange()
    {
        // Sets player glow settings for death explosion
        if (cannonGlow != null)
        {
            cannonGlow.color = new Color(1f, 0.5f, 0f);
            cannonGlow.pointLightOuterRadius = 4f;
            cannonGlow.intensity = 3f;
        }
    }
}
