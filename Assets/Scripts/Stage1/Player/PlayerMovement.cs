using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float boostMult;
    [SerializeField] private float boostCost;
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Light2D playerGlow;
    private PlayerAnimation playerAnimation;

    private Rigidbody2D rigidBody;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    private Vector2 externalForce;
    private float externalForceDecay = 10f;

    private bool isBoosting = false;
    private bool isDead = false;

    private void Awake()
    {
        // Automatically reference a player objects RigidBody2D
        // and PlayerAnimation components
        rigidBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        // Enable interpolation (smoother movement)
        rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
        // Start fade-in
        StartCoroutine(LoadWithFadeIn());
    }

    private void Update()
    {
        if (isDead)
        {
            // Player is dead, do nothing
            return;
        }
        //Check if boost button was pressed
        bool toggleBoostPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        if (toggleBoostPressed)
        {
            // Boost was pressed
            // player is not currently boosting, player is moving, and player has energy
            if (!isBoosting && movementInput != Vector2.zero && playerEnergy.currentEnergy > 0)
            {
                EnableBoost();
            }
            // Otherwise, toggle boost off
            else
            {
                DisableBoost();
            }
        }
        // Player was boosting but stopped mooving, toggle boost off
        if (isBoosting && movementInput == Vector2.zero)
        {
            DisableBoost();
        }
        // Player is currently boosting, drain energy
        if (isBoosting)
        {
            bool hasEnoughEnergy = playerEnergy.UseEnergy(boostCost * Time.deltaTime);
            if (!hasEnoughEnergy)
            {
                // Player ran out of energy, toggle boost off
                DisableBoost();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            // Player is dead, do nothing
            return;
        }
        // Interpolate smoothedMovementInput towards current movementInput
        // to avoid hard, rigid stops
        smoothedMovementInput = Vector2.SmoothDamp(
            smoothedMovementInput,
            movementInput,
            ref movementInputSmoothVelocity,
            0.1f
            );
        // Apply smoothed movement to players RB in addition to any applied external forces
        rigidBody.linearVelocity = (smoothedMovementInput * speed) + externalForce;
        // Gradually reduce applied external forces (again, for smooth movements)
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.fixedDeltaTime * externalForceDecay);
        // Communicate movement direction and boost status to animation controller
        if (playerAnimation != null)
        {
            playerAnimation.SetDirection(smoothedMovementInput, isBoosting);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        //Track player input for directional control
        movementInput = inputValue.Get<Vector2>();
    }

    private void EnableBoost()
    {
        if (isBoosting)
        {
            // Already boosting, do nothing
            return;
        }
        // Toggle boost status, increase speed
        isBoosting = true;
        speed *= boostMult;

    }

    private void DisableBoost()
    {
        if (!isBoosting)
        {
            // Wasn't boosting, do nothing
            return;
        }
        // Toggle boost status, decrease speed
        isBoosting = false;
        speed /= boostMult;
    }

    public void SetGlowPurple()
    {
        // Sets player glow settings for death explosion
        playerGlow.color = new Color(0.6f, 0f, 1f); 
        playerGlow.pointLightOuterRadius = 5f;
        playerGlow.intensity = 3f;
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        // Disable player input, stop all current movement, play death anim
        GetComponent<PlayerInput>().DeactivateInput();
        playerAnimation.Die();
        movementInput = Vector2.zero;
        rigidBody.linearVelocity = Vector2.zero;
        StartCoroutine(RestartWithFade());
    }

    private IEnumerator RestartWithFade()
    {
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader != null)
        {
            fader.missionComplete = false;
            yield return StartCoroutine(fader.FadeOut());
        }
        // Reload scene after fade
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadWithFadeIn()
    {
        // Fade-in
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader != null)
        {
            yield return StartCoroutine(fader.FadeIn());
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ApplyKnockback(Vector2 force)
    {
        // Add given external force to tracker
        externalForce += force;
    }

}
