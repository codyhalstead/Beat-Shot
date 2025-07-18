using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class WeaponManager : MonoBehaviour
{
    
    public Transform weaponHolder;
    private PlayerInput playerInput;
    private Transform playerAimer;
    private Transform player;
    [SerializeField] private SyncedBPMBeat beatTracker;
    [SerializeField] private BeatPowerBarUI beatPowerBar;
    public float beatPowerMax = 240f;
    public float beatPowerPerBeatHit = 20f;

    // Primary
    private GameObject primaryPrefab1;
    private GameObject primaryPrefab2;
    private GameObject primaryPrefab3;
    private GameObject[] primaryWeapons = new GameObject[3];
    private WeaponBase currentPrimaryScript;
    int currentWeaponIndex = 0;

    // Secondary
    private GameObject secondaryPrefab;
    private WeaponBase currentSecondaryScript;

    // Melee
    private GameObject meleePrefab;
    private GameObject currentMeleeObject;
    private WeaponBase currentMeleeScript;

    private bool lastBeatSkipped;
    private float currentBeatPower = 0;
    private int comboCount = 0;
    private int beatPowerStage = 0;

    void Awake()
    {
        // Reference orbiting Aimer and DataManager 
        playerInput = GetComponentInParent<PlayerInput>();
        beatPowerBar.InitializeMaxPerSegment(beatPowerMax / 4);
    }

    void Start()
    {
        // Reference Player and orbiting Aimer 
        playerAimer = GameObject.Find("Player/PlayerAimer")?.transform;
        player = GameObject.Find("Player")?.transform;
        // Equip selected primary (or default)
        InitializePrimaryWeapons();

        // Equip selected secondary (or default)
        InitializeSecondaryWeapon();

        // Equip selected melee (or default)
        InitializeMeleeWeapon();
        
        // Set ammo UI to display secondary ammo count
        //if (ammoUI != null)
        //{
        //    ammoUI.UpdateGrenadeCount(currentSecondaryScript.GetCurrentAmmo());
        //}
        // Start not in combo state
        lastBeatSkipped = true;
    }

    void Update()
    {

        if (Time.timeScale == 0f)
        {
            // Do not spawn attacks if time is froze (ex: paused)
            return;
        }

        if (beatTracker.WasLastBeatSkipped())
        {
            if (!lastBeatSkipped)
            {
                //Debug.Log("Last beat was skipped!");
                lastBeatSkipped = true;
                beatTracker.ClearBeatIndicatorText();
                DropBeatCombo();
            }
        }
        else
        {
            lastBeatSkipped = false;
        }

        // Primary attack button pressed, fire melee
        if (playerInput.actions["Attack"].triggered && currentPrimaryScript != null)
        {
            SyncedBPMBeat.TimingGrade timingGrade = beatTracker.GetTimingRating();
            switch (timingGrade)
            {
                case SyncedBPMBeat.TimingGrade.Perfect:
                    currentPrimaryScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(2f);
                    break;
                case SyncedBPMBeat.TimingGrade.Good:
                    currentPrimaryScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(1f);
                    break;
                case SyncedBPMBeat.TimingGrade.Bad:
                    currentPrimaryScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(1f);
                    break;
                case SyncedBPMBeat.TimingGrade.Miss:
                    currentPrimaryScript.Fire(beatPowerStage, true);
                    DropBeatCombo();
                    break;
            }
        }
        // Secondary attack button pressed, fire secondary
        if (playerInput.actions["SecAttack"].triggered && currentSecondaryScript != null)
        {
            if (beatPowerStage < 1)
            {
                beatPowerBar.FlashBackgroundRed();
            } else {
                currentSecondaryScript.Fire(beatPowerStage, false);
                ExpendBeatCombo();
            }
        }
        // Secondary attack button pressed, fire melee
        if (playerInput.actions["MeleeAttack"].triggered && currentMeleeScript != null)
        {
            SyncedBPMBeat.TimingGrade timingGrade = beatTracker.GetTimingRating();
            switch (timingGrade)
            {
                case SyncedBPMBeat.TimingGrade.Perfect:
                    currentMeleeScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(2f);
                    break;
                case SyncedBPMBeat.TimingGrade.Good:
                    currentMeleeScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(1f);
                    break;
                case SyncedBPMBeat.TimingGrade.Bad:
                    currentMeleeScript.Fire(beatPowerStage, false);
                    IncreaseBeatCombo(1f);
                    break;
                case SyncedBPMBeat.TimingGrade.Miss:
                    currentMeleeScript.Fire(beatPowerStage, true);
                    DropBeatCombo();
                    break;
            }
        }
        if (playerInput.actions["WeaponScrollLeft"].triggered && currentPrimaryScript != null)
        {
            EquipPreviousPrimarYWeapon();
        }
        if (playerInput.actions["WeaponScrollRight"].triggered && currentPrimaryScript != null)
        {
            EquipNextPrimaryWeapon();
        }
    }

    private void InitializePrimaryWeapons() {
        GameObject primary1 = Resources.Load<GameObject>($"Prefabs/Primaries/Primary1");
        if (primary1 != null)
        {
            primaryPrefab1 = Instantiate(primary1, weaponHolder);
            currentPrimaryScript = primaryPrefab1.GetComponent<WeaponBase>();
            AssignFirePoint(currentPrimaryScript, playerAimer);
            primaryWeapons[0] = primaryPrefab1;
        }
        else
        {
            Debug.LogWarning($"Weapon prefab 'Primary1' not found in Prefabs/Primaries/");
        }
        
        GameObject primary2 = Resources.Load<GameObject>($"Prefabs/Primaries/Primary2");
        if (primary2 != null)
        {
            primaryPrefab2 = Instantiate(primary2, weaponHolder);
            WeaponBase primaryScript = primaryPrefab2.GetComponent<WeaponBase>();
            AssignFirePoint(primaryScript, playerAimer);
            primaryWeapons[1] = primaryPrefab2;
        }
        else
        {
            Debug.LogWarning($"Weapon prefab 'Primary2' not found in Prefabs/Primaries/");
        }

        GameObject primary3 = Resources.Load<GameObject>($"Prefabs/Primaries/Primary3");
        if (primary3 != null)
        {
            primaryPrefab3 = Instantiate(primary3, weaponHolder);
            WeaponBase primaryScript = primaryPrefab3.GetComponent<WeaponBase>();
            AssignFirePoint(primaryScript, playerAimer);
            primaryWeapons[2] = primaryPrefab3;
        }
        else
        {
            Debug.LogWarning($"Weapon prefab 'Primary3' not found in Prefabs/Primaries/");
        }
    }



    void InitializeSecondaryWeapon()
    {
        GameObject secondary = Resources.Load<GameObject>($"Prefabs/Secondaries/SecondaryWeapon2");
        if (secondary != null)
        {
            secondaryPrefab = Instantiate(secondary, weaponHolder);
            currentSecondaryScript = secondaryPrefab.GetComponent<WeaponBase>();
            AssignFirePoint(currentSecondaryScript, playerAimer);
        }
        else
        {
            Debug.LogWarning($"Weapon prefab 'SecondaryWeapon2' not found in Prefabs/Secondaries/");
        }
    }

    void InitializeMeleeWeapon()
    {
        GameObject melee = Resources.Load<GameObject>($"Prefabs/Melee/Melee1");
        if (melee != null)
        {
            meleePrefab = Instantiate(melee, weaponHolder);
            currentMeleeScript = meleePrefab.GetComponent<WeaponBase>();
            AssignFirePoint(currentMeleeScript, player);
            AssignFireAim(currentMeleeScript, playerAimer);
        }
        else
        {
            Debug.LogWarning($"Weapon prefab 'Melee1' not found in Prefabs/Melee/");
        }
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

    private void DropBeatCombo()
    {
        currentBeatPower = 0;
        comboCount = 0;
        beatPowerStage = 0;
        beatPowerBar.ResetGuage();
    }

    private void ExpendBeatCombo()
    {
        currentBeatPower = 0;
        comboCount = 0;
        beatPowerStage = 0;
        beatPowerBar.EmptyGuageUponUse();
    }

    private void IncreaseBeatCombo(float multiplier)
    {
        currentBeatPower = currentBeatPower + (beatPowerPerBeatHit * multiplier);
        if (currentBeatPower > beatPowerMax)
        {
            currentBeatPower = beatPowerMax;
        }
        comboCount++;
        beatPowerStage = Mathf.FloorToInt(currentBeatPower / (beatPowerMax / 4));
        beatPowerBar.SetGuageValue(currentBeatPower);
        beatPowerBar.SetComboAmount(comboCount);
    }

    private void EquipNextPrimaryWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % 3;
        currentPrimaryScript = primaryWeapons[currentWeaponIndex].GetComponent<WeaponBase>();
    }

    private void EquipPreviousPrimarYWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 2) % 3;
        currentPrimaryScript = primaryWeapons[currentWeaponIndex].GetComponent<WeaponBase>();
    }
}