using System.Collections;
using UnityEngine;

public class FixedBPMBeat : MonoBehaviour
{
    public float bpm = 120f;
    public BeatIndicatorUI beatUI;

    private Coroutine beatCoroutine;

    void Start()
    {
        float interval = 60f / bpm;  
        StartCoroutine(BeatRoutine(interval));
    }

    IEnumerator BeatRoutine(float interval)
    {
        while (true)
        {
            TriggerBeat();
            yield return new WaitForSeconds(interval);
        }
    }

    void TriggerBeat()
    {
        beatUI.TurnGreen();

        if (beatCoroutine != null)
            StopCoroutine(beatCoroutine);
        beatCoroutine = StartCoroutine(TurnRedAfterDelay(0.1f));
    }

    IEnumerator TurnRedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        beatUI.TurnBlack();
    }

    public void hideBeatIndicatorText()
    {
        beatUI.ShowNoRating();
    }
}