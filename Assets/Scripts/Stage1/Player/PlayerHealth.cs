using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    public float maxHealth = 100f;
    public float currentHealth;
    private Color originalColor;

    public PlayerHealthUI healthBar;

    void Start()
    {
        // Set health to max, update health UI
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // Do nothing
    }

    void Awake()
    {
        // Reference players PlayerMovement, store current sprite color
        playerMovement = GetComponent<PlayerMovement>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(float amount)
    {
        // Subtract given damage amount from health
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Update health UI
        healthBar.SetHealth(currentHealth);
        // Either die if hp now <= 0, or have player sprite flash red
        if (currentHealth <= 0)
        {
            Die();
        } else
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashColor(Color.red));
            }
        }

    }

    public void HealDamage(float amount)
    {
        // Add given amount to current health
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Update health UI,  have player sprite flash green
        healthBar.SetHealth(currentHealth);
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashColor(Color.green));
        }

    }

    void Die()
    {
        // Player HP reached 0, send death status
        if (playerMovement != null)
        {
            playerMovement.Die();
        }
    }

    private IEnumerator FlashColor(Color flashColor)
    {
        // Make player sprite flash given sprite color
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}