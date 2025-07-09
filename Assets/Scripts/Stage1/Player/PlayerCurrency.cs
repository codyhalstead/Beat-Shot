using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public PlayerCurrencyUI currencyUI;
    public int playerCurrency = 0;

    void Start()
    {
        currencyUI.UpdateCurrencyCount(10000);
    }

    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        if (playerCurrency > 9999999)
        {
            playerCurrency = 9999999;
        }
        currencyUI.UpdateCurrencyCount(playerCurrency);
    }

    public bool RemoveCurrency(int amount)
    {
        if (amount <= playerCurrency)
        {
            playerCurrency -= amount;
            currencyUI.UpdateCurrencyCount(playerCurrency);
            return true;
        }
        currencyUI.FlashRedTwice();
        return false;
    }

}
