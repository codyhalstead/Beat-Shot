using System;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;

    public float baseSpeed = 5f;
    public float baseRotationSpeed = 100f;

    private Transform mouseReticle;
    private float rotationSpeedMultiplier = 1f;

    public float homingPercentageIncreasePerLevel = 0.07f;

    void Start()
    {
        GameObject reticleObj = GameObject.FindGameObjectWithTag("MouseReticle");
        if (reticleObj != null)
        {
            mouseReticle = reticleObj.transform;
        }
        rb = GetComponent<Rigidbody2D>();
        target = FindClosestTarget(15f);
    }

    public void SetHomingStrength(int powerLevel)
    {
        rotationSpeedMultiplier = 2f * (powerLevel * homingPercentageIncreasePerLevel);
    }

    void FixedUpdate()
    {
        if (target == null || rb == null)
        {
            return;
        }
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float rotateAmount = Vector3.Cross(transform.up, direction).z;

        rb.angularVelocity = rotateAmount * baseRotationSpeed * rotationSpeedMultiplier;
        rb.linearVelocity = transform.up * baseSpeed;
    }

    Transform FindClosestTarget(float detectionRadius)
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy"); 

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider2D hit in hits)
        {
            float dist = 0f;
            if (mouseReticle != null)
            {
                Vector2 reticleWorldPos = MouseReticle.WorldPosition;
                dist = Vector2.Distance(reticleWorldPos, hit.transform.position);
            }
            else
            {
                dist = Vector2.Distance(transform.position, hit.transform.position);
            }
            
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }
}