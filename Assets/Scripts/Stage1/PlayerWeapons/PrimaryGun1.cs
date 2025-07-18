using UnityEngine;

public class PrimaryGun1 : WeaponBase
{
    public Transform firePoint;
    public float fireForce = 10f;
    public int bulletDamage = 10;
    public float knockBackForce = 10f;
    public float fireRate = 0.2f;
    public float sizeMultiplierbyLevel = 1f;
    public float damageMultiplierbyLevel = 0.5f;
    public float penaltyMultiplier = 0.25f;
    [SerializeField] private AudioSource fireAudioSource;
    public AudioClip firingSound;

    //private float cooldown = 0f;
    private bool isFiringHeld = false;

    public override void Fire(int powerLevel, bool isPenalized)
    {
        if (firingSound != null && fireAudioSource != null)
        {
            fireAudioSource.PlayOneShot(firingSound);
        }
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        float scaleMultiplier = 1f + sizeMultiplierbyLevel * powerLevel;
        float damageMultiplier = 1f + damageMultiplierbyLevel * powerLevel;
        if (isPenalized)
        {
            damageMultiplier *= penaltyMultiplier;
        }
        bullet.transform.localScale *= scaleMultiplier;
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
            bulletScript.damage = Mathf.RoundToInt(bulletDamage * damageMultiplier);
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
