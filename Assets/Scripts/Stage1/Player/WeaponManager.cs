using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    
    public Transform weaponHolder;
    private PlayerInput playerInput;
    private Transform playerAimer;
    private Transform player;

    // Primary
    public GameObject defaultPrimaryPrefab;
    private GameObject currentPrimaryObject;
    private WeaponBase currentPrimaryScript;

    // Secondary
    public GameObject defaultSecondaryPrefab;
    private GameObject currentSecondaryObject;
    private WeaponBase currentSecondaryScript;
    [SerializeField] private PlayerSecAmmoUI ammoUI;

    // Melee
    public GameObject defaultMeleePrefab;
    private GameObject currentMeleeObject;
    private WeaponBase currentMeleeScript;

    void Awake()
    {
        // Reference orbiting Aimer and DataManager 
        playerInput = GetComponentInParent<PlayerInput>();
    }

    void Start()
    {
        // Reference Player and orbiting Aimer 
        playerAimer = GameObject.Find("Player/PlayerAimer")?.transform;
        player = GameObject.Find("Player")?.transform;
        // Equip selected primary (or default)
        EquipDefaultPrimaryWeapon();
        
        // Equip selected secondary (or default)
        EquipDefaultSecondaryWeapon();
        
        // Equip selected melee (or default)
    
        EquipDefaultMeleeWeapon();
        
        // Set ammo UI to display secondary ammo count
        if (ammoUI != null)
        {
            ammoUI.UpdateGrenadeCount(currentSecondaryScript.GetCurrentAmmo());
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            // Do not spawn attacks if time is froze (ex: paused)
            return;
        }

        // Check if fire button is held. Pass information to primary weapon for firing
        float attackHeld = playerInput.actions["Attack"].ReadValue<float>();
        currentPrimaryScript?.HoldFire(attackHeld > 0f);
        // Secondary attack button pressed, fire secondary
        if (playerInput.actions["SecAttack"].triggered && currentSecondaryScript != null)
        {
            if (currentSecondaryScript.GetCurrentAmmo() > 0)
            {
                currentSecondaryScript.Fire();
                if (ammoUI != null)
                {
                    // Update secondary ammo UI
                    ammoUI.UpdateGrenadeCount(currentSecondaryScript.GetCurrentAmmo());
                }
            } 
            else
            {
                if (ammoUI != null)
                {
                    // Update secondary ammo UI
                    ammoUI.FlashRedTwice();
                }
            }
            
        }
        // Secondary attack button pressed, fire melee
        if (playerInput.actions["MeleeAttack"].triggered && currentMeleeScript != null)
        {
            currentMeleeScript.Fire();
        }
    }

    void EquipPrimaryWeapon(string weaponName)
    {
        // Instantiate and equip selected primary
        if (currentPrimaryObject != null)
        {
            Destroy(currentPrimaryObject);
        }
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Primaries/{weaponName}");
        if (prefab != null)
        {
            currentPrimaryObject = Instantiate(prefab, weaponHolder);
            currentPrimaryScript = currentPrimaryObject.GetComponent<WeaponBase>();
            AssignFirePoint(currentPrimaryScript, playerAimer);
        }
        else
        {
            Debug.LogWarning($"Weapon prefab '{name}' not found in Prefabs/Primaries/");
            EquipDefaultPrimaryWeapon();
        }
    }

    void EquipDefaultPrimaryWeapon()
    {
        // Instantiate and equip default primary
        currentPrimaryObject = Instantiate(defaultPrimaryPrefab, weaponHolder);
        currentPrimaryScript = currentPrimaryObject.GetComponent<WeaponBase>();
        AssignFirePoint(currentPrimaryScript, playerAimer);
    }

    void EquipSecondaryWeapon(string weaponName)
    {
        // Instantiate and equip selected secondary
        if (currentSecondaryObject != null)
        {
            Destroy(currentSecondaryObject);
        }
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Secondaries/{weaponName}");
        if (prefab != null)
        {
            currentSecondaryObject = Instantiate(prefab, weaponHolder);
            currentSecondaryScript = currentSecondaryObject.GetComponent<WeaponBase>();
            AssignFirePoint(currentSecondaryScript, playerAimer);
        }
        else
        {
            Debug.LogWarning($"Weapon prefab '{name}' not found in Prefabs/Secondaries/");
            EquipDefaultSecondaryWeapon();
        }
    }

    void EquipDefaultSecondaryWeapon()
    {
        // Instantiate and equip default secondary
        currentSecondaryObject = Instantiate(defaultSecondaryPrefab, weaponHolder);
        currentSecondaryScript = currentSecondaryObject.GetComponent<WeaponBase>();
        AssignFirePoint(currentSecondaryScript, playerAimer);
    }

    void EquipMeleeWeapon(string weaponName)
    {
        // Instantiate and equip selected melee
        if (currentMeleeObject != null)
        {
            Destroy(currentMeleeObject);
        }
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Melee/{weaponName}");
        if (prefab != null)
        {
            currentMeleeObject = Instantiate(prefab, weaponHolder);
            currentMeleeScript = currentMeleeObject.GetComponent<WeaponBase>();
            AssignFirePoint(currentMeleeScript, player);
            AssignFireAim(currentMeleeScript, playerAimer);
        }
        else
        {
            Debug.LogWarning($"Weapon prefab '{name}' not found in Prefabs/Melee/");
            EquipDefaultMeleeWeapon();
        }
    }

    void EquipDefaultMeleeWeapon()
    {
        // Instantiate and equip default melee
        currentMeleeObject = Instantiate(defaultMeleePrefab, weaponHolder);
        currentMeleeScript = currentMeleeObject.GetComponent<WeaponBase>();
        AssignFirePoint(currentMeleeScript, player);
        AssignFireAim(currentMeleeScript, playerAimer);
    }

    private void AssignFirePoint(WeaponBase weaponScript, Transform transform)
    {
        // Assign firepoint to a weapon
        if (weaponScript == null)
        {
            return;
        }
        if (playerAimer != null)
        {
            weaponScript.SetFirePoint(transform); 
        }
        else
        {
            Debug.LogWarning("Firepoint not found");
        }
    }

    private void AssignFireAim(WeaponBase weaponScript, Transform transform)
    {
        // Assign fireaim to a weapon
        if (weaponScript == null)
        {
            return;
        }
        if (playerAimer != null)
        {
            weaponScript.SetFireAim(transform);
        }
        else
        {
            Debug.LogWarning("FireAimer not found");
        }
    }
}