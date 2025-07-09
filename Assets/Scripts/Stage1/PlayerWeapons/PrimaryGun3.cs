using UnityEngine;

public class PrimaryGun3 : WeaponBase
{
    public Transform firePoint;
    [SerializeField] public float fireForce = 10f;
    [SerializeField] public int bulletDamage = 10;
    [SerializeField] public float knockBackForce = 10f;
    [SerializeField] public float fireRate = 0.2f;

    private float cooldown = 0f;
    private bool isFiringHeld = false;


    void Update()
    {
        // For shooting automatically (on button hold)
        if (isFiringHeld && cooldown <= 0f)
        {
            Fire();
            cooldown = fireRate;
        }
        cooldown -= Time.deltaTime;
    }

    public override void Fire()
    {
        // Calc attack direction, center bullet angle
        Vector2 shootDir = firePoint.up;
        float baseAngle = firePoint.eulerAngles.z;

        int numberOfShots = 3;
        float spreadAngle = 15f;

        for (int i = 0; i < numberOfShots; i++)
        {
            // Calculate the spread offset (-15, 0, 15 for 3 shots)
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
                bulletScript.damage = bulletDamage;
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
