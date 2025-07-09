using System.Collections;
using TMPro;
using UnityEngine;

public class ConsumableUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consumableCountText;
    private Coroutine flashCoroutine;
    private Color originalColor;

    private void Awake()
    {
        originalColor = consumableCountText.color;
    }

    public void UpdateConsumableCount(int count)
    {
        // Update consumable UI
        consumableCountText.text = "x" + count;
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
            consumableCountText.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            consumableCountText.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }
}
