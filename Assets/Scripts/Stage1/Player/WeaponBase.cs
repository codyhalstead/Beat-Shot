using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public GameObject bulletPrefab;

    public string weaponName;
    public string spanishWeaponName;
    // -1 == unlimited
    public int currentAmmo = -1;
    public int weaponCost = 1000;

    // Required method for all weapons
    public abstract void Fire();

    public virtual void SetFirePoint(Transform point)
    {
       // Do nothing, can be overwritten when needed
    }

    public virtual void SetFireAim(Transform point)
    {
        // Do nothing, can be overwritten when needed
    }

    public virtual void HoldFire(bool isHeld) 
    {
        // Do nothing, can be overwritten when needed
    }
    public virtual void UpdateWeapon() {
        // Do nothing, can be overwritten when needed
    }

    public virtual int GetCurrentAmmo()
    {
        // Returns current ammo count
        return currentAmmo;
    }

    public virtual void FillAmmo()
    {
        // Do nothing, can be overwritten when needed
    }
}
