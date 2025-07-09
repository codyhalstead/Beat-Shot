using UnityEngine;

public class ExplosionEventHandler : MonoBehaviour
{
    public int damage = 50;
    public float knockbackForce = 10f;
    public bool isTargetPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTargetPlayer)
        {
            // Collision triggered (Player)
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                // Collision was an enemy, do damage
                enemy.TakeDamage(damage);
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Enemy has RB, apply knockback
                    Vector2 direction = (Vector2)enemy.transform.position - (Vector2)transform.position;
                    direction.Normalize();
                    enemyRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
              
                player.TakeDamage(damage);
                var movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    // Apply knockback force to players
                    Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
                    direction.Normalize();
                    movement.ApplyKnockback(direction * knockbackForce);
                }
            }
        }
    }

    public void OnExplosionAnimationEnd()
    {
        // Destroy the explosion object
        Destroy(gameObject);
    }
}
