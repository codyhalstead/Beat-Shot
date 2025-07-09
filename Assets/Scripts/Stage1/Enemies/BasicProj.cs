using UnityEngine;

public class BasicProj : MonoBehaviour
{
    public float speed;
    public float damage;
    public float knockback;
    public float lifetime;

    private Vector2 direction;

    public void Initialize(Vector2 shootDirection, float lifetime)
    {
        // Set projectile direction and lifetime, set destroy timer
        direction = shootDirection.normalized;
        this.lifetime = lifetime;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Collision triggered
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            // Collision was against player, do damage
            player.TakeDamage(damage);
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                // Apply knockback force to players
                movement.ApplyKnockback(direction * knockback);
            }

        }
        // Destroy on any collision
        Destroy(gameObject);
    }


}