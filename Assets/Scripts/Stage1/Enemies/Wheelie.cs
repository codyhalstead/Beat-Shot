using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Wheelie : BaseEnemy
{
    public Light2D wheelieGlow;
    public float sprintSpeed;
    private bool isAlerting = false;

    // Names of Wheelie animations
    public string[] idleDirections = { "IdleUp", "IdleUp", "IdleLeft", "IdleDown", "IdleDown", "IdleDown", "IdleRight", "IdleUp" };
    public string[] walkDirections = { "WalkUp", "WalkUp", "WalkLeft", "WalkDown", "WalkDown", "WalkDown", "WalkRight", "WalkUp" };
    public string[] runDirections = { "RunUp", "RunUp", "RunLeft", "RunDown", "RunDown", "RunDown", "RunRight", "RunUp" };

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
        float distance = Vector2.Distance(transform.position, target.position);
        // Enemy not aware
        if (!hasBeenAlerted && !isAlerting)
        {
            // Player within detection range or attacked, switch to Alerted state
            if (distance <= detectionRange || tookDamage)
            {
                ChangeState(EnemyState.Alerted);
            }
        }
        // Enemy is aware, remain in the Pursuing state
        else if (!isAlerting)
        {
            ChangeState(EnemyState.Pursuing);
        }
        // Execute current behavior
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.Play(idleDirections[3]);
                break;

            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Alerted:
                StartCoroutine(AlertBeforePursuing());
                break;

            case EnemyState.Pursuing:
                PursueTarget();
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
        // Pause for alert duration, then switch to Pursuing state
        isAlerting = true;
        hasBeenAlerted = true;
        anim.Play(idleDirections[3]);
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
        // Move towards target (using sprintSpeed), play proper direction animation
        Vector2 direction = (target.position - transform.position).normalized;
        anim.Play(runDirections[DirectionToIndex(direction)]);
        transform.position += (Vector3)(direction * sprintSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            // No patrol points set, do nothing
            return;
        }
        // Play proper direction animation
        Transform destination = patrolPoints[currentPatrolIndex];
        Vector2 dir = (destination.position - transform.position);
        anim.Play(walkDirections[DirectionToIndex(dir)]);
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
        if (wheelieGlow != null)
        {
            wheelieGlow.color = new Color(1f, 0.5f, 0f);
            wheelieGlow.pointLightOuterRadius = 4f;
            wheelieGlow.intensity = 3f;
        }
    }

    public void SetGlowAlert()
    {
        // Sets player glow settings for alerted flash
        if (wheelieGlow != null)
        {
            wheelieGlow.color = new Color(0.5686f, 0.9137f, 0.6902f);
            wheelieGlow.pointLightOuterRadius = 2f;
            wheelieGlow.intensity = 3f;
        }
    }

    public void SetGlowInvis()
    {
        // Clears player glow settings for alerted flash
        if (wheelieGlow != null)
        {
            wheelieGlow.color = new Color(0f, 0f, 0f, 0f);
            wheelieGlow.pointLightOuterRadius = 2f;
            wheelieGlow.intensity = 3f;
        }
    }

    // Calculates directional index (8-way)
    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDir = _direction.normalized;
        // Divide circle 8-ways, offset by half sector
        float step = 360 / 8;
        float offset = step / 2;
        // Calculate angle (from up, -180 to 180)
        float angle = Vector2.SignedAngle(Vector2.up, norDir);
        // Apply offset, ensure angle is in 0-360 range
        angle += offset;
        if (angle < 0)
        {
            angle += 360;
        }
        // Divide to get step (0-7)
        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }

    // Enemy explodes on contact with player to do damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collision triggered
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // Collision was with the player, do damage
                Vector2 direction = (target.position - transform.position);
                player.TakeDamage(shotDamage); 
                var movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    // Apply knockback force to Player
                    movement.ApplyKnockback(direction * shotKnockback);
                }
                // Self-destruct
                Die();
            }
        }
        
    }


}
