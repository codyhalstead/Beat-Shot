using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyCountText;
    private Coroutine flashCoroutine;
    private Color originalColor;

    private void Awake()
    {
        originalColor = currencyCountText.color;
    }

    public void UpdateCurrencyCount(int count)
    {
        // Update currency UI
        currencyCountText.text = count + "";
    }

    public void FlashRedTwice()
    {
        // Prevent overlapping flashes
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashTextRoutine());
    }

    private IEnumerator FlashTextRoutine()
    {
        int flashes = 2;
        float flashDuration = 0.2f;

        for (int i = 0; i < flashes; i++)
        {
            currencyCountText.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            currencyCountText.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }
}
