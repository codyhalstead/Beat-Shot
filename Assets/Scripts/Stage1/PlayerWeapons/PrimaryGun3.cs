using UnityEngine;

public class PrimaryGun3 : WeaponBase
{
    public Transform firePoint;
    public float fireForce = 10f;
    public float bulletDamage = 10f;
    public float knockBackForce = 10f;
    public float fireRate = 0.2f;
    [SerializeField] private AudioSource fireAudioSource;
    public AudioClip firingSound;
    public int additionalBulletsPerLevel = 2;
    public float damagePercentageIncreasePerLevel = 0.1f;
    public float penaltyMultiplier = 0.25f;

    //private float cooldown = 0f;
    private bool isFiringHeld = false;

    public override void Fire(int powerLevel, bool isPenalized)
    {
        if (firingSound != null && fireAudioSource != null)
        {
            fireAudioSource.PlayOneShot(firingSound);
        }
        // Calc attack direction, center bullet angle
        Vector2 shootDir = firePoint.up;
        float baseAngle = firePoint.eulerAngles.z;

        int numberOfShots = 3;
        float damageMultiplier = 1f;
        for (int i = 0; i < powerLevel; i++)
        {
            numberOfShots += additionalBulletsPerLevel;
            damageMultiplier += damagePercentageIncreasePerLevel;
        }
        if (isPenalized)
        {
            damageMultiplier *= penaltyMultiplier;
        }
        float spreadAngle = 10f;

        for (int i = 0; i < numberOfShots; i++)
        {
            // Calculate the spread offset (-10, 0, 10 for 3 shots)
            float offset = (i - (numberOfShots - 1) / 2f) * spreadAngle;
            float angle = baseAngle + offset;

            // Rotate bullet sprite to match firing angle
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector2 adjustedDirection = Quaternion.Euler(0, 0, offset) * shootDir;

            // Create bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

            // Apply force (if still using Rigidbody2D)
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(adjustedDirection * fireForce, ForceMode2D.Impulse);
            }

            // Apply the collision script
            BulletCollision bulletScript = bullet.GetComponent<BulletCollision>();
            if (bulletScript != null)
            {
                // Pass on weapon information to script
                bulletScript.damage = Mathf.RoundToInt(bulletDamage * damageMultiplier);
                bulletScript.knockbackForce = knockBackForce;
            }
        }
    }

    public override void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    public override void HoldFire(bool isHeld)
    {
        isFiringHeld = isHeld;
    }

}
