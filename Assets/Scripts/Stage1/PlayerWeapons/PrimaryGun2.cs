using System.Collections;
using UnityEngine;

public class PrimaryGun2 : WeaponBase
{
    public Transform firePoint;
    [SerializeField] public float fireForce = 10f;
    [SerializeField] public int bulletDamage = 10;
    [SerializeField] public float knockBackForce = 10f;
    [SerializeField] public float fireRate = 0.2f;
    [SerializeField] public int bulletsPerPowerLevel = 1;
    public float penaltyMultiplier = 0.25f;
    [SerializeField] private AudioSource fireAudioSource;
    public AudioClip firingSound;
    public float homingPercentageIncreasePerLevel = 0.5f;
    public float damagePercentageIncreasePerLevel = 0.1f;


    //private float cooldown = 0f;
    private bool isFiringHeld = false;
    private Coroutine firingCoroutine;

    public override void Fire(int powerLevel, bool isPenalized)
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }

        firingCoroutine = StartCoroutine(FireBurst(powerLevel, isPenalized));
    }

    private IEnumerator FireBurst(int powerLevel, bool isPenalized)
    {
        int totalShots = 2;
        for (int i = 0; i < totalShots; i++)
        {
            ShootOne(powerLevel, isPenalized);
            if (i < totalShots - 1)
            {
                yield return new WaitForSeconds(fireRate);
            }
        }

        firingCoroutine = null;
    }

    private void ShootOne(int powerLevel, bool isPenalized)
    {
        if (firingSound != null && fireAudioSource != null)
        {
            fireAudioSource.pitch = Random.Range(0.90f, 1.1f);
            fireAudioSource.PlayOneShot(firingSound);
        }
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        float damageMultiplier = 1f + damagePercentageIncreasePerLevel * powerLevel;
        if (isPenalized)
        {
            damageMultiplier *= penaltyMultiplier;
        }
        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.up * fireForce;
        }

        BulletCollision bulletScript = bullet.GetComponent<BulletCollision>();
        if (bulletScript != null)
        {

            bulletScript.damage = Mathf.RoundToInt(bulletDamage * damageMultiplier);
            bulletScript.knockbackForce = knockBackForce;
        }

        HomingBullet homing = bullet.GetComponent<HomingBullet>();
        if (homing != null)
        {
            homing.homingPercentageIncreasePerLevel = homingPercentageIncreasePerLevel;
            homing.SetHomingStrength(powerLevel);
            homing.baseSpeed = fireForce;
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
