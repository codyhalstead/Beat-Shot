using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Melee1 : WeaponBase
{
    public Transform firePoint;
    public Transform playerAimer;
    private bool isSwinging = false;
    private GameObject hitbox;
    [SerializeField] public float fireForce = 10f;
    [SerializeField] public int bulletDamage = 10;
    [SerializeField] public float knockBackForce = 10f;

    public override void Fire()
    {
        if (isSwinging)
        {
            // Previous swing in progress, ignore
            return;
        }
        // Create the "Swing"
        hitbox = Instantiate(bulletPrefab, firePoint);
        // Use firepoint center
        hitbox.transform.localPosition = Vector3.zero;
        // Allign with player aiming direction
        hitbox.transform.localRotation = playerAimer.rotation;
        // Apply the collision script
        BulletCollision bulletScript = hitbox.GetComponent<BulletCollision>();
        if (bulletScript != null)
        {
            // Pass on weapon information to script
            bulletScript.destroyOnImpact = false;
            bulletScript.damage = bulletDamage;
            bulletScript.knockbackForce = knockBackForce;
            // Limit swing lifetime
            StartCoroutine(SwingDuration(0.3f));
            // Set as "swinging"
            isSwinging = true;
        }
    }

    public override void SetFirePoint(Transform point)
    {
        firePoint = point;
    }

    public override void SetFireAim(Transform point)
    {
        playerAimer = point;
    }

    private IEnumerator SwingDuration(float time)
    {
        // Completes swing after given duration
        yield return new WaitForSeconds(time);
        CompleteSwing();
    }

    public void CompleteSwing()
    {
        // Destroy "swing" object, set as not "swinging"
        Destroy(hitbox);
        isSwinging = false;
    }
}
