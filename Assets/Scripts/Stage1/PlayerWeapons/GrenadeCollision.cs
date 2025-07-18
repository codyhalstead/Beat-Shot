using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GrenadeCollision : MonoBehaviour
{
    public AudioClip explosionSound;
    public AudioClip thrusterSound;
    [SerializeField] private AudioSource audioSource;

    public float explosionDelay = 1f;
    public float explosionRadius = 2f;
    public int damage = 50;
    public float knockbackForce = 10f;
    public float scaleMultiplier = 0f;
    public GameObject explosionEffect;
    
    private bool hasExploded = false;

    void Start()
    {
        if (audioSource != null && thrusterSound != null)
        {
            audioSource.clip = thrusterSound;
            audioSource.loop = true;
            audioSource.Play();
        }
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
            Vector3 spawnPosition = transform.position + new Vector3(0f, 0.5f, 0f);
            GameObject explosion = Instantiate(explosionEffect, spawnPosition, Quaternion.identity);
            if (scaleMultiplier > 0)
            {
                explosion.transform.localScale *= scaleMultiplier;
            }
            if (explosionSound != null && audioSource != null)
            {
                GameObject tempAudio = new GameObject("ExplosionSound");
                tempAudio.transform.position = transform.position;
                AudioSource source = tempAudio.AddComponent<AudioSource>();
                source.clip = explosionSound;
                source.volume = 0.8f; 
                source.spatialBlend = 1f;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.minDistance = 5f;
                source.maxDistance = 30f;
                source.Play();
                Destroy(tempAudio, explosionSound.length);
            }
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