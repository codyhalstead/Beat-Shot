using System.Collections;
using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    public AudioSource audioSource;
    public int sampleSize = 1024;
    public float sensitivity = 1.3f;
    private float smoothedBassEnergy = 0f;
    public float smoothingFactor = 0.2f;

    public BeatIndicatorUI beatUI;

    private float[] samples;
    private float[] freqBand;
    private float[] bandBuffer;
    private float[] bufferDecrease;

    private float prevEnergy = 0f;

    private Coroutine beatCoroutine;

    void Start()
    {
        samples = new float[sampleSize];
        freqBand = new float[8];
        bandBuffer = new float[8];
        bufferDecrease = new float[8];
    }

    void Update()
    {
        // Get spectrum data
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);


        // Calculate bass energy between 20 Hz and 250 Hz
        float bassEnergy = GetBassEnergy(samples, sampleSize, 20f, 250f);

        // Smooth the energy to reduce jitter
        smoothedBassEnergy = Mathf.Lerp(smoothedBassEnergy, bassEnergy, smoothingFactor);

        // Detect beat when energy spikes above threshold compared to previous smoothed energy
        if (smoothedBassEnergy > prevEnergy * sensitivity)
        {
            beatUI.TurnGreen();

            // Stop existing coroutine if running, then start timer to turn red
            if (beatCoroutine != null)
                StopCoroutine(beatCoroutine);
            beatCoroutine = StartCoroutine(TurnRedAfterDelay(0.1f));
        }

        prevEnergy = smoothedBassEnergy;
    }

    float GetBassEnergy(float[] spectrum, int size, float minFreq, float maxFreq)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        float binWidth = sampleRate / (float)size;

        int minBin = Mathf.Clamp(Mathf.FloorToInt(minFreq / binWidth), 0, size - 1);
        int maxBin = Mathf.Clamp(Mathf.CeilToInt(maxFreq / binWidth), 0, size - 1);

        float energy = 0f;
        for (int i = minBin; i <= maxBin; i++)
        {
            energy += spectrum[i];
        }

        return energy;
    }

    IEnumerator TurnRedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        beatUI.TurnBlack();
    }
}