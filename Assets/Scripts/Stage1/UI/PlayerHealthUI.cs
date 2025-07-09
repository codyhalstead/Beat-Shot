using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float health)
    {
        // Set UI max health
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        // Adjust health slider UI
        slider.value = health;
    }
}