using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;
    [SerializeField] private float energyRegenRate = 10f;

    public PlayerEnergyUI energyBar;

    void Start()
    {
        // Set energy to max, update health UI
        currentEnergy = maxEnergy;
        energyBar.SetMaxEnergy(maxEnergy);
    }

    private void Update()
    {
        RegenerateEnergy();
    }

    public bool UseEnergy(float amount)
    {
        if (currentEnergy < amount)
        {
            // Player does not have enough energy to spend, return false
            return false;
        }
        // Subtract used energy, update UI, return true
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        energyBar.SetEnergy(currentEnergy);
        return true;
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            // Player energy not full, regenerate and update UI
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            energyBar.SetEnergy(currentEnergy);
        }
    }

}
