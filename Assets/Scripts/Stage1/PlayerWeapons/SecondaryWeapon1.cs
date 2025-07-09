using UnityEngine;

public class SecondaryWeapon1 : WeaponBase
{
    //public GameObject grenadePrefab;
    public Transform firePoint;
    [SerializeField] public float fireForce = 10f;
    [SerializeField] public int grenadeDamage = 50;
    [SerializeField] public float knockBackForce = 10f;
    [SerializeField] public float explosionDelay = 3f;
    [SerializeField] public float explosionRadius = 2f;
    [SerializeField] public int maxAmmo = 25;

    private void Awake()
    {
        // Set ammo to max
        currentAmmo = maxAmmo;
    }

    public override void Fire()
    {
        if (currentAmmo <= 0)
        {
            // Out of ammo, do nothing
            return;
        }
        // Create grenade
        GameObject grenade = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Apply force to grenade
            rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
            rb.angularVelocity = 100f;
        }
        // Apply the collision script
        GrenadeCollision grenadeScript = grenade.GetComponent<GrenadeCollision>();
        if (grenadeScript != null)
        {
            // Pass on weapon information to script
            grenadeScript.damage = grenadeDamage;
            grenadeScript.knockbackForce = knockBackForce;
            grenadeScript.explosionDelay = explosionDelay;
            grenadeScript.explosionRadius = explosionRadius;
        }
        // Reduce current ammo
        currentAmmo--;
    }

    public override void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

}