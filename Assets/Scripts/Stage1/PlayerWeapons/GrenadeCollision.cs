using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCollision : MonoBehaviour
{
    public float explosionDelay = 1f;
    public float explosionRadius = 2f;
    public int damage = 50;
    public float knockbackForce = 10f;
    public GameObject explosionEffect;
    
    private bool hasExploded = false;

    void Start()
    {
        // Set auto explosion time
        Invoke(nameof(Explode), explosionDelay);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Collision triggered
        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        if (explosionEffect != null)
        {
            // Create explosion object
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            // Pass on data to explosion handler (including this game object) if applicable
            ExplosionEventHandler handler = explosion.GetComponent<ExplosionEventHandler>();
            if (handler != null)
            {
                handler.damage = damage;
                handler.knockbackForce = knockbackForce;
            }
        }
        // Destroy this grenade game object
        Destroy(gameObject);
    }

}