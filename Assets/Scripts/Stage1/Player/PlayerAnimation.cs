using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    // Names of player animations
    public string[] staticDirections = {"StaticN", "StaticN", "StaticW", "StaticS", "StaticS", "StaticS", "StaticE", "StaticN"};
    public string[] walkDirections = {"WalkN", "WalkN", "WalkW", "WalkS", "WalkS", "WalkS", "WalkE", "WalkN"};
    public string[] boostDirections = { "BoostN", "BoostN", "BoostW", "BoostS", "BoostS", "BoostS", "BoostE", "BoostN" };
    int lastDirection;

    private void Awake()
    {
        // Automatically reference the player Animator component
        anim = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction, bool isBoosting)
    {
        string[] directionArray = null;
        if (direction.magnitude < 0.1)
        {
            // Player not moving, use static animations
            directionArray = staticDirections;
        }
        else
        {
            if (isBoosting)
            {
                // Player is moving and boosting, use boost animations
                directionArray = boostDirections;
            }
            else
            {
                // Player is moving normally, use walk animations
                directionArray = walkDirections;
                
            }
            // Calculate directional index from direction
            // Used to reference appropriate directional animations from array.
            lastDirection = DirectionToIndex(direction);
        }
        // Play determined animation based on movement
        anim.Play(directionArray[lastDirection], 0);
    }

    // Calculates directional index (8-way)
    private int DirectionToIndex(Vector2 direction)
    {
        Vector2 norDir = direction.normalized;
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

    public void Die()
    {
        anim.Play("Death", 0);
    }
}
