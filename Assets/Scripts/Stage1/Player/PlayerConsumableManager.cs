using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableManager : MonoBehaviour
{
    [SerializeField] public int healValue = 40;
    [SerializeField] private BeatIndicatorUI consumableUI;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerHealth playerHealth;
    public int playerMedkits = 0;

    void Start()
    {
        // Update UI to show consumable amount
        if (consumableUI != null)
        {
            //consumableUI.UpdateConsumableCount(playerMedkits);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            // Do not use if time is froze (ex: paused)
            return;
        }

        // If consumable button pressed and player has medkits
        // Heal player damage, decrement available medkit count, update UI
        if (playerInput.actions["Crouch"].triggered && playerHealth != null)
        {
            if (playerMedkits > 0)
            {
                playerHealth.HealDamage(healValue);
                playerMedkits--;
                //consumableUI.UpdateConsumableCount(playerMedkits);
            }
            else
            {
                //consumableUI.FlashRedTwice();
            }
   
        }

    }
}
