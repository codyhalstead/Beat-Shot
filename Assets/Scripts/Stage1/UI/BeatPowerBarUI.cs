using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeatPowerBarUI : MonoBehaviour
{
    public Slider power1Slider;
    public Slider power2Slider;
    public Slider power3Slider;
    public Slider power4Slider;
    public TextMeshProUGUI comboNumberText;
    public Image power1SliderBackFill;
    public Image power2SliderBackFill;
    public Image power3SliderBackFill;
    public Image power4SliderBackFill;
    public Image power1SliderFill;
    public Image power2SliderFill;
    public Image power3SliderFill;
    public Image power4SliderFill;

    public Color guageColor1;
    public Color guageColor2;
    public Color guageColor3;
    public Color guageColor4;
    public Color guageDrainColor;

    [SerializeField] private AudioSource audioSource;
    public AudioClip maxedSound;

    private float maxPerSegment;

    private Coroutine flashCoroutine;
    private bool isFrozen = false;
    private bool isMaxed = false;
    private float currentEnergy;

    private void OnAwake()
    {
        power1SliderFill.color = guageColor1;
        power2SliderFill.color = guageColor2;
        power3SliderFill.color = guageColor3;
        power4SliderFill.color = guageColor4;
    }

    public void InitializeMaxPerSegment(float maxPerSegment)
    {
        power1Slider.maxValue = maxPerSegment;
        power2Slider.maxValue = maxPerSegment;
        power3Slider.maxValue = maxPerSegment;
        power4Slider.maxValue = maxPerSegment;
        this.maxPerSegment = maxPerSegment;
        ResetGuage();
    }

    public void SetGuageValue(float amount)
    {
        if (amount > maxPerSegment * 4)
        {
            amount = maxPerSegment * 4;
        }
        currentEnergy = amount;
        if (isFrozen)
        {
            return;
        }
        power1Slider.value = Mathf.Clamp(amount, 0f, maxPerSegment);
        power2Slider.value = Mathf.Clamp(amount - maxPerSegment, 0f, maxPerSegment);
        power3Slider.value = Mathf.Clamp(amount - 2 * maxPerSegment, 0f, maxPerSegment);
        power4Slider.value = Mathf.Clamp(amount - 3 * maxPerSegment, 0f, maxPerSegment);
        if (currentEnergy == maxPerSegment * 4 )
        {
            if (!isMaxed)
            {
                PlayLevelMaxedSound();
                isMaxed = true;
            }
        }
        else
        {
            isMaxed = false;
        }
    }

    public void ResetGuage()
    {
        SetGuageValue(0);
        comboNumberText.SetText("0");
    }

    public void SetComboAmount(int comboAmount)
    {
        comboNumberText.SetText(comboAmount + "");
    }

    public void FlashBackgroundRed()
    {
        flashCoroutine = StartCoroutine(FlashBackgroundRedEnum());
    }

    private IEnumerator FlashBackgroundRedEnum()
    {
        Color originalColor = power1SliderBackFill.color;
        Color flashColor = Color.red;

        float flashTime = 0.05f;
        int flashCount = 2;

        for (int i = 0; i < flashCount; i++)
        {
            power1SliderBackFill.color = flashColor;
            power2SliderBackFill.color = flashColor;
            power3SliderBackFill.color = flashColor;
            power4SliderBackFill.color = flashColor;

            yield return new WaitForSeconds(flashTime);

            power1SliderBackFill.color = originalColor;
            power2SliderBackFill.color = originalColor;
            power3SliderBackFill.color = originalColor;
            power4SliderBackFill.color = originalColor;

            yield return new WaitForSeconds(flashTime);
        }

        flashCoroutine = null;
    }

    public void EmptyGuageUponUse()
    {
        isFrozen = true;

        power1SliderFill.color = guageDrainColor;
        power2SliderFill.color = guageDrainColor; 
        power3SliderFill.color = guageDrainColor; 
        power4SliderFill.color = guageDrainColor;
        StartCoroutine(ResumeGaugeAfterDelay(0.2f));
        ResetGuage();
    }

    private IEnumerator ResumeGaugeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumeGuageVisual();
    }

    public void ResumeGuageVisual()
    {
        isFrozen = false;

        power1SliderFill.color = guageColor1;
        power2SliderFill.color = guageColor2;
        power3SliderFill.color = guageColor3;
        power4SliderFill.color = guageColor4;
        SetGuageValue(currentEnergy);
    }

    public void PlayLevelMaxedSound()
    {
        if (audioSource != null && maxedSound != null)
        {
            audioSource.PlayOneShot(maxedSound);
        }
    }

}
