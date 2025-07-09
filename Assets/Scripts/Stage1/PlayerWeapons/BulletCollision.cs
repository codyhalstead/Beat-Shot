using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // Keeps track of enemies hit (To prevent multi-triggers)
    private HashSet<GameObject> hitEnemies = new();

    public int damage = 10;
    public float knockbackForce = 5f;
    public float bulletLifetime = 7f;
    public bool destroyOnImpact = true;
    public bool noMultiHits = true;

   

    void Start()
    {
        // Destroy after lifetime
        Destroy(gameObject, bulletLifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Collision triggered
        if (noMultiHits && hitEnemies.Contains(other.gameObject))
        {
            // Collision already triggered once on target
            // and multi-hits disabled, ignore
            return;
        }
        // Handle collision hit
        HandleHit(other.gameObject, transform.position);

    }

    protected void HandleHit(GameObject target, Vector2 hitPoint)
    {
        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            // Collision was an enemy, do damage
            enemy.TakeDamage(damage);
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Enemy has RB, apply knockback
                Vector2 direction = (Vector2)enemy.transform.position - hitPoint;
                direction.Normalize();
                enemyRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
            // Add enemy to collision HashSet
            hitEnemies.Add(target);
        }
        if (destroyOnImpact) {
            // Bullet was set to be destroyed on impact
            Destroy(gameObject);
        }
    }
}
