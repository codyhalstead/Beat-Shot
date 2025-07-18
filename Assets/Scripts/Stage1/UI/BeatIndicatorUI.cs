using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeatIndicatorUI : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI consumableCountText;
    //private Coroutine flashCoroutine;
    //private Color originalColor;
    public Image beatIndicator;
    public TextMeshProUGUI ratingText;
    public Color perfectColor;
    public Color goodColor;
    public Color okColor;
    public Color missColor;

    [SerializeField] private float pulseScale = 1.3f;
    [SerializeField] private float pulseDuration = 0.15f;
    private Coroutine pulseCoroutine;
    private Vector3 originalScale;

    private void Awake()
    {
        //originalColor = consumableCountText.color;
        TurnBlack();
        originalScale = ratingText.transform.localScale;
    }

    public void TurnGreen()
    {
        beatIndicator.color = Color.green;
    }

    public void TurnBlack()
    {
        beatIndicator.color = Color.black;
    }

    public void ShowPerfectRating()
    {
        // Update UI text
        ratingText.color = perfectColor;
        ratingText.text = "Perfect";
        PulseText();
    }

    public void ShowGoodRating()
    {
        // Update UI text
        ratingText.color = goodColor;
        ratingText.text = "Good";
        PulseText();
    }

    public void ShowOKRating()
    {
        // Update UI text
        ratingText.color = okColor;
        ratingText.text = "OK";
        PulseText();
    }

    public void ShowMissRating()
    {
        // Update UI text
        ratingText.color = missColor;
        ratingText.text = "Miss";
        PulseText();
    }

    public void ShowNoRating()
    {
        // Update UI text
        ratingText.text = "";
        //ratingText.color = Color.white;
    }

    private void PulseText()
    {
        // Always reset scale to prevent stacking
        ratingText.transform.localScale = originalScale;

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        pulseCoroutine = StartCoroutine(PulseCoroutine());
    }

    private IEnumerator PulseCoroutine()
    {
        Vector3 targetScale = originalScale * pulseScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / (pulseDuration / 2f);
            ratingText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / (pulseDuration / 2f);
            ratingText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        ratingText.transform.localScale = originalScale;
    }

    public void UpdateConsumableCount(int count)
    {
        // Update consumable UI
        //consumableCountText.text = "x" + count;
    }

    public void FlashRedTwice()
    {
        // Prevent overlapping flashes
        //if (flashCoroutine != null)
        //{
        //    StopCoroutine(flashCoroutine);
        //}
        //flashCoroutine = StartCoroutine(FlashTextRoutine());
    }

    //private IEnumerator FlashTextRoutine()
    //{
    //    int flashes = 2;
    //    float flashDuration = 0.2f;

    //    for (int i = 0; i < flashes; i++)
    //    {
            //consumableCountText.color = Color.red;
            //yield return new WaitForSeconds(flashDuration);
            //consumableCountText.color = originalColor;
            //yield return new WaitForSeconds(flashDuration);
    //    }

    //    flashCoroutine = null;
    //}
}
