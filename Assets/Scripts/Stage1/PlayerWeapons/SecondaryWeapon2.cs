using UnityEngine;

public class SecondaryWeapon2 : WeaponBase
{
    //public GameObject grenadePrefab;
    public Transform firePoint;
    [SerializeField] public float fireForce = 10f;
    [SerializeField] public int grenadeDamage = 50;
    [SerializeField] public float knockBackForce = 10f;
    [SerializeField] public float explosionDelay = 3f;
    [SerializeField] public float explosionRadius = 2f;
    public float sizeMultiplierbyLevel = 1f;
    public float damageMultiplierbyLevel = 0.35f;
    public float penaltyMultiplier = 0.25f;

    private void Awake()
    {
      
    }

    public override void Fire(int powerLevel, bool isPenalized)
    {
        // Create grenade
        GameObject grenade = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 90f));
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        float scaleMultiplier = 1f + sizeMultiplierbyLevel * (powerLevel - 1);
        float damageMultiplier = 1f + damageMultiplierbyLevel * (powerLevel - 1);
        if (isPenalized)
        {
            damageMultiplier *= penaltyMultiplier;
        }
        if (rb != null)
        {
            // Apply force to grenade
            rb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
        }
        // Apply the collision script
        GrenadeCollision grenadeScript = grenade.GetComponent<GrenadeCollision>();
        if (grenadeScript != null)
        {
            // Pass on weapon information to script
            grenadeScript.damage = Mathf.RoundToInt(grenadeDamage * damageMultiplier);
            grenadeScript.knockbackForce = knockBackForce;
            grenadeScript.explosionDelay = explosionDelay;
            grenadeScript.explosionRadius = explosionRadius;
            grenadeScript.scaleMultiplier = scaleMultiplier;
        }
        // Reduce current ammo
        currentAmmo--;
    }

    public override void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

}