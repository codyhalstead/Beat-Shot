using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerSecAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI grenadeCountText;
    private Coroutine flashCoroutine;
    private Color originalColor;

    private void Awake()
    {
        originalColor = grenadeCountText.color;
    }

    public void UpdateGrenadeCount(int count)
    {
        // Update secondary ammo UI
        grenadeCountText.text = "x" + count;
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
            grenadeCountText.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            grenadeCountText.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }
}
