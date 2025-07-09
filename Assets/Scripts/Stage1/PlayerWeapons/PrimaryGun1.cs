using UnityEngine;

public class PrimaryGun1 : WeaponBase
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
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Apply force to bullet
            rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
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

    public override void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    public override void HoldFire(bool isHeld)
    {
        isFiringHeld = isHeld;
    }

}
