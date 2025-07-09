using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyUI : MonoBehaviour
{
    public Slider slider;

    public void SetMaxEnergy(float energy)
    {
        // Set max energy for UI
        slider.maxValue = energy;
        slider.value = energy;
    }

    public void SetEnergy(float energy)
    {
        // Adjust slider for UI current energy
        slider.value = energy;
    }
}